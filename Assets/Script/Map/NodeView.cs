using System;
using UnityEngine;
using UnityEngine.UI;

public class NodeView : MonoBehaviour
{
    protected MapNode node;
    private Action<MapNode> onFinished;
    [SerializeField]private MainUIManager mainUIManager;
    private void Awake()
    {
        mainUIManager = FindFirstObjectByType<MainUIManager>();
        mainUIManager.HideButton();
    }
    public void Initialize(MapNode node, Action<MapNode> onFinished)
    {
        this.node = node;
        this.onFinished = onFinished;
    }
    public void ResolveNode()   
    {
        onFinished?.Invoke(node);
        mainUIManager.ShowButton();
        mainUIManager.RefreshStats();
        Destroy(gameObject);

    }
}
