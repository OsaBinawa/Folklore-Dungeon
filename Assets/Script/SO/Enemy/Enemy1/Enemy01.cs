using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : EnemyUnit
{
    [Header("Attacks")]
    [SerializeField] private List<AttackBase> attacks;

    private PlayerUnit player;

    private void Awake()
    {
        player = FindFirstObjectByType<PlayerUnit>();
    }

    // Called from Animation Event
    public void Basic()
    {
        if (attacks == null || attacks.Count == 0)
            return;

        ExecuteAttack(attacks[0], player);
        Debug.Log("test");
    }
}
