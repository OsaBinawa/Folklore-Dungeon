using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public int minEnemies = 1;
    public int maxEnemies = 3;

    public Transform enemyContainer;
    public EnemyUnit[] enemyPrefabs;

    private TurnManager turnManager;

    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        SpawnEnemies();
    }

    void SpawnEnemies()
    {
        int count = Random.Range(minEnemies, maxEnemies + 1);

        List<EnemyUnit> spawnedEnemies = new();

        for (int i = 0; i < count; i++)
        {
            EnemyUnit prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            EnemyUnit enemy = Instantiate(prefab, enemyContainer);
            spawnedEnemies.Add(enemy);
        }

        turnManager.RegisterEnemies(spawnedEnemies);
    }

}
