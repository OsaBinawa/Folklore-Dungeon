using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private PlayerUnit player;
    [SerializeField] private float gaugeMax = 1000f;

    [SerializeField] private List<EnemyUnit> enemies = new List<EnemyUnit>();

    private bool isPlayerTurn;

    public void RegisterEnemy(EnemyUnit enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public void UnregisterEnemy(EnemyUnit enemy)
    {
        enemies.Remove(enemy);
    }
    private void Start()
    {
        LogPredictedTurnOrder();
    }
    private void Update()
    {
        if (isPlayerTurn) return;

        // Player gauge
        player.AddGauge(player.Stats.FinalSpeed * Time.deltaTime);

        if (player.ActionGauge >= gaugeMax)
        {
            Debug.Log($"[TURN] Player acts (Speed: {player.Stats.FinalSpeed})");
            player.ResetGauge();
            isPlayerTurn = true;
            return;
        }

        // Enemy gauges
        foreach (EnemyUnit enemy in enemies)
        {
            if (enemy.CurrentHP <= 0) continue;

            enemy.AddGauge(enemy.Speed * Time.deltaTime);

            if (enemy.ActionGauge >= gaugeMax)
            {
                Debug.Log($"[TURN] Enemy acts: {enemy.data.name} (Speed: {enemy.Speed})");
                StartCoroutine(EnemyTurn(enemy));
                return;
            }
        }
    }


    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
    }

    // ✅ THIS WAS MISSING
    private IEnumerator EnemyTurn(EnemyUnit enemy)
    {
        enemy.ResetGauge();

        // Optional: skip turn if broken
        if (enemy.IsBroken)
        {
            enemy.RecoverFromBreak();
            yield break;
        }

        yield return new WaitForSeconds(0.5f);

        // Enemy attacks player
        player.TakeDamage(enemy.Damage);

        yield return new WaitForSeconds(0.2f);
    }

    private void LogPredictedTurnOrder()
    {
        Debug.Log("=== TURN ORDER (PREDICTED) ===");

        List<(string name, float timeToTurn)> order = new List<(string, float)>();

        // Player
        float playerTime =
            (gaugeMax - player.ActionGauge) / player.Stats.FinalSpeed;

        order.Add(("Player", playerTime));

        // Enemies
        foreach (EnemyUnit enemy in enemies)
        {
            float enemyTime =
                (gaugeMax - enemy.ActionGauge) / enemy.Speed;

            order.Add(($"Enemy: {enemy.data.name}", enemyTime));
        }

        // Sort by soonest turn
        order.Sort((a, b) => a.timeToTurn.CompareTo(b.timeToTurn));

        for (int i = 0; i < order.Count; i++)
        {
            Debug.Log($"{i + 1}. {order[i].name}");
        }
    }

}
