using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;

public class ActionUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private TMP_Text turnOrderText;


    private void Start()
    {
        Debug.Log("ActionUI Start()");
        turnOrderText.text = "TMP TEXT IS WORKING";
        Hide();
        if (turnManager == null)
            turnManager = FindFirstObjectByType<TurnManager>();

        turnManager.OnTimelineUpdated += Refresh;

        Refresh(); // initial draw
    }

    private void OnEnable()
    {
        turnManager.OnTimelineUpdated += Refresh;
    }

    private void OnDisable()
    {
        turnManager.OnTimelineUpdated -= Refresh;
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
    }
    private void Refresh()
    {
        if (turnOrderText == null) return;

        StringBuilder sb = new StringBuilder();

        foreach (var entry in turnManager.AVMap
                     .OrderByDescending(x => x.Value))
        {
            sb.AppendLine(
                $"{GetUnitName(entry.Key)} : {Mathf.FloorToInt(entry.Value)}"
            );
        }

        turnOrderText.text = sb.ToString();
    }

    private string GetUnitName(object unit)
    {
        if (unit is PlayerUnit)
            return "Player";

        if (unit is EnemyUnit enemy)
            return enemy.Data.name;

        return "Unknown";
    }
}
