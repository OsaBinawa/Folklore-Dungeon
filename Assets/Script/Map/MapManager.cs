using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapManager : MonoBehaviour
{
    public Transform MapContainer;
    public GameObject NodeButtonPrefabs;
    [SerializeField] private MapRun currentRun;

    [SerializeField] private MapNode currentNode;

    private void Start()
    {
        currentRun = new MapRun();
        currentRun.Generate();
        DisplayAllDepths();
        //DisplayDepth(0);
    }

    private void DisplayDepth(int depth)
    {
        foreach (Transform t in MapContainer) Destroy(t.gameObject);

        List<MapNode> nodes = currentRun.DeptNodes[depth];

        float spacingX = 200f; // horizontal distance between nodes
        float startX = -((nodes.Count - 1) * spacingX) / 2; // center nodes horizontally
        float y = -depth * 150f; // vertical spacing between depths

        for (int i = 0; i < nodes.Count; i++)
        {
            MapNode localNode = nodes[i];
            GameObject btnObj = Instantiate(NodeButtonPrefabs, MapContainer);

            // Set anchored position
            RectTransform rt = btnObj.GetComponent<RectTransform>();
            rt.anchoredPosition = new Vector2(startX + i * spacingX, y);

            // Add button listener
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => EnterNode(localNode));
        }
    }

    private void DisplayAllDepths()
    {
        foreach (Transform t in MapContainer) Destroy(t.gameObject);

        for (int depth = 0; depth < currentRun.DeptNodes.Count; depth++)
        {
            List<MapNode> nodes = currentRun.DeptNodes[depth];

            float spacingX = 200f;
            float startX = -((nodes.Count - 1) * spacingX) / 2;
            float y = -depth * 150f; // vertical spacing per depth

            for (int i = 0; i < nodes.Count; i++)
            {
                MapNode localNode = nodes[i];
                GameObject btnObj = Instantiate(NodeButtonPrefabs, MapContainer);

                RectTransform rt = btnObj.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startX + i * spacingX, y);

                Button btn = btnObj.GetComponent<Button>();
                btn.onClick.AddListener(() => EnterNode(localNode));

                // Optionally: disable buttons that are not yet unlocked
                btn.interactable = (depth == 0); // only depth 0 interactable
            }
        }
    }


    public void EnterNode(MapNode node)
    {
        currentNode = node;
        node.Enter();
        MapContainer.gameObject.SetActive(false);

        Debug.Log($"Simulate resolving {node.NodeType}");
        ResolveNode();
    }

    private void ResolveNode()
    {
        currentNode.Resolve();

        if (currentNode.NodeType == NodeType.Boss)
        {
            Debug.Log("Boss defeated! Run complete.");
        }
        else
        {
            // Show next depth
            int nextDepth = currentNode.Depth + 1;
            MapContainer.gameObject.SetActive(true);
            DisplayDepth(nextDepth);
        }
    }
}
