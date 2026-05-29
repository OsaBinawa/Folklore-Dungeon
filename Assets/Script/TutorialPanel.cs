using DG.Tweening;
using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    //public static TutorialPanel Instance;
    [SerializeField] private CanvasGroup _canvasGroup;
    [Header("Intro Tutorial")]
    [SerializeField] private GameObject mapIntroPopup;
    [SerializeField] private GameObject invisibleCloseButton;

    [Header("Node Tutorials")]
    [SerializeField] private GameObject combatPopup;
    [SerializeField] private GameObject readingPopup;
    [SerializeField] private GameObject eventPopup;
    [SerializeField] private GameObject restPopup;
    [SerializeField] private GameObject shopPopup;

    [Header("Tween")]
    private Tween fadeTween;
    private Tween blinkTween;

    private MapNode pendingNode;

    private void Awake()
    {
        //Instance = this;
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

            default:
                return null;
        }
    }

    public void CloseAllPanels()
    {
        foreach (Transform child in transform)
        {
            fadeTween = _canvasGroup
             .DOFade(0f, 0.25f)
             .SetUpdate(true)
             .OnComplete(() =>
             {
                 _canvasGroup.alpha = 1f;
                 child.gameObject.SetActive(false);
             });
        }

        invisibleCloseButton.SetActive(false);

        Time.timeScale = 1f;
    }

    [ContextMenu("Reset Tutorials")]
    public void ResetTutorials()
    {
        PlayerPrefs.DeleteAll();

        Debug.Log("Tutorials Reset");
    }
}
