using System.Collections;
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
    [SerializeField] private Animator anim;
    [SerializeField] private int currentSkillPoint;
    [SerializeField] private int MaxSkillPoint = 5;

    //private WeaponSO CurrentWeapon => weaponSlot.EquippedWeapon;
    public PlayerStats Stats => stats;
    public int MaxHP => stats.MaxHP;
    public ElementType CurrentElement =>
        EquippedWeapon != null ? EquippedWeapon.Element : ElementType.Physical;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();
        if (RunManager.Instance != null)
        {
            Initialize(RunManager.Instance.Player);
        }
        else
        {
            Debug.LogError("RunManager.Instance is NULL in PlayerUnit.Awake()");
        }

        currentSkillPoint = 5;

        /*if (runManager == null)
        {
            runManager = FindAnyObjectByType<RunManager>();
        }
        */
    }

    private void Start()
    {
        //UpdateStats();
        //sr = GetComponent<SpriteRenderer>();
       
        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>(); 
        if (weaponSlot == null)
            weaponSlot = FindFirstObjectByType<Slots>();
        
        SyncWeaponFromSlot();
    }
    private void Update()
    {
       
    }
    private void SyncWeaponFromSlot()
    {
        if (weaponSlot != null)
            EquippedWeapon = weaponSlot.EquippedWeapon;
    }
    public void Initialize(PlayerRunData runData)
    {
        stats.Initialize(runData);
        Debug.Log("PlayerUnit initialized");
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
        if (currentSkillPoint < MaxSkillPoint)
        {
            currentSkillPoint++;
        }
        else
        {
            Debug.Log("Skill Point reach max");
        }
        Debug.Log("Player uses Basic Attack");
        turnManager.NotifyPlayerActionComplete();
    }

    public void TakeDamage(int amount, ElementType element)
    {
        stats.TakesDamage(amount);
        StartCoroutine(TakingDamageSpriteChange());
        Debug.Log("Player HP now: " + stats.CurrentHP);

        /*if (currentHP <= 0)
            Die();*/
    }

    public void PerformWeaponAttack()
    {
        if (!weaponSlot.HasWeapon || currentTarget == null)
            return;
        if (currentSkillPoint >= EquippedWeapon.skillCost)
        {
            // Optional: play animation
            if (EquippedWeapon.AttackAnimation != null)
            {
                // Your animator logic here
                anim.SetTrigger("PenAttack");
            }
            Debug.Log($"[Attack] Using Weapon: {EquippedWeapon.WeaponName}");
            Debug.Log($"[Attack] Target: {currentTarget.name}");
            foreach (var effect in EquippedWeapon.Effects)
            {
                ResolveEffect(effect);
            }
            currentSkillPoint -= EquippedWeapon.skillCost;
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

        if (EquippedWeapon != null)
            totalAttack += EquippedWeapon.AttackBonus;

        int damage = totalAttack + baseValue;

        Debug.Log(
       $"[Damage] BaseAttack: {stats.FinalAttack} | " +
       $"WeaponBonus: {EquippedWeapon.AttackBonus} | " +
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
        turnManager.NotifyPlayerActionComplete();
    }

}
