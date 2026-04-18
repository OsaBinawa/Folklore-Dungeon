using System.Collections;
using System.Collections.Generic;
using NUnit;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] public WeaponSO EquippedWeapon;
    //[SerializeField] private Equipment weapon;
    [SerializeField] private Equipment armor;

    [Header("Runtime")]
    [SerializeField] private int currentHP;
    [SerializeField] private EnemyUnit currentTarget;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private Image sr;
    [SerializeField] private Slots weaponSlot;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Animator anim;
    [SerializeField] private int currentSkillPoint;
    [SerializeField] private int MaxSkillPoint = 5;
    private List<ActiveConsumable> activeConsumables = new();
    private List<ItemSO> processedHeld = new();
    [Header("Skill Point UI")]
    [SerializeField] private Image[] skillPointIcons;
    [SerializeField] private Sprite fullSkillPointSprite;
    [SerializeField] private Sprite emptySkillPointSprite;
    [SerializeField] private TMP_Text debugText;

    //private WeaponSO CurrentWeapon => weaponSlot.EquippedWeapon;
    public PlayerStats Stats => stats;
    public int MaxHP => stats.MaxHP;
    public ElementType CurrentElement =>
        EquippedWeapon != null ? EquippedWeapon.Element : ElementType.Physical;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        if (weaponSlot == null)
            weaponSlot = FindFirstObjectByType<Slots>();
        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();

        if (RunManager.Instance != null)
        {
            Initialize(RunManager.Instance.Player);
        }
        
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
                if (Random.value < 0.3f)
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
    }

    public void PerformWeaponAttack()
    {
        if (!weaponSlot.HasWeapon || currentTarget == null)
            return;
        if (currentSkillPoint >= EquippedWeapon.skillCost)
        {
            if (EquippedWeapon.AttackAnimation != null)
            {
                anim.SetTrigger("PenAttack");
            }
            Debug.Log($"[Attack] Using Weapon: {EquippedWeapon.WeaponName}");
            Debug.Log($"[Attack] Target: {currentTarget.name}");
            debugText.text = "[Attack] Using Weapon: " + EquippedWeapon.WeaponName;
            foreach (var effect in EquippedWeapon.Effects)
            {
                ResolveEffect(effect);
            }
            ApplyAdjacentHit();
            foreach (var buff in weaponSlot.OwnedBuffs)
            {
                if (buff.slowChance)
                {
                    if (Random.value < 0.3f)
                    {
                        turnManager.ModifyAV(currentTarget, 10);
                    }
                }
            }

            currentSkillPoint -= EquippedWeapon.skillCost;
            UpdateSkillPointUI();
        }
        //turnManager.NotifyPlayerActionComplete();
    }

    private void ResolveEffect(AttackEffect effect)
    {
        switch (effect.Type)
        {
            case EffectType.Damage:
                ApplyDamage(effect.Value);
                break;

            case EffectType.DelayAV:
                ApplyDelay(effect.Value);
                break;
        }
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

        if (EquippedWeapon != null)
            totalAttack += EquippedWeapon.AttackBonus;

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
        sr.color = Color.white;
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
        currentTarget = target;
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

    private List<EnemyUnit> GetEnemyList()
    {
        var field = typeof(TurnManager).GetField("enemies",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);

        return field.GetValue(turnManager) as List<EnemyUnit>;
    }

}
