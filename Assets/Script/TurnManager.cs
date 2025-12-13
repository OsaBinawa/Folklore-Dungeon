using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public const float TURN_THRESHOLD = 10000f;

    [SerializeField] private PlayerUnit player;
    [SerializeField] private List<EnemyUnit> enemies = new();

    // 🔥 AV lives ONLY here
    private Dictionary<object, float> avMap = new();

    private void Start()
    {
        avMap[player] = 0f;
        StartCoroutine(TimelineLoop());
    }

    // =========================
    // REGISTRATION
    // =========================

    public void RegisterEnemy(EnemyUnit enemy)
    {
        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
            avMap[enemy] = 0f;
        }
    }

    public void UnregisterEnemy(EnemyUnit enemy)
    {
        enemies.Remove(enemy);
        avMap.Remove(enemy);
    }

    // =========================
    // PLAYER CALLBACK
    // =========================

    public void NotifyPlayerActionComplete()
    {
        StartCoroutine(TimelineLoop());
    }

    // =========================
    // TIMELINE LOOP
    // =========================

    private IEnumerator TimelineLoop()
    {
        while (true)
        {
            TickAV();
            LogAVs();

            object next = GetHighestReadyUnit();
            if (next != null)
            {
                avMap[next] -= TURN_THRESHOLD;

                if (ReferenceEquals(next, player))
                {
                    Debug.Log(">>> PLAYER TURN <<<");
                    yield break; // wait for UI input
                }
                else
                {
                    EnemyUnit enemy = next as EnemyUnit;
                    Debug.Log($">>> ENEMY TURN: {enemy.EnemyData.name} <<<");

                    yield return StartCoroutine(EnemyTurn(enemy));
                    continue;
                }
            }

            yield return null;
        }
    }

    // =========================
    // ENEMY TURN
    // =========================

    private IEnumerator EnemyTurn(EnemyUnit enemy)
    {
        yield return null;

        player.TakeDamage(enemy.Damage);
        Debug.Log($"{enemy.EnemyData.name} attacks player");

        yield return new WaitForSeconds(0.2f);
    }

    // =========================
    // AV CORE
    // =========================

    private void TickAV()
    {
        float tick = 1f;

        avMap[player] += player.Stats.FinalSpeed * tick;

        foreach (EnemyUnit enemy in enemies)
            avMap[enemy] += enemy.Speed * tick;
    }

    public void DelayUnit(object unit, float amount)
    {
        if (avMap.ContainsKey(unit))
            avMap[unit] -= amount;
    }

    // =========================
    // HSR-STYLE READY SELECTION
    // =========================

    private object GetHighestReadyUnit()
    {
        List<(object unit, float av, int speed)> ready = new();

        if (avMap[player] >= TURN_THRESHOLD)
        {
            ready.Add((
                player,
                avMap[player],
                player.Stats.FinalSpeed
            ));
        }

        foreach (EnemyUnit enemy in enemies)
        {
            if (avMap[enemy] >= TURN_THRESHOLD)
            {
                ready.Add((
                    enemy,
                    avMap[enemy],
                    enemy.Speed
                ));
            }
        }

        if (ready.Count == 0)
            return null;

        ready.Sort((a, b) =>
        {
            if (!Mathf.Approximately(a.av, b.av))
                return b.av.CompareTo(a.av);   // higher AV first

            return b.speed.CompareTo(a.speed); // tie → higher speed
        });

        return ready[0].unit;
    }

    // =========================
    // DEBUG
    // =========================

    private void LogAVs()
    {
        string log = $"[AV] Player: {avMap[player]:F0}";

        foreach (EnemyUnit enemy in enemies)
            log += $" | {enemy.EnemyData.name}: {avMap[enemy]:F0}";

        Debug.Log(log);
    }
}
