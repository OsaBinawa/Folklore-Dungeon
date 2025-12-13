using System.Linq;
using UnityEngine;

public class EnemyUnit : MonoBehaviour
{
    public EnemyData data;

    [SerializeField] private int currentHP;
    [SerializeField] private int currentToughness;
    [SerializeField] private float actionGauge;
    [SerializeField] private bool isBroken;

    public int CurrentHP => currentHP;
    public int Speed => data.Speed;
    public int Damage => data.Damage;
    public float ActionGauge => actionGauge;
    public bool IsBroken => isBroken;

    private void Awake()
    {
        currentHP = data.MaxHP;
        currentToughness = data.MaxToughness;
    }
    private void Start()
    {
        currentHP = data.MaxHP;
        currentToughness = data.MaxToughness;

        FindFirstObjectByType<TurnManager>().RegisterEnemy(this);
    }
    public void TakeDamage(int dmg, ElementType element)
    {
        currentHP -= dmg;

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
        currentHP -= 100;
        actionGauge -= 300f;
    }

    public void RecoverFromBreak()
    {
        isBroken = false;
        currentToughness = data.MaxToughness;
    }

    private void OnDestroy()
    {
        FindFirstObjectByType<TurnManager>()?.UnregisterEnemy(this);
    }
    public void AddGauge(float value) => actionGauge += value;
    public void ResetGauge() => actionGauge = 0;
}
