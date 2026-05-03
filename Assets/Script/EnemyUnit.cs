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
    [SerializeField] protected bool isBroken;
    [SerializeField] protected int maxEnergy = 100;
    [SerializeField] protected int currentEnergy;
    [SerializeField] protected int energyRegenPerTurn = 10;
    [SerializeField] public Image sr;
    [SerializeField] public Animator anim;
    [SerializeField] private Slider HPbar;
    protected List<ElementType> runtimeWeaknesses = new();
    private bool ultimateQueued;
    protected TurnManager turnManager;
    protected EnemyAttack currentAttack;
    [Header("Attacks")]
    [SerializeField] private List<AttackBase> attacks;
    protected Dictionary<EnemyAttack, int> cooldowns = new();
    private PlayerUnit player;

    public int Speed => data.Speed;
    public EnemyData Data => data;
    private void Awake()
    {
        player = FindFirstObjectByType<PlayerUnit>();
    }
    protected virtual void Start()
    {
        Setup();
        sr = GetComponent<Image>();
        HPbar.maxValue = data.MaxHP;
        HPbar.value = data.MaxHP;
    }

    public virtual void Initialize(EnemyData enemyData)
    {
        data = enemyData;
        Setup();
    }

    protected void Setup()
    {
        currentHP = data.MaxHP;
        //currentToughness = data.MaxToughness;
        currentEnergy = maxEnergy;
        isBroken = false;

        /* cooldowns.Clear();
         foreach (var atk in data.Attacks)
             cooldowns[atk] = 0;*/
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
            return;
        }

        
        var action = ChooseAttack();
        Debug.Log("Animation String = [" + action.AnimationString + "]");
        anim.SetTrigger(action.AnimationString);
        OnActionFinished();
        /*OnTurnStart();

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
        OnTurnEnd();*/
    }

    protected virtual EnemyAttack ChooseAttack()
    {
        /*List<EnemyAttack> pool = data.Attacks
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

        return pool[Random.Range(0, pool.Count)];*/

        var pool = data.Actions;

        foreach (var action in pool)
        {
            if (UnityEngine.Random.value <= action.Chance)
                return action;
        }

        return pool[UnityEngine.Random.Range(0, pool.Count)];
    }

    /*protected bool CanUseAttack(EnemyAttack atk)
    {
        if (atk.UsesEnergy && currentEnergy < atk.EnergyCost)
            return false;

        return true;
    }*/

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

    /*protected virtual void ExecuteEffect(AttackEffect effect, PlayerUnit player)
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
    }*/

    protected void ExecuteAttack(AttackBase attack, PlayerUnit target)
    {
        if (attack == null || target == null)
            return;

        for (int i = 0; i < attack.HitCount; i++)
        {
            target.TakeDamage(attack.BaseDamage, attack.Element);

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

        if (player != null)
            player.SetTarget(this);
    }
}
