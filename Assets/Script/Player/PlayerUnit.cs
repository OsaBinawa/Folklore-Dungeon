using System;
using System.Collections;
using System.Collections.Generic;
//using NUnit;
using TMPro;
//using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] public WeaponSO EquippedWeapon;
    public static event Action OnPlayerDied, OnPlayerDamaged;
    public static event Action<string> 
    OnInsufficientSkillPoint,
    OnAttackNoTarget,
    OnPlayerBasicAttack,
    OnPlayerSkill,
    OnUltimateEnergyInsufficient,
    OnPlayerUltimate,
    OnPlayerHeal,
    OnPlayerUsedItem;
    //[SerializeField] private Equipment weapon;
    private BattleTargeting targeting;
    [SerializeField] private Equipment armor;
    private PlayerRunData runData;
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
    [SerializeField] private MainUIManager mainUIManager;
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
    [SerializeField] private GameObject UltimateEffect;
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
        if (mainUIManager==null)
            mainUIManager =FindAnyObjectByType<MainUIManager>();
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
        currentSkillPoint = 3;
        UpdateSkillPointUI();
        /*if (runManager == null)
        {
            runManager = FindAnyObjectByType<RunManager>();
        }
        */
    }
    public void Initialize(PlayerRunData runData)
    {
        this.runData = runData;

        stats.Initialize(runData);
        stats.RecalculateStatBuffs(weaponSlot);

        foreach (var item in inventory.HeldConsumables)
        {
            UseConsumable(item);
            processedHeld.Add(item);
        }

        inventory.ClearHeldConsumables();

        UpdateUltimateUI();
        mainUIManager.RefreshStats();
        Debug.Log("PlayerUnit initialized");
    }

    private void Start()
    {
        //UpdateStats();
        SyncDebugValues();
        //sr = GetComponent<SpriteRenderer>();
    }
    private void Update()
    {
        CheckHeldConsumables();
        SyncWeaponFromSlot();
    }
    private void SyncDebugValues()
    {
        if (runData == null)
            return;

        currentEnergy = runData.CurrentEnergy;
        maxEnergy = runData.MaxEnergy;
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
            OnAttackNoTarget?.Invoke("No target");
            return;
        }
        anim.SetTrigger("BasicAttack");
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
        runData.GainEnergy(10);
        Debug.Log("Player uses Basic Attack");
        OnPlayerBasicAttack?.Invoke("Player is attacking " + currentTarget.name.Replace("(Clone)", ""));
        turnManager.NotifyPlayerActionComplete();
    }

    public void TakeDamage(int amount, ElementType element)
    {
        stats.TakesDamage(amount, weaponSlot);
        OnPlayerDamaged?.Invoke();

        StartCoroutine(TakingDamageSpriteChange());
        Debug.Log("Player HP now: " + stats.CurrentHP);

        foreach (var buff in weaponSlot.OwnedBuffs)
        {
            if (buff.counter && currentTarget != null)
            {
                currentTarget.TakeDamage(Mathf.RoundToInt(stats.FinalAttack * 0.8f),CurrentElement);
                            
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
        if (currentSkillPoint > 1)
        {
            if (currentTarget == null || EquippedWeapon == null)
            {
                OnAttackNoTarget?.Invoke("No Target");
                return;
            }

            // if (currentSkillPoint >= 1)
            // { currentSkillPoint -= 2; }
            currentSkillPoint -=2;
            OnPlayerSkill?.Invoke("Player used skill on " + currentTarget.name.Replace("(Clone)", ""));
            UpdateSkillPointUI();
            StartCoroutine(AttackRoutine(
                EquippedWeapon.SkillAnimation,
                EquippedWeapon.SkillTargetType,
                EquippedWeapon.SkillEffects
            ));
        }
        else
        {
            Debug.Log("Dont Have Point");
            OnInsufficientSkillPoint?.Invoke("Not enough Skill Point");
        }
    }

    public void PerformUltimate()
    {
        if (currentTarget == null || EquippedWeapon == null)
            return;

        if (runData.CurrentEnergy < EquippedWeapon.UltimateEnergyCost)
        {
            Debug.Log("Not enough energy!");
            OnUltimateEnergyInsufficient?.Invoke("Not enough energy");
            return;
        }

        runData.ConsumeEnergy(EquippedWeapon.UltimateEnergyCost);
        OnPlayerUltimate?.Invoke("Player performed ultimate");

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
        runData.GainEnergy(20);
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
        // Heal instantly
        if (item.Heal > 0)
        {
            stats.Heal(item.Heal);
            OnPlayerHeal?.Invoke("Player used healing item");
        }

        // Remove old instance if already active
        activeConsumables.RemoveAll(c => c.source == item);

        // Add new buff
        activeConsumables.Add(new ActiveConsumable(item));

        OnPlayerUsedItem?.Invoke($"Player used {item.Name}");

        UpdateConsumableStats();

        if (mainUIManager != null)
            mainUIManager.RefreshStats();
    }

    private void UpdateConsumableStats()
    {
        int totalAtk = 0;
        int totalSpd = 0;

        foreach (var c in activeConsumables)
        {
            totalAtk += c.atkMod;
            totalSpd += c.spdMod;
        }

        stats.SetConsumableBonus(totalAtk, totalSpd);

        Debug.Log(
            $"Consumables => ATK+{totalAtk} SPD+{totalSpd}"
        );
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

        // Store consumable bonuses
        stats.SetConsumableBonus(bonusAtk, bonusSpd);

        // Rebuild everything
        stats.RecalculateStats();

        Debug.Log(
            $"Consumable totals: ATK+{bonusAtk} SPD+{bonusSpd}"
        );
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

        UpdateConsumableStats();

        if (mainUIManager != null)
            mainUIManager.RefreshStats();
    }
    private void UpdateUltimateUI()
    {
        if (ultimateFillImage != null)
        {
            // Reverse fill
            ultimateFillImage.fillAmount =
                1f - ((float)runData.CurrentEnergy / runData.MaxEnergy);
        }

        bool isReady =
            EquippedWeapon != null &&
            runData.CurrentEnergy >= EquippedWeapon.UltimateEnergyCost;

        // Play effect once
        if (isReady && !ultimateReadyTriggered)
        {
            ultimateReadyTriggered = true;

            if (ultimateReadyEffect != null)
                ultimateReadyEffect.Play();
            if (UltimateEffect != null)
                UltimateEffect.SetActive(true);
        }

        // Reset when no longer ready
        if (!isReady)
        {
            ultimateReadyTriggered = false;

            if (ultimateReadyEffect != null)
                ultimateReadyEffect.Stop();
            if (UltimateEffect != null)
                UltimateEffect.SetActive(false);
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
