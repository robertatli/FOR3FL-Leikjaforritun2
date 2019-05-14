using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int gCost;
    public int hCost;
    public int gridX, gridY;
    public bool walkable = true;
    public List<Node> myNeighbours;
    public Node parent;
    public int modifier = 10;
    public int tempModifiers = 0;
    public bool isOptimised = false;
    public int getModifier()
    {
        return modifier + tempModifiers;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}


public class ThreadedNode
{

    public int gCost;
    public int hCost;
    public int gridX, gridY;
    public bool walkable = true;
    public List<ThreadedNode> myNeighbours;
    public ThreadedNode parent;
    public Vector3 worldPos;
    public int modifier = 10;
    public int tempModifiers = 0;
    public int getModifier()
    {
        return modifier + tempModifiers;
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
