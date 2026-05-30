using DG.Tweening;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    //public static TutorialPanel Instance;
    [SerializeField] private CanvasGroup _canvasGroup;
    [Header("Intro Tutorial")]
    [SerializeField] private GameObject mapIntroPopup;
    [SerializeField] private GameObject invisibleCloseButton;
    [SerializeField] public GameObject Guide;
    [SerializeField] private Transform popupContainer;
    public MapManager mapManager;

    [Header("Node Tutorials")]
    [SerializeField] private GameObject combatPopup;
    [SerializeField] private GameObject readingPopup;
    [SerializeField] private GameObject eventPopup;
    [SerializeField] private GameObject restPopup;
    [SerializeField] private GameObject shopPopup;
    [SerializeField] private GameObject ElitePopup;
    [SerializeField] private GameObject BossPopup;
    

    [Header("Tween")]
    private Tween fadeTween;
    private Tween blinkTween;
    private NodeView nodes;
    private MapNode pendingNode;

    private void Awake()
    {
        mapManager = FindAnyObjectByType<MapManager>();
    }

    private void Start()
    {
        ShowMapIntroIfNeeded();
        invisibleCloseButton.SetActive(true);
    }
    private void ShowMapIntroIfNeeded()
    {
        string key = "Tutorial_MapIntro";

        if (PlayerPrefs.GetInt(key, 0) == 1)
            return;

        ShowPopup(mapIntroPopup, false);
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();
    }
    public bool TryShowNodeTutorial(MapNode node)
    {
        string key = $"Tutorial_{node.NodeType}";
        invisibleCloseButton.SetActive(true);

        if (PlayerPrefs.GetInt(key, 0) == 1)
            return false;

        pendingNode = node;

        GameObject popup = GetPopup(node.NodeType);
        if (popup == null)
            return false;

        ShowPopup(popup, true);
        Guide.SetActive(false);
        PlayerPrefs.SetInt(key, 1);
        PlayerPrefs.Save();

        return true;
    }
    private void ShowPopup(GameObject popup, bool pauseGame = true)
    {
        popup.SetActive(true);

        if (pauseGame)
            Time.timeScale = 0f;
    }
    
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

            case NodeType.Elite:
                return combatPopup;

            case NodeType.Boss:
                return combatPopup;

            default:
                return null;
        }
    }

    public void CloseAllPanels()
    {
        fadeTween?.Kill();

        fadeTween = _canvasGroup
            .DOFade(0f, 0.25f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                foreach (Transform child in popupContainer)
                {
                    child.gameObject.SetActive(false);
                }

                _canvasGroup.alpha = 1f;
                invisibleCloseButton.SetActive(false);

                Time.timeScale = 1f;
            });
    }
    public bool TryShowNodeTutorial_NoSave(MapNode node)
    {
        if (node == null)
        {
            Debug.LogWarning("Node is null");
            return false;
        }

        GameObject popup = GetPopup(node.NodeType);
        if (popup == null)
            return false;

        invisibleCloseButton.SetActive(true);

        ShowPopup(popup, true);

        return true;
    }
    public void ShowTutorBtn()
    {
        if (mapManager == null)
        {
            Debug.LogWarning("MapManager not found");
            return;
        }

        if (mapManager.CurrentNode == null)
        {
            ShowMapGuide();
            return;
        }

        TryShowNodeTutorial_NoSave(mapManager.CurrentNode);
    }
    public void ClearNode()
    {
        mapManager.ClearCurrentNode();
    }

    public void ShowMapGuide()
    {
        invisibleCloseButton.SetActive(true);

        ShowPopup(mapIntroPopup, true);
    }
    [ContextMenu("Reset Tutorials")]
    public void ResetTutorials()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Tutorials Reset");
    }
}
