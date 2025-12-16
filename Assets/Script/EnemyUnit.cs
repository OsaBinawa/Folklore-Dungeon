using UnityEngine;
using System.Linq;

public class EnemyUnit : MonoBehaviour
{
    [SerializeField] private EnemyData data;

    [Header("Runtime")]
    [SerializeField] private int currentHP;
    [SerializeField] private int currentToughness;
    [SerializeField] private bool isBroken;

    public int Speed => data.Speed;
    public int Damage => data.Damage;
    public EnemyData EnemyData => data;

    private TurnManager turnManager;

    private void Start()
    {
        currentHP = data.MaxHP;
        currentToughness = data.MaxToughness;

        turnManager = FindFirstObjectByType<TurnManager>();
        turnManager?.RegisterEnemy(this);
    }

    private void OnDestroy()
    {
        turnManager?.UnregisterEnemy(this);
    }

    // ================= ENEMY TURN =================

    public void Act(PlayerUnit player)
    {
        Debug.Log($"{data.name} attacks player");
        player.TakeDamage(data.Damage);
    }

    // ================= DAMAGE & BREAK =================

    public void TakeDamage(int damage, ElementType element)
    {
        currentHP -= damage;

        if (!isBroken && data.Weaknesses.Contains(element))
        {
            currentToughness--;
            if (currentToughness <= 0)
                TriggerBreak();
        }

        if (currentHP <= 0)
            Die();
    }

    private void TriggerBreak()
    {
        isBroken = true;
        currentToughness = 0;

        
        //turnManager?.DelayUnit(this, 3000f);
        turnManager?.ModifyAV(this, 3000f);
        Debug.Log($"{data.name} is Broken → delayed");
    }

    private void Die()
    {
        Debug.Log($"{data.name} defeated");
        Destroy(gameObject);
    }
}
