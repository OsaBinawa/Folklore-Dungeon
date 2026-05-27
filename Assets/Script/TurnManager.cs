using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TurnManager : MonoBehaviour
{
    public const float BASE_AV_SCALE = 1000f;
    public const float DISPLAY_AV_SCALE = 100f;
    private bool enemyActionFinished;
    [SerializeField] private PlayerUnit player;
    [SerializeField] private List<EnemyUnit> enemies = new();
    public ActionUI UI;
    private Dictionary<object, float> avMap = new();
    private Coroutine playerTurnRoutine;
    [SerializeField] private float stuckTimeout = 10f;
    private float stateTimer;
    public IReadOnlyDictionary<object, float> AVMap => avMap;
    public Action OnTimelineUpdated;

    public float turnTime = 5f;
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
    private void Update()
    {
        if (state == TurnState.EnemyTurn)
        {
            stateTimer += Time.deltaTime;

            if (stateTimer >= stuckTimeout)
            {
                Debug.LogWarning($"Enemy turn stuck for {stuckTimeout} seconds. Recovering...");
                ForceRecoverTurn();
            }
        }
        else
        {
            stateTimer = 0f;
        }
    }

    private void ForceRecoverTurn()
    {
        enemyActionFinished = true;

        stateTimer = 0f;

        state = TurnState.Timeline;

        StopAllCoroutines();
        StartCoroutine(TimelineLoop());

        OnTimelineUpdated?.Invoke();
    }
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
    private float GetBaseAV(object unit)
    {
        if (unit is PlayerUnit p)
            return BASE_AV_SCALE / Mathf.Max(10f, p.Stats.FinalSpeed);

        if (unit is EnemyUnit e)
            return BASE_AV_SCALE / Mathf.Max(10f, e.Speed);

        return BASE_AV_SCALE;
    }
    public void ModifyAV(object unit, float amount)
    {
        if (!avMap.ContainsKey(unit)) return;

        avMap[unit] += amount;
        OnTimelineUpdated?.Invoke();
    }
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
                    //StartPlayerTurnTimer();
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
        /*yield return null;
        enemy.Act(player);
        yield return new WaitForSeconds(0.2f);*/
        enemyActionFinished = false;

        enemy.Act(player);

        yield return new WaitUntil(() => enemyActionFinished);
    }

    private void TickAV()
    {
        float tick = Time.deltaTime * 1000f;

        avMap[player] -= tick;

        for (int i = 0; i < enemies.Count; i++)
        {
            avMap[enemies[i]] -= tick;
        }
    }
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
    public void NotifyEnemyActionComplete()
    {
        /*state = TurnState.Timeline;
        OnTimelineUpdated?.Invoke();
        StartCoroutine(TimelineLoop());*/
        enemyActionFinished = true;
    }
    /*private void StartPlayerTurnTimer()
    {
        // Kill any old timer just in case
        if (playerTurnRoutine != null)
        {
            StopCoroutine(playerTurnRoutine);
            playerTurnRoutine = null;
        }

        playerTurnRoutine = StartCoroutine(PlayerTurnCounter());
    }*/
    private IEnumerator PlayerTurnCounter()
    {
        float timeleft = turnTime;
        UI.SetMaxTime(turnTime);

        while (timeleft > 0f && state == TurnState.PlayerTurn)
        {
            timeleft -= Time.deltaTime;
            UI.UpdateTime(timeleft);
            yield return null;
        }

        if (state == TurnState.PlayerTurn)
        {
            Debug.Log("Your Time Expired");
            NotifyPlayerActionComplete();
        }

        playerTurnRoutine = null;
    }
    public void RegisterEnemies(List<EnemyUnit> enemies)
    {
        foreach (var enemy in enemies)
        {
            RegisterEnemy(enemy);
        }
    }
    public int GetDisplayAV(object unit)
    {
        if (!avMap.ContainsKey(unit)) return 0;

        float normalized = avMap[unit] / BASE_AV_SCALE;
        int display = Mathf.CeilToInt(normalized * DISPLAY_AV_SCALE);

        return Mathf.Clamp(display, 0, (int)DISPLAY_AV_SCALE);
    }
}
