using System;
using UnityEngine;

public class BasicBrokenEnemy : EnemyUnit
{
    public static event Action<string> OnShredProgress;
    [Header("Broken Enemy")]
    [SerializeField] private int explosionDamage = 20;
    [SerializeField] private Sprite[] stages;
    private bool progressedThisTurn = false;
    [SerializeField]private int turnCounter = -1;

    public override void Act(PlayerUnit player)
    {     
        progressedThisTurn = false;
        base.Act(player);

    }

    public void ProgressState()
    {
        if (progressedThisTurn) return;

        progressedThisTurn = true;

        turnCounter++;

        UpdateVisual();

        Debug.Log($"{name} stage: {turnCounter}");

        if (stages != null && turnCounter >= stages.Length - 1)
        {
            Explode();
            return;
        }
    }

    private void UpdateVisual()
    {
        if (stages == null || stages.Length == 0) return;

        int index = Mathf.Clamp(turnCounter, 0, stages.Length - 1);
        sr.sprite = stages[index];

        OnShredProgress?.Invoke(data.name.Replace("(Clone)", "") + " on shredding progress " + (index + 1).ToString() + "/" + stages.Length);
        Debug.Log(data.name.Replace("(Clone)", "") + " on shredding progress " + (index + 1).ToString() + "/3");
    }

    private void Explode()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player != null)
        {
            Debug.Log($"{name} explodes for {explosionDamage}");
            player.TakeDamage(explosionDamage, ElementType.None);
            OnShredProgress?.Invoke(data.name.Replace("(Clone)", "") + " exploding, dealing damage to player");
        }

        Die();

        OnActionFinished();
    }
}
