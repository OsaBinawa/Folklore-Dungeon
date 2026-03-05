using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public Transform MapContainer;
    public Transform ContentRoot;
    public Transform Root;
    public Transform NodeViewContainer;
    [SerializeField] private List<NodeSpriteEntry> NodeSprites;
    [SerializeField] private GameObject NodeButtonPrefabs;
    [SerializeField] private MapRun currentRun;
    [SerializeField] private MapNode currentNode;
    [SerializeField] private GameObject CombatPrefab;
    [SerializeField] private GameObject EventPrefab;
    [SerializeField] private GameObject RestPrefab;
    [SerializeField] private GameObject BossPrefab;
    [SerializeField] private GameObject ReadPrefab;
    [SerializeField] private GameObject ShopPrefab;
    [SerializeField] private LineDrawer lineDrawer;
    [SerializeField] private Scrollbar scroll;

    private int currentUnlockedDepth = 1;

    private GameObject GetPrefabForNode(NodeType type)
    {
        return type switch
        {
            NodeType.Combat => CombatPrefab,
            NodeType.Event => EventPrefab,
            NodeType.Rest => RestPrefab,
            NodeType.Boss => BossPrefab,
            NodeType.Shop => ShopPrefab,
            NodeType.Reading => ReadPrefab,
            _ => CombatPrefab
        };
    }

    private void Start()
    {
        currentRun = new MapRun();
        currentRun.Generate();
        DisplayAllDepths();
        AutoCompleteStartNode();
        //InitializeMap();
        //DisplayDepth(0);

    }
    private void DisplayAllDepths()
    {
        foreach (Transform t in MapContainer)
            Destroy(t.gameObject);

        for (int depth = 0; depth < currentRun.DeptNodes.Count; depth++)
        {
            List<MapNode> nodes = currentRun.DeptNodes[depth];

            float spacingX = 200f;
            float startX = -((nodes.Count - 1) * spacingX) / 2f;
            float y = -depth * 150f;

            for (int i = 0; i < nodes.Count; i++)
            {
                MapNode localNode = nodes[i];

                GameObject btnObj = Instantiate(NodeButtonPrefabs, MapContainer);

                RectTransform rt = btnObj.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startX + i * spacingX, y);

                Button btn = btnObj.GetComponent<Button>();
                localNode.Button = btn;

                btn.onClick.AddListener(() => EnterNode(localNode));

                
                btn.interactable = false;

                TMP_Text txt = btnObj.GetComponentInChildren<TMP_Text>();
                if (txt != null)
                    txt.text = localNode.NodeType.ToString();
                Image img = btnObj.GetComponent<Image>();
                if (img != null)
                    img.sprite = GetSpriteForNode(localNode.NodeType);

            }
        }
        lineDrawer.Draw(currentRun);
        AdjustContentHeight();
    }


    private void AutoCompleteStartNode()
    {
        var startNode = currentRun.DeptNodes[0][0];

        startNode.Completed = true;

        if (startNode.Button != null)
            startNode.Button.interactable = false;

        
        if (currentRun.DeptNodes.Count > 1)
        {
            foreach (var node in currentRun.DeptNodes[1])
            {
                if (node.Button != null)
                    node.Button.interactable = true;
            }
        }
    }



    public void EnterNode(MapNode node)
    {
        currentNode = node;
        node.Enter();

        MapContainer.gameObject.SetActive(false);
        Root.gameObject.SetActive(false);
        NodeViewContainer.gameObject.SetActive(true);

        GameObject prefab = GetPrefabForNode(node.NodeType);
        GameObject viewObj = Instantiate(prefab, NodeViewContainer);

        NodeView view = viewObj.GetComponent<NodeView>();
        view.Initialize(node, OnNodeFinished);
    }

    private void OnNodeFinished(MapNode node)
    {
        node.Resolve();
        node.Completed = true;

        NodeViewContainer.gameObject.SetActive(false);
        MapContainer.gameObject.SetActive(true);
        Root.gameObject.SetActive(true);

        // Disable all buttons first
        foreach (var depth in currentRun.DeptNodes)
        {
            foreach (var n in depth)
            {
                if (n.Button != null)
                    n.Button.interactable = false;
            }
        }

        // Enable only reachable next nodes
        foreach (var next in node.NextNodes)
        {   
            if (next.Button != null && !next.Completed)
                next.Button.interactable = true;
        }

        if (node.NodeType == NodeType.Boss)
            Debug.Log("Run complete!");
    }


    private void UpdateButtonLocks()
    {
        for (int depth = 0; depth < currentRun.DeptNodes.Count; depth++)
        {
            foreach (var node in currentRun.DeptNodes[depth])
            {
                if (node.Button == null)
                    continue;

                bool clickable =
                    depth == currentUnlockedDepth &&
                    !node.Completed;

                node.Button.interactable = clickable;
            }
        }
    }
    public void UpdateNodeInteractivity(MapNode currentNode)
    {
        // 1. Disable ALL nodes first
        foreach (var depth in currentRun.DeptNodes)
        {
            foreach (var node in depth)
            {
                if (node.Button != null)
                    node.Button.interactable = false;
            }
        }

        // 2. Enable only reachable next nodes
        foreach (var next in currentNode.NextNodes)
        {
            if (next.Button != null)
                next.Button.interactable = true;
        }
    }

    public void InitializeMap()
    {
        foreach (var depth in currentRun.DeptNodes)
        {
            foreach (var node in depth)
            {
                node.Button.interactable = false;
            }
        }

        var startNode = currentRun.DeptNodes[0][0];
        startNode.Button.interactable = true;
    }

    private void AdjustContentHeight()
    {
        RectTransform content = ContentRoot.GetComponent<RectTransform>();
        RectTransform viewport = content.parent.GetComponent<RectTransform>();
        //content.anchoredPosition -= new Vector2(0, 5000f);
        float depthSpacing = 150f;
        float totalHeight = currentRun.DeptNodes.Count * depthSpacing;

        float minHeight = viewport.rect.height + -10f;

        float finalHeight = Mathf.Max(totalHeight + 500f, minHeight);

        content.sizeDelta = new Vector2(content.sizeDelta.x, finalHeight);

        //scroll.size = .5f;
    }


    private void ResolveNode()
    {
        currentNode.Resolve();
        currentNode.Button.interactable = false;

        if (currentNode.NodeType == NodeType.Boss)
        {
            Debug.Log("Boss defeated! Run complete.");
            return;
        }

        int nextDepth = currentNode.Depth + 1;

        foreach (var node in currentRun.DeptNodes[nextDepth])
        {
            node.Button.interactable = true;
        }
    }
    private Sprite GetSpriteForNode(NodeType type)
{
    foreach (var entry in NodeSprites)
    {
        if (entry.Type == type)
            return entry.Sprite;
    }

    return null;
}

}
[System.Serializable]
public class NodeSpriteEntry
{
    public NodeType Type;
    public Sprite Sprite;
}

