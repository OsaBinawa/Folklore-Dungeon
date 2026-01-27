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
        var startNodes = currentRun.DeptNodes[0];

        foreach (var node in startNodes)
        {
            node.Completed = true;

            if (node.Button != null)
                node.Button.interactable = false;
        }

        if (currentRun.DeptNodes.Count > 1)
        {
            foreach (var node in currentRun.DeptNodes[1])
                node.Button.interactable = true;
        }
        currentUnlockedDepth = 1;
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
        //node.Resolve();
        foreach (var n in currentRun.DeptNodes[node.Depth])
        {
            n.Completed = true;
        }
        if (node.Depth + 1 > currentUnlockedDepth)
            currentUnlockedDepth = node.Depth + 1;

        NodeViewContainer.gameObject.SetActive(false);
        MapContainer.gameObject.SetActive(true);
        UpdateButtonLocks();
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
