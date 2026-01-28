using System.Collections;
using UnityEngine;

public class PlayerUnit : MonoBehaviour
{
    [SerializeField] private PlayerStats stats;
    [SerializeField] private Equipment weapon;
    [SerializeField] private Equipment armor;

    [Header("Runtime")]
    [SerializeField] private int currentHP;
    [SerializeField] private EnemyUnit currentTarget;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private SpriteRenderer sr;

    public PlayerStats Stats => stats;
    public int MaxHP => stats.MaxHP;

    public ElementType CurrentElement =>
        weapon != null ? weapon.Element : ElementType.Physical;

    private void Awake()
    {
        if (stats == null)
            stats = GetComponent<PlayerStats>();

        // AUTO initialize from RunManager
        if (RunManager.Instance != null)
            Initialize(RunManager.Instance.Player);
    }

    private void Start()
    {
        //UpdateStats();
        //sr = GetComponent<SpriteRenderer>();
       
        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>();
    }

    public void Initialize(PlayerRunData runData)
    {
        stats.Initialize(runData);
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

}
