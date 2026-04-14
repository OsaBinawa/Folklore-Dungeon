using System.Collections.Generic;
using UnityEngine;

public class MapRun
{
    public List<List<MapNode>> DeptNodes = new List<List<MapNode>>();

    public int MaxDepth = 10;

    
    public int RestInterval = 4;
    public int MinWidth = 2;
    public int MaxWidth = 4;

    public void Generate()
    {
        DeptNodes.Clear();

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
                    
                    type = PickRandomNode();
                }
                else
                {
                    
                    type = rowType;
                }

                MapNode node = new MapNode(depth, type);
                node.index = i;

                nodesAtDepth.Add(node);
                //Debug.Log($"Depth {depth}, Node {i}, Type {type}");
            }

            DeptNodes.Add(nodesAtDepth);
        }

        
        for (int d = 0; d < DeptNodes.Count - 1; d++)
        {
            ConnectRows(DeptNodes[d], DeptNodes[d + 1]);
        }

        Debug.Log("Map Generated");
    }

  
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

    
    private int GetRowWidth(NodeType type)
    {
        if (type == NodeType.Start ||
            type == NodeType.Rest ||
            type == NodeType.Boss)
            return 1;

        return Random.Range(MinWidth, MaxWidth + 1);
    }

    private void ConnectRows(List<MapNode> currentRow, List<MapNode> nextRow)
    {
        int currentWidth = currentRow.Count;
        int nextWidth = nextRow.Count;

        foreach (var node in currentRow)
        {
            node.NextNodes.Clear();

            List<int> validTargets = new List<int>();

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

                for (int offset = -1; offset <= 1; offset++)
                {
                    int target = mappedIndex + offset;
                    if (target >= 0 && target < nextWidth)
                        validTargets.Add(target);
                }
            }

            if (validTargets.Count == 0)
                continue;

            int primaryIndex = validTargets[Random.Range(0, validTargets.Count)];
            MapNode primaryTarget = nextRow[primaryIndex];
            node.NextNodes.Add(primaryTarget);

            if (validTargets.Count > 1 && Random.value < 0.25f)
            {
                int secondaryIndex;

                do
                {
                    secondaryIndex = validTargets[Random.Range(0, validTargets.Count)];
                }
                while (secondaryIndex == primaryIndex);

                node.NextNodes.Add(nextRow[secondaryIndex]);
            }
        }

        foreach (var nextNode in nextRow)
        {
            bool hasParent = false;

            foreach (var currentNode in currentRow)
            {
                if (currentNode.NextNodes.Contains(nextNode))
                {
                    hasParent = true;
                    break;
                }
            }

            if (!hasParent)
            {
                MapNode closestNode = currentRow[0];
                float closestDistance = Mathf.Abs(currentRow[0].index - nextNode.index);

                foreach (var currentNode in currentRow)
                {
                    float distance = Mathf.Abs(currentNode.index - nextNode.index);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestNode = currentNode;
                    }
                }

                closestNode.NextNodes.Add(nextNode);
            }
        }
    }



    private NodeType PickRandomNode()
    {
        float roll = Random.value;

        if (roll < 0.4f) return NodeType.Combat;     
        else if (roll < 0.6f) return NodeType.Event; 
        else if (roll < 0.8f) return NodeType.Reading; 
        else if (roll < 0.9f) return NodeType.Shop;  
        else return NodeType.Elite;                  
    }
}
