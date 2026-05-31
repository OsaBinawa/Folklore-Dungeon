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
                return ElitePopup;

            case NodeType.Boss:
                return BossPopup;

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
    [ContextMenu("Print PlayerPrefs Location")]
    public void PrintLocation()
    {
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);
        Debug.Log("Temporary Cache Path: " + Application.temporaryCachePath);

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        Debug.Log("Windows PlayerPrefs are stored in Registry:");
        Debug.Log(@"HKEY_CURRENT_USER\Software\Unity\UnityEditor\YourCompanyName\YourProductName");
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        Debug.Log("macOS PlayerPrefs stored in plist:");
        Debug.Log("~/Library/Preferences/unity.YourCompanyName.YourProductName.plist");
#endif

#if UNITY_ANDROID
        Debug.Log("Android PlayerPrefs stored in:");
        Debug.Log("/data/data/<package_name>/shared_prefs/");
#endif

#if UNITY_IOS
        Debug.Log("iOS PlayerPrefs stored in NSUserDefaults (system managed)");
#endif
    }
}
