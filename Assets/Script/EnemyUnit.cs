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

    private void Start()
    {
        currentHP = data.MaxHP;
        currentToughness = data.MaxToughness;

        FindFirstObjectByType<TurnManager>()?.RegisterEnemy(this);
    }

    // ======================
    // COMBAT
    // ======================

    public void TakeDamage(int damage, ElementType element)
    {
        currentHP -= damage;

        if (!isBroken && data.Weaknesses.Contains(element))
        {
            currentToughness--;
            if (currentToughness <= 0)
                TriggerBreak();
        }
    }

    private void TriggerBreak()
    {
        isBroken = true;
        currentToughness = 0;

        // Break delay (HSR style)
        FindFirstObjectByType<TurnManager>()
            ?.DelayUnit(this, 3000f);

        Debug.Log($"{data.name} is Broken (AV delayed)");
    }

    private void OnDestroy()
    {
        FindFirstObjectByType<TurnManager>()?.UnregisterEnemy(this);
    }
}
