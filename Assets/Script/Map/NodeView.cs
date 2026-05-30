using System;
using UnityEngine;
using UnityEngine.UI;

public class NodeView : MonoBehaviour
{
    protected MapNode node;
    private Action<MapNode> onFinished;
    [SerializeField]private MainUIManager mainUIManager;
    [SerializeField]private TutorialPanel panel;

    private void Awake()
    {
        mainUIManager = FindFirstObjectByType<MainUIManager>();
        mainUIManager.HideButton();
        panel = FindFirstObjectByType<TutorialPanel>();
        panel.Guide.SetActive(false);
        
    }
    public void Initialize(MapNode node, Action<MapNode> onFinished)
    {
        this.node = node;
        this.onFinished = onFinished;
        panel.TryShowNodeTutorial(node);
    }
    public void ResolveNode()   
    {
        onFinished?.Invoke(node);
        mainUIManager.ShowButton();
        mainUIManager.RefreshStats();
        panel.Guide.SetActive(true);
        panel.ClearNode();
        Destroy(gameObject);

    }
}
