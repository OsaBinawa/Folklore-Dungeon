using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleTargeting : MonoBehaviour
{
    private TurnManager turnManager;

    private void Awake()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
    }

    public EnemyUnit[] GetTargets(TargetType type, EnemyUnit mainTarget)
    {
        List<EnemyUnit> enemies = turnManager
            .AVMap
            .Keys
            .OfType<EnemyUnit>()
            .Where(e => e.CanBeTargeted())
            .ToList();
        foreach (var e in enemies)
            e.SetTargeted(false);
        switch (type)
        {
            case TargetType.Single:
                return new EnemyUnit[] { mainTarget };

            case TargetType.All:
                return enemies.ToArray();

            case TargetType.Adjacent3:
                return GetAdjacent(enemies, mainTarget);

            case TargetType.Random:
                return enemies.ToArray();
        }

        return new EnemyUnit[] { mainTarget };
    }

    private EnemyUnit[] GetAdjacent(List<EnemyUnit> enemies, EnemyUnit mainTarget)
    {
        int index = enemies.IndexOf(mainTarget);

        List<EnemyUnit> result = new List<EnemyUnit>();

        if (index > 0)
            result.Add(enemies[index - 1]);

        result.Add(mainTarget);

        if (index < enemies.Count - 1)
            result.Add(enemies[index + 1]);

        return result.ToArray();

    }
}
