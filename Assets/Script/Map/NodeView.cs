using System;
using UnityEngine;
using UnityEngine.UI;

public class NodeView : MonoBehaviour
{
    protected MapNode node;
    private Action<MapNode> onFinished;
    

    public void Initialize(MapNode node, Action<MapNode> onFinished)
    {
        this.node = node;
        this.onFinished = onFinished;
    }
    public void ResolveNode()
    {
        onFinished?.Invoke(node);
        Destroy(gameObject);
    }
}
