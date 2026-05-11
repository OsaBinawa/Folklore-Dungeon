using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatManager : MonoBehaviour
{
    [SerializeField] private NodeView nodeView;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private PlayerUnit playerUnit;
    [SerializeField] private Inventory inventory;
    [SerializeField] private TMP_Text text1;

    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;

    [SerializeField] private AudioClip CombatBGM;

    private bool gameEnd = false;

    private void OnEnable()
    {
        PlayerUnit.OnPlayerDied += CheckLose;
        EnemyUnit.OnEnemyDied += CheckWin;
    }

    private void OnDisable()
    {
        PlayerUnit.OnPlayerDied -= CheckLose;
        EnemyUnit.OnEnemyDied -= CheckWin;
    }
   
    void Start()
    {
        turnManager = FindFirstObjectByType<TurnManager>();
        playerUnit = FindFirstObjectByType<PlayerUnit>();
        inventory = FindFirstObjectByType<Inventory>();
        SoundManager.Instance.PlayBGM(CombatBGM);
    }

    
    void Update()
    {
        //CheckWin();
    }

    private void CheckWin()
    {
        if (gameEnd) return;

        StartCoroutine(CheckWinDelayed());
    }

    private System.Collections.IEnumerator CheckWinDelayed()
    {
        yield return null; // wait 1 frame

        bool hasEnemy = turnManager.AVMap.Keys.OfType<EnemyUnit>().Any();

        if (!hasEnemy)
        {
            gameEnd = true;

            if (winPanel != null)
                winPanel.SetActive(true);

            int randomAmount = Random.Range(10, 20);
            inventory.money += randomAmount;

            Debug.Log("You Get " + randomAmount + " money");

            if (text1 != null)
            {
                text1.gameObject.SetActive(true);
                text1.text = "You Get " + randomAmount + " money";
            }
        }
    }

    private void CheckLose()
    {
        if (gameEnd) return;

        gameEnd = true;
        losePanel.SetActive(true);
        Debug.Log("YOU LOSE");
    }

    public void ResolveCombat()
    {
        nodeView.ResolveNode();
        RunManager.Instance.PlayGameBGM();
    }

    public void GoToScene(string Scene)
    {
        SceneManager.LoadScene(Scene);
    }
}
