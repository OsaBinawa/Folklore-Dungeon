using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ActionUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private TMP_Text turnOrderText;
    [SerializeField] private TMP_Text currentTurnText;
    [SerializeField] private Slider playerTimer;

    [SerializeField] private MainUIManager mainUIManager;

    private Tween blinkTween;

    private void Start()
    {
        mainUIManager = FindFirstObjectByType<MainUIManager>();
        mainUIManager.HideButton();
        Debug.Log("ActionUI Start()");
        turnOrderText.text = "TMP TEXT IS WORKING";
        Hide();
        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>();

        turnManager.OnTimelineUpdated += Refresh;

        Refresh(); 

        BlinkText();
    }

    private void OnEnable()
    {
        turnManager.OnTimelineUpdated += Refresh;
        CombatManager.OnResolveNode += StopBlink;
        TurnManager.OnPlayerTurn += ChangeTurnText;
    }

    private void OnDisable()
    {
        turnManager.OnTimelineUpdated -= Refresh;
        CombatManager.OnResolveNode -= StopBlink;
        TurnManager.OnPlayerTurn -= ChangeTurnText;
    }

    public void Show()
    {
        root.SetActive(true);
        //Refresh();
    }

    public void Hide()
    {
        root.SetActive(false);
    }
    private void OnDestroy()
    {
        if (turnManager != null)
            turnManager.OnTimelineUpdated -= Refresh;

        StopBlink();
    }
    private void Refresh()
    {
        if (turnOrderText == null) return;

        StringBuilder sb = new StringBuilder();

        foreach (var entry in turnManager.AVMap
                     .OrderByDescending(x => x.Value))
        {
            sb.AppendLine(
                // $"{GetUnitName(entry.Key)} : {Mathf.FloorToInt(entry.Value)}" \\ pake kalo butuh av
                $"{GetUnitName(entry.Key)}"
            );
        }

        turnOrderText.text = sb.ToString();
    }

    private void ChangeTurnText(bool playerTurn)
    {
        if (playerTurn)
        {
            currentTurnText.color = Color.black;
            currentTurnText.text = "Current Turn: Player Turn";
        }
        else
        {
            currentTurnText.color = Color.red;
            currentTurnText.text = "Current Turn: Enemy Turn";
        }
    }

    private string GetUnitName(object unit)
    {
        if (unit is PlayerUnit)
            return "Player";

        if (unit is EnemyUnit enemy)
            return enemy.Data.name;

        return "Unknown";
    }

    public void SetMaxTime(float time)
    {
        playerTimer.maxValue = time;
        playerTimer.value = time;
    }

    public void UpdateTime(float timeLeft)
    {
        playerTimer.value = timeLeft;
    }
    public void OpenInventory()
    {
        mainUIManager.openInv();
    }
    
    public void ResetTimer()
    {
        playerTimer.value = playerTimer.maxValue;
    }

    private void BlinkText()
    {
        blinkTween?.Kill();
        currentTurnText.DOFade(0.5f, 1f)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private void StopBlink()
    {
        blinkTween?.Kill();
    }
}
