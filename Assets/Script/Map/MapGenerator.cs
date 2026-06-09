using System.Collections.Generic;
using System.Text;
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

    public int DifficultyTier { get; private set; } = 0;

    public MapRun(int maxDepth, int restInterval, int minWidth, int maxWidth, int minEliteNodes, int maxEliteNodes)
    {
        MaxDepth = maxDepth;
        RestInterval = restInterval;
        MinWidth = minWidth;
        MaxWidth = maxWidth;
        MinEliteNodes = minEliteNodes;
        MaxEliteNodes = maxEliteNodes;

        StringBuilder log = new StringBuilder();
        log.AppendLine("[MapRun Constructor]");
        log.AppendLine($"MaxDepth      : {MaxDepth}");
        log.AppendLine($"RestInterval  : {RestInterval}");
        log.AppendLine($"MinWidth      : {MinWidth}");
        log.AppendLine($"MaxWidth      : {MaxWidth}");
        log.AppendLine($"MinEliteNodes : {MinEliteNodes}");
        log.AppendLine($"MaxEliteNodes : {MaxEliteNodes}");
        Debug.Log(log.ToString());
    }

    public void IncreaseDifficulty()
    {
        DifficultyTier++;

        StringBuilder log = new StringBuilder();
        log.AppendLine("[IncreaseDifficulty]");
        log.AppendLine($"DifficultyTier : {DifficultyTier}");
        Debug.Log(log.ToString());
    }

    public void Generate()
    {
        StringBuilder log = new StringBuilder();
        log.AppendLine("[Generate]");

        DeptNodes.Clear();
        currentEliteCount = 0;

        log.AppendLine("DeptNodes cleared");
        log.AppendLine("currentEliteCount reset to 0");

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

            log.AppendLine("--------------------------------");
            log.AppendLine($"Depth             : {depth}");
            log.AppendLine($"Row Type          : {rowType}");
            log.AppendLine($"Node Count        : {nodeCount}");
            log.AppendLine($"Row Before Rest   : {rowBeforeRest}");
            log.AppendLine($"Forced Combat Idx : {forcedCombatIndex}");

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

                log.AppendLine($"Node Created  : Depth={node.Depth}, Index={node.index}, Type={node.NodeType}");
            }

            DeptNodes.Add(nodesAtDepth);
        }

        EnsureMinimumEliteNodes();

        for (int d = 0; d < DeptNodes.Count - 1; d++)
        {
            ConnectRows(DeptNodes[d], DeptNodes[d + 1]);
            log.AppendLine($"Connected Row     : Depth {d} -> Depth {d + 1}");
        }

        log.AppendLine("--------------------------------");
        log.AppendLine($"Total Rows         : {DeptNodes.Count}");
        log.AppendLine($"Final Elite Count  : {currentEliteCount}");
        log.AppendLine("Generate finished");

        Debug.Log(log.ToString());
    }

    private NodeType DetermineRowType(int depth)
    {
        NodeType result;

        if (depth == 0)
            result = NodeType.Start;
        else if (depth == MaxDepth)
            result = NodeType.Boss;
        else if (RestInterval > 0 && depth % RestInterval == 0)
            result = NodeType.Rest;
        else
            result = NodeType.Combat;

        StringBuilder log = new StringBuilder();
        log.AppendLine("[DetermineRowType]");
        log.AppendLine($"Input Depth : {depth}");
        log.AppendLine($"MaxDepth    : {MaxDepth}");
        log.AppendLine($"RestInterval: {RestInterval}");
        log.AppendLine($"Result      : {result}");
        Debug.Log(log.ToString());

        return result;
    }

    private int GetRowWidth(NodeType type)
    {
        int width;

        if (type == NodeType.Start ||
            type == NodeType.Rest ||
            type == NodeType.Boss)
        {
            width = 1;
        }
        else
        {
            width = Random.Range(MinWidth, MaxWidth + 1);
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("[GetRowWidth]");
        log.AppendLine($"Node Type : {type}");
        log.AppendLine($"MinWidth  : {MinWidth}");
        log.AppendLine($"MaxWidth  : {MaxWidth}");
        log.AppendLine($"Result    : {width}");
        Debug.Log(log.ToString());

        return width;
    }

    private void ConnectRows(List<MapNode> currentRow, List<MapNode> nextRow)
    {
        StringBuilder log = new StringBuilder();
        log.AppendLine("[ConnectRows]");

        int currentWidth = currentRow.Count;
        int nextWidth = nextRow.Count;

        log.AppendLine($"Current Width : {currentWidth}");
        log.AppendLine($"Next Width    : {nextWidth}");

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

                log.AppendLine($"Node {node.index} Normalized={normalizedIndex}, Mapped={mappedIndex}");
            }

            if (validTargets.Count == 0)
                continue;

            int primaryIndex = validTargets[Random.Range(0, validTargets.Count)];
            MapNode primaryTarget = nextRow[primaryIndex];

            node.NextNodes.Add(primaryTarget);

            log.AppendLine($"Primary   : Depth {node.Depth} Node {node.index} -> Depth {primaryTarget.Depth} Node {primaryTarget.index}");

            if (validTargets.Count > 1 && Random.value < 0.25f)
            {
                int secondaryIndex;

                do
                {
                    secondaryIndex = validTargets[Random.Range(0, validTargets.Count)];
                }
                while (secondaryIndex == primaryIndex);

                node.NextNodes.Add(nextRow[secondaryIndex]);

                log.AppendLine($"Secondary : Depth {node.Depth} Node {node.index} -> Depth {nextRow[secondaryIndex].Depth} Node {nextRow[secondaryIndex].index}");
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

                log.AppendLine($"Parent Fix: Depth {closestNode.Depth} Node {closestNode.index} -> Depth {nextNode.Depth} Node {nextNode.index}");
            }
        }

        Debug.Log(log.ToString());
    }

    private void EnsureMinimumEliteNodes()
    {
        StringBuilder log = new StringBuilder();
        log.AppendLine("[EnsureMinimumEliteNodes]");
        log.AppendLine($"Current Elite Count : {currentEliteCount}");
        log.AppendLine($"Minimum Elite Nodes : {MinEliteNodes}");

        if (currentEliteCount >= MinEliteNodes)
        {
            log.AppendLine("Result              : Minimum elite already fulfilled");
            Debug.Log(log.ToString());
            return;
        }

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
                    validNodes.Add(node);
            }
        }

        log.AppendLine($"Valid Combat Nodes  : {validNodes.Count}");

        while (currentEliteCount < MinEliteNodes && validNodes.Count > 0)
        {
            int index = Random.Range(0, validNodes.Count);

            log.AppendLine($"Converted To Elite  : Depth={validNodes[index].Depth}, Index={validNodes[index].index}");

            validNodes[index].NodeType = NodeType.Elite;
            validNodes.RemoveAt(index);
            currentEliteCount++;
        }

        log.AppendLine($"Final Elite Count   : {currentEliteCount}");

        Debug.Log(log.ToString());
    }

    private NodeType PickRandomNode(int depth)
    {
        float roll = Random.value;

        bool eliteUnlocked = depth > RestInterval;
        bool canSpawnElite =
            eliteUnlocked &&
            currentEliteCount < MaxEliteNodes;

        NodeType result;

        if (roll < 0.3f)
        {
            result = NodeType.Combat;
        }
        else if (roll < 0.6f)
        {
            result = NodeType.Event;
        }
        else if (roll < 0.8f)
        {
            result = NodeType.Reading;
        }
        else if (roll < 0.9f)
        {
            result = NodeType.Shop;
        }
        else
        {
            if (canSpawnElite)
            {
                currentEliteCount++;
                result = NodeType.Elite;
            }
            else
            {
                result = NodeType.Combat;
            }
        }

        StringBuilder log = new StringBuilder();
        log.AppendLine("[PickRandomNode]");
        log.AppendLine($"Depth               : {depth}");
        log.AppendLine($"Roll                : {roll}");
        log.AppendLine($"Elite Unlocked      : {eliteUnlocked}");
        log.AppendLine($"Can Spawn Elite     : {canSpawnElite}");
        log.AppendLine($"Current Elite Count : {currentEliteCount}");
        log.AppendLine($"Result              : {result}");
        Debug.Log(log.ToString());

        return result;
    }
}