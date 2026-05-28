using System.Collections.Generic;
using UnityEngine;

public class MapRun
{
    public List<List<MapNode>> DeptNodes = new List<List<MapNode>>();
    public int MaxDepth = 10;
    public int RestInterval = 4;
    public int MinWidth = 2;
    public int MaxWidth = 4;
    public int MinEliteNodes = 1;
    public int MaxEliteNodes = 3;

    private int currentEliteCount = 0;
    public MapRun(int maxDepth, int restInterval,int minWidth, int maxWidth, int minEliteNodes, int maxEliteNodes)
    {
        MaxDepth = maxDepth;
        RestInterval = restInterval;
        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinEliteNodes = minEliteNodes;
        MaxEliteNodes = maxEliteNodes;
    }
    public int DifficultyTier { get; private set; } = 0;
    public void IncreaseDifficulty()
    {
        DifficultyTier++;
    }
    public void Generate()
    {
        DeptNodes.Clear();
        currentEliteCount = 0;

        for (int depth = 0; depth <= MaxDepth; depth++)
        {
            List<MapNode> nodesAtDepth = new List<MapNode>();

            NodeType rowType = DetermineRowType(depth);
            int nodeCount = GetRowWidth(rowType);

            bool rowBeforeRest =
                depth + 1 <= MaxDepth &&
                DetermineRowType(depth + 1) == NodeType.Rest;

            int forcedCombatIndex = rowBeforeRest
                ? Random.Range(0, nodeCount)
                : -1;

            for (int i = 0; i < nodeCount; i++)
            {
                NodeType type;

                if (rowType == NodeType.Combat)
                {
                    if (i == forcedCombatIndex)
                    {
                        type = NodeType.Combat;
                    }
                    else
                    {
                        type = PickRandomNode(depth);
                    }
                }
                else
                {
                    type = rowType;
                }

                MapNode node = new MapNode(depth, type);
                node.index = i;

                nodesAtDepth.Add(node);
            }

            DeptNodes.Add(nodesAtDepth);
        }
        EnsureMinimumEliteNodes();
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

    private void EnsureMinimumEliteNodes()
    {
        if (currentEliteCount >= MinEliteNodes)
            return;

        List<MapNode> validNodes = new List<MapNode>();

        foreach (var row in DeptNodes)
        {
            foreach (var node in row)
            {
                bool valid =
                    node.NodeType == NodeType.Combat &&
                    node.Depth > RestInterval &&
                    node.Depth < MaxDepth;

                if (valid)
                {
                    validNodes.Add(node);
                }
            }
        }

        while (currentEliteCount < MinEliteNodes && validNodes.Count > 0)
        {
            int index = Random.Range(0, validNodes.Count);

            validNodes[index].NodeType = NodeType.Elite;

            validNodes.RemoveAt(index);

            currentEliteCount++;
        }
    }

    private NodeType PickRandomNode(int depth)
    {
        float roll = Random.value;
        bool eliteUnlocked = depth > RestInterval;
        bool canSpawnElite =
            eliteUnlocked &&
            currentEliteCount < MaxEliteNodes;
        if (roll < 0.3f)
        {
            return NodeType.Combat;
        }
        else if (roll < 0.6f)
        {
            return NodeType.Event;
        }
        else if (roll < 0.8f)
        {
            return NodeType.Reading;
        }
        else if (roll < 0.9f)
        {
            return NodeType.Shop;
        }
        else
        {
            if (canSpawnElite)
            {
                currentEliteCount++;
                return NodeType.Elite;
            }

            return NodeType.Combat;
        }
    }
}
