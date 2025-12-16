using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public const float BASE_AV_SCALE = 10000f;
    public const float DISPLAY_AV_SCALE = 1000f;

    [SerializeField] private PlayerUnit player;
    [SerializeField] private List<EnemyUnit> enemies = new();
    public ActionUI UI;
    private Dictionary<object, float> avMap = new();
    public IReadOnlyDictionary<object, float> AVMap => avMap;

    public Action OnTimelineUpdated;

    private enum TurnState
    {
        Timeline,
        PlayerTurn,
        EnemyTurn
    }

    private TurnState state = TurnState.Timeline;

    private void Start()
    {
        RegisterPlayer();
        StartCoroutine(TimelineLoop());
        OnTimelineUpdated?.Invoke();
    }

    // ---------------- REGISTRATION ----------------

    private void RegisterPlayer()
    {
        avMap[player] = GetBaseAV(player);
    }

    public void RegisterEnemy(EnemyUnit enemy)
    {
        if (enemies.Contains(enemy)) return;

        enemies.Add(enemy);
        avMap[enemy] = GetBaseAV(enemy);
        OnTimelineUpdated?.Invoke();
    }

    public void UnregisterEnemy(EnemyUnit enemy)
    {
        enemies.Remove(enemy);
        avMap.Remove(enemy);
        OnTimelineUpdated?.Invoke();
    }

    // ---------------- AV CORE ----------------

    private float GetBaseAV(object unit)
    {
        if (unit is PlayerUnit p)
            return BASE_AV_SCALE / p.Stats.FinalSpeed;

        if (unit is EnemyUnit e)
            return BASE_AV_SCALE / e.Speed;

        return BASE_AV_SCALE;
    }

    public void ModifyAV(object unit, float amount)
    {
        if (!avMap.ContainsKey(unit)) return;

        avMap[unit] += amount;
        OnTimelineUpdated?.Invoke();
    }

    // ---------------- FLOW ----------------

    public void NotifyPlayerActionComplete()
    {
        state = TurnState.Timeline;
        OnTimelineUpdated?.Invoke();
        StartCoroutine(TimelineLoop());
    }

    private IEnumerator TimelineLoop()
    {
        while (state == TurnState.Timeline)
        {
            TickAV();

            object next = GetNextReadyUnit();
            if (next != null)
            {
                avMap[next] += GetBaseAV(next); // reset after turn

                if (ReferenceEquals(next, player))
                {
                    state = TurnState.PlayerTurn;
                    UI.Show();
                    yield break;
                }
                else
                {
                    EnemyUnit enemy = next as EnemyUnit;
                    state = TurnState.EnemyTurn;
                    UI.Hide();
                    yield return StartCoroutine(EnemyTurn(enemy));
                    state = TurnState.Timeline;
                }

                OnTimelineUpdated?.Invoke();
            }

            yield return null;
        }
    }

    private IEnumerator EnemyTurn(EnemyUnit enemy)
    {
        yield return null;
        enemy.Act(player);
        yield return new WaitForSeconds(0.2f);
    }

    // ---------------- TICK ----------------

    private void TickAV()
    {
        float tick = Time.deltaTime * 1000f;

        avMap[player] -= tick;

        foreach (EnemyUnit enemy in enemies)
            avMap[enemy] -= tick;
    }

    // ---------------- SELECTION ----------------

    private object GetNextReadyUnit()
    {
        object best = null;
        float lowestAV = float.MaxValue;

        foreach (var pair in avMap)
        {
            if (pair.Value <= 0 && pair.Value < lowestAV)
            {
                lowestAV = pair.Value;
                best = pair.Key;
            }
        }

        return best;
    }

    // ---------------- DEBUG DISPLAY ----------------

    public int GetDisplayAV(object unit)
    {
        if (!avMap.ContainsKey(unit)) return 0;

        float normalized = avMap[unit] / BASE_AV_SCALE;
        int display = Mathf.CeilToInt(normalized * DISPLAY_AV_SCALE);

        return Mathf.Clamp(display, 0, (int)DISPLAY_AV_SCALE);
    }
}
