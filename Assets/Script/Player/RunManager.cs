using System.Collections.Generic;
using UnityEngine;

public class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }
    [SerializeField] private PlayerBaseStats startingStats;
    [SerializeField] public List<BuffSO> AllAvailableBuff =  new List<BuffSO>();
    public PlayerRunData Player { get; private set; }
    [SerializeField] private AudioClip inGameBGM;
    private void Awake()
    {
        Debug.Log("RUNMANAGER AWAKE");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        PlayGameBGM();

        Instance = this;
        //DontDestroyOnLoad(gameObject);

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

    public void PlayGameBGM()
    {
        SoundManager.Instance.PlayBGM(inGameBGM);
    }
}
