using System;
using UnityEngine;

public class EliteMissEnemy : EnemyUnit
{
    public static event Action<string> OnInflictSlow;
    [Header("Miss Settings")]
    [SerializeField] private float delayChance = 0.4f;
    [SerializeField] private float delayAmount = 25f;
    public override void Act(PlayerUnit player)
    {
        base.Act(player);
        TryApplyDelay();
    }
    public void TryApplyDelay()
    {
        PlayerUnit player = FindFirstObjectByType<PlayerUnit>();

        if (player == null) return;

        if (UnityEngine.Random.value <= delayChance)
        {
            Debug.Log($"{name} applies delay!");

            OnInflictSlow?.Invoke("Player slowed by " + data.name);

            turnManager?.ModifyAV(player, delayAmount);
        }
    }
}
