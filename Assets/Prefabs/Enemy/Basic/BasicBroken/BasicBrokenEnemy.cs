using UnityEngine;

public class BasicBrokenEnemy : EnemyUnit
{
    [Header("Broken Enemy Settings")]
    [SerializeField] private int turnsBeforeExplosion = 3;
    [SerializeField] private int explosionDamage = 20;
    [SerializeField] private Sprite[] stages; 

    private int turnCounter = 0;
    protected override void OnTurnEnd()
    {
        base.OnTurnEnd();

        turnCounter++;

        UpdateVisual();

        if (turnCounter >= turnsBeforeExplosion)
        {
            Explode();
        }
    }

    private void UpdateVisual()
    {
        if (stages == null || stages.Length == 0) return;

        int index = Mathf.Clamp(turnCounter, 0, stages.Length - 1);
        sr.sprite = stages[index];
    }

    private void Explode()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player != null)
        {
            Debug.Log($"{name} explodes for {explosionDamage} damage");
            player.TakeDamage(explosionDamage, ElementType.None);
        }

        Die();
    }
}
