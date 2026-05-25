using System;
using System.Collections;
using System.Collections.Generic;
//using NUnit;
using TMPro;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] public WeaponSO EquippedWeapon;
    public static event Action OnPlayerDied;
    //[SerializeField] private Equipment weapon;
    private BattleTargeting targeting;
    [SerializeField] private Equipment armor;

    [Header("Runtime")]
    [SerializeField] private int currentHP;
    [SerializeField] private EnemyUnit currentTarget;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Image sr;
    [SerializeField] private Image weaponSprite;
    [SerializeField] private Image UltIcon;
    [SerializeField] private Color handColor;
    [SerializeField] private Slots weaponSlot;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Animator anim;
    [SerializeField] private int currentSkillPoint;
    [SerializeField] private int MaxSkillPoint = 5;
    [SerializeField] private int currentEnergy = 0;
    [SerializeField] private int maxEnergy = 100;
    private List<ActiveConsumable> activeConsumables = new();
    private List<ItemSO> processedHeld = new();
    [Header("Skill Point UI")]
    [SerializeField] private Image[] skillPointIcons;
    [SerializeField] private Sprite fullSkillPointSprite;
    [SerializeField] private Sprite emptySkillPointSprite;
    [SerializeField] private TMP_Text debugText;

    [Header("Ultimate UI")]
    [SerializeField] private Image ultimateFillImage;
    [SerializeField] private ParticleSystem ultimateReadyEffect;
    private bool ultimateReadyTriggered = false;

    //private WeaponSO CurrentWeapon => weaponSlot.EquippedWeapon;
    public PlayerStats Stats => stats;
    public float MaxHP => stats.MaxHP;
    public ElementType CurrentElement =>
        EquippedWeapon != null ? EquippedWeapon.Element : ElementType.None;

    private void Awake()
    {
        if (stats == null)
            stats = FindFirstObjectByType<PlayerStats>();

        if (weaponSlot == null)
            weaponSlot = FindFirstObjectByType<Slots>();
        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();
        targeting = FindFirstObjectByType<BattleTargeting>();
        if (RunManager.Instance != null)
        {
            Initialize(RunManager.Instance.Player);
        }
        EquipWeapon(weaponSlot.EquippedWeapon);
        currentSkillPoint = 5;
        UpdateSkillPointUI();
        /*if (runManager == null)
        {
            runManager = FindAnyObjectByType<RunManager>();
        }
        */
    }
    public void Initialize(PlayerRunData runData)
    {
        stats.Initialize(runData);
        //stats.RecalculateStats();
        stats.RecalculateStatBuffs(weaponSlot);
        foreach (var item in inventory.HeldConsumables)
        {
            UseConsumable(item);
            processedHeld.Add(item);
        }
        inventory.ClearHeldConsumables();
        Debug.Log("PlayerUnit initialized");
    }

    private void Start()
    {
        //UpdateStats();
        //sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        CheckHeldConsumables();
        SyncWeaponFromSlot();
    }
    private void CheckHeldConsumables()
    {
        if (inventory == null) return;

        List<ItemSO> toRemove = new();

        foreach (var item in inventory.HeldConsumables)
        {
            if (!processedHeld.Contains(item))
            {
                UseConsumable(item);
                processedHeld.Add(item);

                toRemove.Add(item); // mark for removal
            }
        }

        // remove after loop (important)
        foreach (var item in toRemove)
        {
            inventory.RemoveHeld(item);
        }
    }

    private void ApplyBuff()
    {
        if (inventory != null)
        {
            foreach (var item in inventory.HeldConsumables)
            {
                UseConsumable(item); // apply immediately
            }
        }

    }
  
    private void SyncWeaponFromSlot()
    {
        if (weaponSlot != null)
            EquippedWeapon = weaponSlot.EquippedWeapon;
        EquipWeapon(weaponSlot.EquippedWeapon);
    }
    
    /*public void UpdateStats()
    {
        stats.Recalculate(weapon, armor);
        currentHP = stats.FinalHP;
    }*/

    public void BasicAttack()
    {
        if (currentTarget == null)
        {
            Debug.LogWarning("No target");
            return;
        }

        currentTarget.TakeDamage(stats.FinalAttack, CurrentElement);
        ApplyAdjacentHit();
        if (currentSkillPoint < MaxSkillPoint)
        {
            currentSkillPoint++;
            UpdateSkillPointUI();
        }
        else
        {
            Debug.Log("Skill Point reach max");
        }

        foreach (var buff in weaponSlot.OwnedBuffs)
        {
            if (buff.slowChance)
            {
                if (UnityEngine.Random.value < 0.3f)
                {
                    turnManager.ModifyAV(currentTarget, 10);
                }
            }
        }

        Debug.Log("Player uses Basic Attack");
        turnManager.NotifyPlayerActionComplete();
    }

    public void TakeDamage(int amount, ElementType element)
    {
        stats.TakesDamage(amount, weaponSlot);

        StartCoroutine(TakingDamageSpriteChange());
        Debug.Log("Player HP now: " + stats.CurrentHP);

        foreach (var buff in weaponSlot.OwnedBuffs)
        {
            if (buff.counter && currentTarget != null)
            {
                currentTarget.TakeDamage(stats.FinalAttack, CurrentElement);
            }
        }

        if (Stats.CurrentHP <= 0)
        {
            Died();
        }
    }

    private void Died()
    {
        OnPlayerDied?.Invoke();
    }

    public void EquipWeapon(WeaponSO newWeapon)
    {
        EquippedWeapon = newWeapon;

        // Update UI
        if (weaponSprite != null && newWeapon != null)
        {
            weaponSprite.sprite = newWeapon._WeaponSprite;
        }
        if (UltIcon != null && newWeapon != null)
        {
            UltIcon.sprite = newWeapon.UltIcon;
        }
    }

    public void PerformSkill()
    {
        if (currentTarget == null || EquippedWeapon == null)
            return;
        if (currentSkillPoint >= 1) 
        { currentSkillPoint -= 2; }
        UpdateSkillPointUI();
        StartCoroutine(AttackRoutine(
            EquippedWeapon.SkillAnimation,
            EquippedWeapon.SkillTargetType,
            EquippedWeapon.SkillEffects
        ));
    }

    public void PerformUltimate()
    {
        if (currentTarget == null || EquippedWeapon == null)
            return;

        if (currentEnergy < EquippedWeapon.UltimateEnergyCost)
        {
            Debug.Log("Not enough energy!");
            return;
        }

        currentEnergy -= EquippedWeapon.UltimateEnergyCost;

        StartCoroutine(AttackRoutine(
            EquippedWeapon.UltimateAnimation,
            EquippedWeapon.UltimateTargetType,
            EquippedWeapon.UltimateEffects
        ));
        UpdateUltimateUI();
    }
    private IEnumerator AttackRoutine(
        AnimationClip animClip,
        TargetType targetType,
        WeaponEffect[] effects)
       {
        // Play animation
        anim.SetTrigger(animClip.name);

        // Get targets
        EnemyUnit[] targets = targeting.GetTargets(targetType, currentTarget);

        // Wait for hit timing
        yield return new WaitForSeconds(0.5f);

        // Apply effects
        foreach (var effect in effects)
        {
            effect.Apply(this, targets);
        }

        // Gain energy (optional)
        currentEnergy = Mathf.Min(currentEnergy + 20, maxEnergy);
        UpdateUltimateUI();
        // End turn
        FindFirstObjectByType<TurnManager>().NotifyPlayerActionComplete();
    }

    private void ApplyDamage(int baseValue)
    {
        int totalAttack = stats.FinalAttack;

        float bonusPercent = 0f;

        foreach (var buff in weaponSlot.OwnedBuffs)
        {
            if (!buff.bonusVsNonWeak)
                continue;

            if (!currentTarget.IsWeakTo(CurrentElement))
            {
                bonusPercent += buff.atkBonusVsNonWeakPercent;
            }
        }

        totalAttack = Mathf.RoundToInt(totalAttack * (1 + bonusPercent / 100f));

        /*if (EquippedWeapon != null)
            totalAttack += EquippedWeapon.AttackBonus;*/

        int damage = totalAttack + baseValue;

        Debug.Log(
            $"[Damage] BaseAttack: {stats.FinalAttack} | " +
            $"Bonus%: {bonusPercent} | " +
            $"FinalDamage: {damage}"
        );

        currentTarget.TakeDamage(damage, CurrentElement);
    }


    private void ApplyDelay(int amount)
    {
        turnManager.ModifyAV(currentTarget, amount);
    }


    public IEnumerator TakingDamageSpriteChange()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = handColor;
    }

    public int GetCurrentHP()
    {
        return stats.CurrentHP;
    }

    public int GetMaxHP()
    {
        return stats.MaxHP;
    }

    public void SetTarget(EnemyUnit target)
    {
        if (currentTarget != null)
            currentTarget.SetTargeted(false);

        currentTarget = target;

        if (currentTarget != null)
            currentTarget.SetTargeted(true);
    }

    public void NotifyTurnEnd()
    {
        TickConsumables();
        turnManager.NotifyPlayerActionComplete();
    }

    private void UpdateSkillPointUI()
    {
        for (int i = 0; i < skillPointIcons.Length; i++)
        {
            if (i < currentSkillPoint)
                skillPointIcons[i].sprite = fullSkillPointSprite;
            else
                skillPointIcons[i].sprite = emptySkillPointSprite;
        }
    }
    private void ApplyAdjacentHit()
    {
        foreach (var buff in weaponSlot.OwnedBuffs)
        {
            if (!buff.adjacentHit)
                continue;

            List<EnemyUnit> enemies = GetEnemyList();
            int index = enemies.IndexOf(currentTarget);

            if (index == -1)
                return;

            if (index > 0)
            {
                enemies[index - 1].TakeDamage(stats.FinalAttack, CurrentElement);
            }

            if (index < enemies.Count - 1)
            {
                enemies[index + 1].TakeDamage(stats.FinalAttack, CurrentElement);
            }
        }
    }

    public void UseConsumable(ItemSO item)
    {
        // Instant heal
        if (item.Heal > 0)
        {
            stats.Heal(item.Heal);
        }

        // Check if already active → refresh duration
        ActiveConsumable existing = activeConsumables
            .Find(c => c.source == item);

        if (existing != null)
        {
            existing.remainingTurns = item.duration;
        }
        else
        {
            activeConsumables.Add(new ActiveConsumable(item));
        }

        RecalculateConsumableStats();
    }

    private void RecalculateConsumableStats()
    {
        int bonusAtk = 0;
        int bonusSpd = 0;

        foreach (var c in activeConsumables)
        {
            bonusAtk += c.atkMod;
            bonusSpd += c.spdMod;
        }

        stats.RecalculateStats();
        stats.RecalculateStatBuffs(weaponSlot);

        // Apply consumable bonus AFTER everything
        stats.SetConsumableBonus(bonusAtk, bonusSpd);
    }

    public void TickConsumables()
    {
        for (int i = activeConsumables.Count - 1; i >= 0; i--)
        {
            activeConsumables[i].remainingTurns--;

            if (activeConsumables[i].remainingTurns <= 0)
            {
                activeConsumables.RemoveAt(i);
            }
        }

        RecalculateConsumableStats();
    }
    private void UpdateUltimateUI()
    {
        if (ultimateFillImage != null)
        {
            // Reverse fill
            ultimateFillImage.fillAmount =
                1f - ((float)currentEnergy / maxEnergy);
        }

        bool isReady =
            EquippedWeapon != null &&
            currentEnergy >= EquippedWeapon.UltimateEnergyCost;

        // Play effect once
        if (isReady && !ultimateReadyTriggered)
        {
            ultimateReadyTriggered = true;

            if (ultimateReadyEffect != null)
                ultimateReadyEffect.Play();
        }

        // Reset when no longer ready
        if (!isReady)
        {
            ultimateReadyTriggered = false;

            if (ultimateReadyEffect != null)
                ultimateReadyEffect.Stop();
        }
    }
    private List<EnemyUnit> GetEnemyList()
    {
        var field = typeof(TurnManager).GetField("enemies",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        return field.GetValue(turnManager) as List<EnemyUnit>;
    }

}
