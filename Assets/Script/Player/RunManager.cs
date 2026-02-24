using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }
    [SerializeField] private PlayerBaseStats startingStats;
    public PlayerRunData Player { get; private set; }
    private void Awake()
    {
        Debug.Log("RUNMANAGER AWAKE");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        StartNewRun();
    }

    public void StartNewRun()
    {
        Player = new PlayerRunData(
            startingStats.maxHP,
            startingStats.baseAttack,
            startingStats.baseSpeed
        );

        if (startingStats.startingEquipment != null)
        {
            foreach (var eq in startingStats.startingEquipment)
                Player.EquippedItems.Add(eq);
        }
        Debug.Log("StartingStats = " + startingStats);
    }
}
