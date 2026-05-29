using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public static TutorialPanel Instance;

    [Header("Intro Tutorial")]
    [SerializeField] private GameObject mapIntroPopup;

    [Header("Node Tutorials")]
    [SerializeField] private GameObject combatPopup;
    [SerializeField] private GameObject readingPopup;
    [SerializeField] private GameObject eventPopup;
    [SerializeField] private GameObject restPopup;
    [SerializeField] private GameObject shopPopup;
    private MapNode pendingNode;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowMapIntroIfNeeded();
    }

    // =========================
    // FIRST MAP INTRO
    // =========================
    private void ShowMapIntroIfNeeded()
    {
        string key = "Tutorial_MapIntro";

        if (PlayerPrefs.GetInt(key, 0) == 1)
            return;

        ShowPopup(mapIntroPopup);

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }

    // =========================
    // NODE CLICK
    // =========================
    public void OnNodeClicked(MapNode node)
    {
        bool showingTutorial = TryShowNodeTutorial(node);

        if (!showingTutorial)
        {
            EnterNode(node);
        }
    }

    // =========================
    // SHOW NODE TUTORIAL
    // =========================
    private bool TryShowNodeTutorial(MapNode node)
    {
        string key = $"Tutorial_{node.NodeType}";

        // already shown
        if (PlayerPrefs.GetInt(key, 0) == 1)
            return false;

        pendingNode = node;

        GameObject popup = GetPopup(node.NodeType);

        if (popup == null)
            return false;

        ShowPopup(popup);

        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        return true;
    }

    // =========================
    // SHOW POPUP
    // =========================
    private void ShowPopup(GameObject popup)
    {
        popup.SetActive(true);

        Time.timeScale = 0f;
    }

    // =========================
    // CLOSE POPUP BUTTON
    // =========================
    public void CloseTutorial(GameObject popup)
    {
        popup.SetActive(false);

        Time.timeScale = 1f;

        if (pendingNode != null)
        {
            EnterNode(pendingNode);
            pendingNode = null;
        }
    }

    // =========================
    // GET POPUP
    // =========================
    private GameObject GetPopup(NodeType type)
    {
        switch (type)
        {
            case NodeType.Combat:
                return combatPopup;

            case NodeType.Reading:
                return readingPopup;

            case NodeType.Event:
                return eventPopup;

            case NodeType.Rest:
                return restPopup;

            case NodeType.Shop:
                return shopPopup;

            default:
                return null;
        }
    }

    // =========================
    // ENTER NODE
    // =========================
    private void EnterNode(MapNode node)
    {
        Debug.Log($"Entering node: {node.NodeType}");

        // SceneManager.LoadScene(...)
        // or your node logic
    }

    // =========================
    // DEBUG RESET
    // =========================
    [ContextMenu("Reset Tutorials")]
    public void ResetTutorials()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Tutorials Reset");
    }
}
