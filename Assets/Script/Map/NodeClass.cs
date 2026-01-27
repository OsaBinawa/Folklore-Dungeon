using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapNode
{
    public int Depth;
    public NodeType NodeType;
    public List<MapNode> NextNodes;
    public bool Completed;
    public Button Button;


    public MapNode(int depth, NodeType type)
    {
        Depth = depth;
        NodeType = type;
        NextNodes = new List<MapNode>();
        Completed = false;

    }

    public void Enter()
    {
        Debug.Log($"Entering node: Depth {Depth}, NodeType {NodeType}");
    }
    
    public void Resolve()
    {
        Debug.Log($"Resolved node: Depth {Depth}, NodeType {NodeType}");
    }
} 