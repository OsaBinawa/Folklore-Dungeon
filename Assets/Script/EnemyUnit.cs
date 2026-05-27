using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class EnemyUnit : MonoBehaviour, IPointerClickHandler
{
    [Header("Data")]
    [SerializeField] protected EnemyData data;
    public static event Action OnEnemyDied;
    
    [Header("Runtime")]
    [SerializeField] protected int currentHP;
    [SerializeField] protected int currentToughness;
    [SerializeField] protected int scaledMaxHP;
    [SerializeField] protected float damageMultiplier = 1f;
    [SerializeField] protected bool isBroken;
    [SerializeField] protected int maxEnergy = 100;
    [SerializeField] protected int currentEnergy;
    [SerializeField] protected int energyRegenPerTurn = 10;
    [SerializeField] public Image sr;
    [SerializeField] public Animator anim;
    [SerializeField] private Slider HPbar;

    [Header("Runtime Buffs")]
    protected float attackMultiplier = 1f;
    protected float speedMultiplier = 1f;

    [SerializeField] private GameObject targetIndicator;
    protected List<ElementType> runtimeWeaknesses = new();
    private bool ultimateQueued;
    protected TurnManager turnManager;
    protected EnemyAttack currentAttack;
    [Header("Attacks")]
    [SerializeField] private List<AttackBase> attacks;
    protected Dictionary<EnemyAttack, int> cooldowns = new();
    private PlayerUnit player;

    public int Speed => Mathf.RoundToInt(data.Speed * speedMultiplier);
    public EnemyData Data => data;
    private void Awake()
    {
        player = FindFirstObjectByType<PlayerUnit>();
    }
    protected virtual void Start()
    {
        Setup();
        //sr = GetComponent<Image>();
        if (targetIndicator != null)
            targetIndicator.SetActive(false);
        HPbar.maxValue = currentHP;
        HPbar.value = currentHP;
    }
    public void SetTargeted(bool value)
    {
        if (targetIndicator != null)
            targetIndicator.SetActive(value);
    }
    public virtual void Initialize(EnemyData enemyData, int difficultyTier)
    {
        data = enemyData;

        if (data.IgnoreDifficultyScaling)
        {
            scaledMaxHP = data.MaxHP;
            damageMultiplier = 1f;
        }
        else
        {
            float hpMultiplier = 1f + (difficultyTier * 1f);

            damageMultiplier = 1f + (difficultyTier * 1f);

            scaledMaxHP = Mathf.RoundToInt(
                data.MaxHP * hpMultiplier
            );
        }

        Setup();
    }

    protected virtual void Setup()
    {
        currentHP = scaledMaxHP > 0
                    ? scaledMaxHP
                    : data.MaxHP;
        isBroken = false;

        runtimeWeaknesses = new List<ElementType>(data.Weaknesses);

        turnManager = FindFirstObjectByType<TurnManager>();
        turnManager?.RegisterEnemy(this);
    }

    protected virtual void OnDestroy()
    {
        turnManager?.UnregisterEnemy(this);
    }
    public void Basic(int atkIndex)
    {
        if (attacks == null || attacks.Count == 0)
            return;

        ExecuteAttack(attacks[atkIndex], player);
        OnActionFinished();
        Debug.Log("test");
    }
    public virtual void Act(PlayerUnit player)
    {
        if (!ultimateQueued && data.Ultimate != null)
        {
            if (currentEnergy >= data.Ultimate.EnergyRequired)
                ultimateQueued = true;
        }
        if (ultimateQueued)
        {
            anim.SetTrigger(data.Ultimate.AnimationString);
            currentEnergy = 0;
            ultimateQueued = false;
            Debug.Log("Ulti");
            return;
        }
        var action = ChooseAttack();
        Debug.Log("Animation String = [" + action.AnimationString + "]");
        Debug.Log(name + " chose: " + action.AnimationString);
        anim.SetTrigger(action.AnimationString);
    }

    protected virtual EnemyAttack ChooseAttack()
    {
        var pool = data.Actions;

        foreach (var action in pool)
        {
            if (UnityEngine.Random.value <= action.Chance)
                return action;
        }

        return pool[UnityEngine.Random.Range(0, pool.Count)];
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

    protected void ExecuteAttack(AttackBase attack, PlayerUnit target)
    {
        if (attack == null || target == null)
            return;

        for (int i = 0; i < attack.HitCount; i++)
        {
            int finalDamage = Mathf.RoundToInt(
                attack.BaseDamage *
                attackMultiplier *
                damageMultiplier
            );

            target.TakeDamage(finalDamage, attack.Element);

            foreach (var effect in attack.Effects)
            {
                switch (effect.Type)
                {
                    case EffectType.Damage:
                        target.TakeDamage(effect.Value, attack.Element);
                        break;

                    case EffectType.DelayAV:
                        turnManager?.ModifyAV(target, effect.Value);
                        break;

                    case EffectType.Slow:
                        
                        break;
                }
            }
        }
    }

    public void HpBarUpdate()
    {
        if(HPbar != null)
        {
            HPbar.value = currentHP;
        }
    }

    public void OnActionFinished()
    {
        turnManager.NotifyEnemyActionComplete();
    }
    protected virtual void OnTurnStart() { }
    protected virtual void OnTurnEnd() { }
    protected virtual void OnAttackChosen(EnemyAttack attack) { }
    protected virtual void OnAttackExecuted(EnemyAttack attack) { }
    protected virtual void ModifyDamage(ref int damage) { }

    public virtual void TakeDamage(int damage, ElementType element)
    {
        bool isWeak = runtimeWeaknesses.Contains(element);
        float multiplier = isWeak ? 1.5f : 1f;

        int finalDamage = Mathf.RoundToInt(damage * multiplier);

        currentHP -= finalDamage;

        StartCoroutine(TakingDamageSpriteChange());

        if (currentHP <= 0)
        {
            Die();
        }

        HpBarUpdate();

        Debug.Log($"Damage: {damage} → {finalDamage} (x{multiplier})");
    }


    protected virtual void TriggerBreak()
    {
        isBroken = true;
        currentToughness = 0;
        turnManager?.ModifyAV(this, 3000f);
    }

    protected virtual void Die()
    {
        OnEnemyDied?.Invoke();
        Destroy(gameObject);
    }

    protected void PlayAnimation(string trigger)
    {
        if (!string.IsNullOrEmpty(trigger))
            GetComponent<Animator>()?.SetTrigger(trigger);
    }
    public virtual bool CanBeTargeted()
    {
        return true;
    }
    public IEnumerator TakingDamageSpriteChange()
    {
        sr.color = Color.red;
        yield return new WaitForSeconds(.1f);
        sr.color = Color.white;
    }
    public bool IsWeakTo(ElementType element)
    {
        return runtimeWeaknesses.Contains(element);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();
        SetTargeted(false);
        if (this is EliteTypoEnemy typo && !typo.CanBeTargeted())
            return;
        if (player != null)
            player.SetTarget(this);
    }
}
