using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {


    public static Pathfinding me;
    public List<Node> temp;
    public bool highlightPathFound = false;
    public GameObject player;

    public ThreadedNode[,] pathNodes;
    bool doubleCheckForWalkable = false;
    void Awake()
    {
        if (me == null)
        {
            me = this;
        }
    }

    // Use this for initialization
   void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); // til að targeta player
    }



    // Update is called once per frame
   

    public List<Vector3> findPath(GameObject me, GameObject targetObj) // finnur path
    {
        List<Vector3> retVal;
        List<Node> tilePath = new List<Node>();
        findPath(me, targetObj, ref tilePath);
        retVal = convertNodeListToVectorList(tilePath);
        return retVal;
    }

    public int getPathCost(GameObject me, GameObject targetObj) // ef það eru waighted tiles sem eru "dýrari" að pathfinda sig á þá tekur þetta þá með
    {
        List<Node> tilePath = new List<Node>();
        findPath(me, targetObj, ref tilePath);


        return tilePath.Count;
    }

    List<Vector3> convertNodeListToVectorList(List<Node> path)
    {
        List<Vector3> retVal = new List<Vector3>();
        foreach (Node t in path)
        {
            retVal.Add(t.transform.position);
        }
        return retVal;
    }
    //"einfalt" a* algorithm til að finna path
    public void findPath(GameObject startPos, GameObject endPos, ref List<Node> store)
    {

        Node startNode; // þar sem pathfindingið byrjar pathið

        if (startPos.GetComponent<Node>() == true)
        {
            startNode = startPos.GetComponent<Node>();
        }
        else
        {
            startNode = CreateNodesFromTilemaps.me.findNearestNode(startPos.transform.position);
        }


        Node endNode; // þar sem pathfindingið er að reyna að komast til

        if (endPos.GetComponent<Node>() == true)
        {
            endNode = endPos.GetComponent<Node>();
        }
        else
        {
            endNode = CreateNodesFromTilemaps.me.findNearestNode(endPos.transform.position);
        }


        if (startNode.walkable == false)
        {
            
        }

        if (endNode.walkable == false)
        {
            endNode = getNearestWalkableTiles(endNode.gridX, endNode.gridY);
        }

        if (startNode == null || endNode == null)
        {
            return;
        }



        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)// finnur út styðstu leiðina frá byrjunar Node til enda Node
        {
            Node node = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == endNode)
            {
                RetracePath(startNode, endNode, ref store);
                return;
            }

            foreach (Node neighbour in node.myNeighbours)
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour) || neighbour == null || node == null)
                {
                    continue;
                }

                int newCostToNeighbour = node.gCost + GetDistance(node, neighbour) + node.getModifier();
                if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, endNode);
                    neighbour.parent = node;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }

    }

    Node getNearestWalkableTiles(int gridX, int gridY) // kallað ef tile sem er leitað að er unwalkable, finnur tile sem er walkable í radius frá unwalkable end tileinu
    {
        Node invalidTile = CreateNodesFromTilemaps.me.Nodes[gridX, gridY].GetComponent<Node>();

        foreach (Node t in invalidTile.myNeighbours)
        {
            if (t.walkable == true)
            {
                return t;
            }
        }

        return null;
    }


    void RetracePath(Node startNode, Node targetNode, ref List<Node> store)
    {
        List<Node> path = new List<Node>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);

            currentNode = currentNode.parent;
        }
        path.Reverse();
        store = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }


  

}
