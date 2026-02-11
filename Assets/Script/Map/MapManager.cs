using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public Transform MapContainer;
    public Transform NodeViewContainer;
    [SerializeField] private GameObject NodeButtonPrefabs;
    [SerializeField] private MapRun currentRun;
    [SerializeField] private MapNode currentNode;
    [SerializeField] private GameObject CombatPrefab;
    [SerializeField] private GameObject EventPrefab;
    [SerializeField] private GameObject RestPrefab;
    [SerializeField] private GameObject BossPrefab;
    private int currentUnlockedDepth = 1;

    private GameObject GetPrefabForNode(NodeType type)
    {
        return type switch
        {
            NodeType.Combat => CombatPrefab,
            NodeType.Event => EventPrefab,
            NodeType.Rest => RestPrefab,
            NodeType.Boss => BossPrefab,
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
            }
        }
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

}
