using System.Collections.Generic;
using UnityEngine;

public class MapRun
{
    public List<List<MapNode>> DeptNodes = new List<List<MapNode>>();
    public int MaxDepth = 10;

    public void Generate()
    {
        DeptNodes.Clear();
        for (int depth = 0; depth <= MaxDepth; depth++)
        {
            List<MapNode> nodesAtDepth = new List<MapNode>();
            int nodeCount = Random.Range(1, 4);
            for (int i = 0; i < nodeCount; i++)
            {
                NodeType type;
                if (depth == 0)
                    type = NodeType.Start;
                else if (depth == MaxDepth)
                    type = NodeType.Boss;
                else if (depth % 4 == 0)
                    type = NodeType.Rest;
                else
                    type = PickRandomNode();

                MapNode node = new MapNode(depth, type);
                nodesAtDepth.Add(node);
                Debug.Log($"Depth {depth}, Node {i}, Type {type}");
            }
            DeptNodes.Add(nodesAtDepth);
        }

        for (int d = 0; d < DeptNodes.Count - 1; d++)
        {
            foreach (var node in DeptNodes[d])
            {
                foreach (var nextNode in DeptNodes[d + 1])
                {
                    node.NextNodes.Add(nextNode);
                }
            }
        }

        Debug.Log("Map Generated");
    }

    private NodeType PickRandomNode()
    {
        float roll = Random.value;
        if (roll < 0.6f) return NodeType.Combat;
        if (roll < 0.8f) return NodeType.Event;
        return NodeType.Elite;
    }
}