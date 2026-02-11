using System.Collections.Generic;
using UnityEngine;

public class MapRun
{
    public List<List<MapNode>> DeptNodes = new List<List<MapNode>>();

    public int MaxDepth = 10;

    // NEW configurable settings
    public int RestInterval = 4;
    public int MinWidth = 3;
    public int MaxWidth = 5;

    public void Generate()
    {
        DeptNodes.Clear();

        // ============================
        // STEP 1: CREATE ROWS
        // ============================
        for (int depth = 0; depth <= MaxDepth; depth++)
        {
            List<MapNode> nodesAtDepth = new List<MapNode>();

            NodeType rowType = DetermineRowType(depth);
            int nodeCount = GetRowWidth(rowType);

            for (int i = 0; i < nodeCount; i++)
            {
                NodeType type;

                if (rowType == NodeType.Combat)
                {
                    // Normal row → random per node
                    type = PickRandomNode();
                }
                else
                {
                    // Start / Rest / Boss → fixed type
                    type = rowType;
                }

                MapNode node = new MapNode(depth, type);
                node.index = i;

                nodesAtDepth.Add(node);
                Debug.Log($"Depth {depth}, Node {i}, Type {type}");
            }

            DeptNodes.Add(nodesAtDepth);
        }

        // ============================
        // STEP 2: CONNECT ROWS (CZN STYLE)
        // ============================
        for (int d = 0; d < DeptNodes.Count - 1; d++)
        {
            ConnectRows(DeptNodes[d], DeptNodes[d + 1]);
        }

        Debug.Log("Map Generated");
    }

    // =====================================
    // ROW TYPE (Boss priority safe)
    // =====================================
    private NodeType DetermineRowType(int depth)
    {
        if (depth == 0)
            return NodeType.Start;

        if (depth == MaxDepth)
            return NodeType.Boss;

        if (RestInterval > 0 && depth % RestInterval == 0)
            return NodeType.Rest;

        return NodeType.Combat;
    }

    // =====================================
    // WIDTH RULES
    // =====================================
    private int GetRowWidth(NodeType type)
    {
        if (type == NodeType.Start ||
            type == NodeType.Rest ||
            type == NodeType.Boss)
            return 1;

        return Random.Range(MinWidth, MaxWidth + 1);
    }

    // =====================================
    // CZN ADJACENT CONNECTION LOGIC
    // =====================================
    private void ConnectRows(List<MapNode> currentRow, List<MapNode> nextRow)
    {
        int currentWidth = currentRow.Count;
        int nextWidth = nextRow.Count;

        foreach (var node in currentRow)
        {
            node.NextNodes.Clear();

            List<int> validTargets = new List<int>();

            // Full tunnel if next row has only 1 node
            if (nextWidth == 1)
            {
                validTargets.Add(0);
            }
            else
            {
                float normalizedIndex = (currentWidth == 1)
                    ? 0.5f
                    : (float)node.index / (currentWidth - 1);

                int mappedIndex = Mathf.RoundToInt(normalizedIndex * (nextWidth - 1));

                // Adjacent only
                for (int offset = -1; offset <= 1; offset++)
                {
                    int target = mappedIndex + offset;

                    if (target >= 0 && target < nextWidth)
                        validTargets.Add(target);
                }
            }

            // 1–2 connections
            int connectionCount = Random.Range(1, Mathf.Min(3, validTargets.Count + 1));

            for (int i = 0; i < connectionCount; i++)
            {
                int randomTargetIndex = validTargets[Random.Range(0, validTargets.Count)];
                MapNode targetNode = nextRow[randomTargetIndex];

                if (!node.NextNodes.Contains(targetNode))
                {
                    node.NextNodes.Add(targetNode);
                }
            }

            // Safety guarantee (at least 1 connection)
            if (node.NextNodes.Count == 0 && validTargets.Count > 0)
            {
                int fallbackIndex = validTargets[Random.Range(0, validTargets.Count)];
                node.NextNodes.Add(nextRow[fallbackIndex]);
            }
        }
    }

    private NodeType PickRandomNode()
    {
        float roll = Random.value;

        if (roll < 0.6f) return NodeType.Combat;
        if (roll < 0.8f) return NodeType.Event;
        return NodeType.Elite;
    }
}
