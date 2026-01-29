using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class EnemyUnit : MonoBehaviour, IPointerClickHandler
{
    [Header("Data")]
    [SerializeField] protected EnemyData data;

    [Header("Runtime")]
    [SerializeField] protected int currentHP;
    [SerializeField] protected int currentToughness;
    [SerializeField] protected bool isBroken;
    [SerializeField] protected int maxEnergy = 100;
    [SerializeField] protected int currentEnergy;
    [SerializeField] protected int energyRegenPerTurn = 10;
    [SerializeField] private SpriteRenderer sr;

    protected TurnManager turnManager;
    protected EnemyAttack currentAttack;

    protected Dictionary<EnemyAttack, int> cooldowns = new();

    public int Speed => data.Speed;
    public EnemyData Data => data;

    protected virtual void Start()
    {
        Setup();
        sr = GetComponent<SpriteRenderer>();
    }

    public virtual void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        Setup();
    }

    protected void Setup()
    {
        currentHP = data.MaxHP;
        currentToughness = data.MaxToughness;
        currentEnergy = maxEnergy;
        isBroken = false;

        cooldowns.Clear();
        foreach (var atk in data.Attacks)
            cooldowns[atk] = 0;

        turnManager = FindFirstObjectByType<TurnManager>();
        turnManager?.RegisterEnemy(this);
    }

    protected virtual void OnDestroy()
    {
        turnManager?.UnregisterEnemy(this);
    }

    public void Act(PlayerUnit player)
    {
        OnTurnStart();

        currentAttack = ChooseAttack();
        if (currentAttack == null) return;

        if (currentAttack.UsesEnergy)
            currentEnergy -= currentAttack.EnergyCost;

        OnAttackChosen(currentAttack);
        PlayAnimation(currentAttack);

        foreach (var effect in currentAttack.Effects)
            ExecuteEffect(effect, player);

        OnAttackExecuted(currentAttack);

        if (data.IsUnique)
        {
            cooldowns[currentAttack] = currentAttack.Cooldown;
            TickCooldowns();
        }

        RegenerateEnergy();
        OnTurnEnd();
    }

    protected virtual EnemyAttack ChooseAttack()
    {
        List<EnemyAttack> pool = data.Attacks
            .Where(CanUseAttack)
            .ToList();

        if (pool.Count == 0)
            pool = data.Attacks.ToList();

        if (data.IsUnique)
        {
            pool = pool
                .Where(a => cooldowns[a] <= 0)
                .ToList();

            if (pool.Count == 0)
                pool = data.Attacks.ToList();
        }

        foreach (var atk in pool)
            if (Random.value <= atk.Chance)
                return atk;

        return pool[Random.Range(0, pool.Count)];
    }

    protected bool CanUseAttack(EnemyAttack atk)
    {
        if (atk.UsesEnergy && currentEnergy < atk.EnergyCost)
            return false;

        return true;
    }

    protected void TickCooldowns()
    {
        foreach (var key in cooldowns.Keys.ToList())
            if (cooldowns[key] > 0)
                cooldowns[key]--;
    }

    protected void RegenerateEnergy()
    {
        currentEnergy = Mathf.Min(
            currentEnergy + energyRegenPerTurn,
            maxEnergy
        );
    }

    protected virtual void ExecuteEffect(AttackEffect effect, PlayerUnit player)
    {
        switch (effect.Type)
        {
            case EffectType.Damage:
                int dmg = data.BaseDamage + effect.Value;
                ModifyDamage(ref dmg);
                player.TakeDamage(dmg, currentAttack.Element);
                break;

            case EffectType.DelayAV:
                turnManager.ModifyAV(player, effect.Value);
                break;
        }
    }

    protected virtual void OnTurnStart() { }
    protected virtual void OnTurnEnd() { }
    protected virtual void OnAttackChosen(EnemyAttack attack) { }
    protected virtual void OnAttackExecuted(EnemyAttack attack) { }
    protected virtual void ModifyDamage(ref int damage) { }

    public virtual void TakeDamage(int damage, ElementType element)
    {
        currentHP -= damage;

        if (!isBroken && data.Weaknesses.Contains(element))
        {
            currentToughness--;
            if (currentToughness <= 0)
                TriggerBreak();
        }
        StartCoroutine(TakingDamageSpriteChange());
        if (currentHP <= 0)
            Die();
    }

    protected virtual void TriggerBreak()
    {
        isBroken = true;
        currentToughness = 0;
        turnManager?.ModifyAV(this, 3000f);
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected void PlayAnimation(EnemyAttack attack)
    {
        if (!string.IsNullOrEmpty(attack.AnimationTrigger))
            GetComponent<Animator>()?.SetTrigger(attack.AnimationTrigger);
    }

    public IEnumerator TakingDamageSpriteChange()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = Color.white;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player != null)
            player.SetTarget(this);
    }
}
