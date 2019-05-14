using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
public class CreateNodesFromTilemaps : MonoBehaviour {
    public static CreateNodesFromTilemaps me;//singleton to reference the node array for calculating paths
    public Tilemap floor;//the base of the floor, all nodes will be created above a respective floor tiles
    public Tilemap[] unwalkable; //if there is a tile in one of these tilemaps at a location then the tile is unwalkable
    public Tilemap[] weightIncrease; //if there is a tile in one of these tilemaps at a location then the tile will have its weight increased.

    public GameObject nodePrefab;//prefab node
    public int scanStartX = -250, scanStartY = -250, scanFinishX = 250, scanFinishY = 250;//area in the world to check for tiles
    public List<GameObject> unsortedNodes;//all the nodes in the world
    public GameObject[,] Nodes; //sorted 2d array of nodes, may contain null entries if the map is of an odd shape e.g. gaps, can't be serialized so needs to be rebuilt when the game is run
    public int widthOfNodes, heightOfNodes;//can't serialize a 2d array so we store the width/height to make sure that we can rebuild it
    [SerializeField]
    private Tile SpikeTile;
    // Use this for initialization
    void Awake () {
        me = this;
        reconstructNodeArray();
	}

    //Endur býr til node arrayið í runtime því þú getur ekki serializeð 2d array
    void reconstructNodeArray()
    {
        Nodes = new GameObject[widthOfNodes, heightOfNodes];
        foreach(GameObject g in unsortedNodes)
        {
            Node n = g.GetComponent<Node>();
            Nodes[n.gridX, n.gridY] = n.gameObject;
        }
    }

    //fær lista af ganganlegum nodes
    List<GameObject> walkableNodes;
    public GameObject getRandomWalkableNode()
    {
        GameObject retVal;
        if(walkableNodes==null)
        {
            walkableNodes = new List<GameObject>();

            foreach(GameObject g in unsortedNodes)
            {
                if(g.GetComponent<Node>().walkable==true)
                {
                    walkableNodes.Add(g);
                }
            }
        }
        retVal = walkableNodes[Random.Range(0, walkableNodes.Count)];
        return retVal;  
    }
    int startY = 0, startX = 0;
    //finnur næstu node í 10x10 grid
    public Node findNearestNode(Vector3 pos)
    {
        pos -= new Vector3(0.5f,0.5f,0.0f);//svo að nodein spawni í miðjuni á tileinu
        Node retVal = null;
        int xInd = -1, yInd = -1;
        for (int x = startX; x < Nodes.GetLength(0) - 1 && xInd < 0; x++)
        {
            if (Nodes[x, 0] == null)
            {
                continue;
            }
            else
            {
                if (Nodes[x, 0].transform.position.x >= pos.x)
                {
                    xInd = x;
                    break;
                }
            }
        }


        for (int y = startY; y < Nodes.GetLength(1) - 1 && yInd < 0; y++)
        {
            if (Nodes[xInd, y] == null)
            {
                continue;
            }
            else
            {
                if (Nodes[xInd, y].transform.position.y >= pos.y)
                {
                    yInd = y;
                    break;
                }
            }
        }


        if (xInd >= Nodes.GetLength(0) || yInd >= Nodes.GetLength(1) || xInd < 0 || yInd < 0 || Nodes[xInd, yInd] == null)
        {
            GameObject nearest = null;
            float dist = 9999999.0f;
            for (int x = xInd - 5; x < xInd + 5; x++)
            {
                if (x < 0 || x > Nodes.GetLength(0))
                {
                    continue;
                }
                for (int y = yInd - 5; y < yInd + 5; y++)
                {
                    if (y < 0 || y > Nodes.GetLength(1))
                    {
                        continue;
                    }
                    if (Nodes[x, y] != null)
                    {
                        float d = Vector2.Distance(Nodes[x, y].transform.position, pos);
                        if (d < dist)
                        {
                            dist = d;
                            nearest = Nodes[x, y];
                        }
                    }
                }
            }
            retVal = nearest.GetComponent<Node>();
        }
        else
        {
            retVal = Nodes[xInd, yInd].GetComponent<Node>();

        }

        return retVal;
    }
	

    public void createNodeFromTilemaps()//býr til nodes eftir tilemap, gerist áður en leikurinn byrjar fyrir load time
    {
        clearExistingNodes();
        
        unsortedNodes = new List<GameObject>();
        GameObject parent = new GameObject();
        parent.name = "Node parent";
        int gridX = 0, gridY = 0;
        int boundX = 0, boundY = 0;
        bool foundTileOnLastPass = false;

        for (int x = scanStartX; x < scanFinishX; x++)
        {
            for (int y = scanStartY; y < scanFinishY; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tb = floor.GetTile(floor.WorldToCell(pos));

                if (tb == null)
                {
                    if (foundTileOnLastPass == true)
                    {
                        gridY++;
                        if (gridX > boundX)
                        {
                            boundX = gridX;
                        }

                        if (gridY > boundY)
                        {
                            boundY = gridY;
                        }
                    }
                }
                else
                {
                    bool foundObstacle = false;
                    foreach (Tilemap t in unwalkable)
                    {
                        TileBase tb2 = t.GetTile(t.WorldToCell(pos));
                        if (tb2 == null)
                        {

                        }
                        else
                        {
                            foundObstacle = true;
                        }
                    }




                    if (foundObstacle == true)
                    {
                        GameObject node = (GameObject)Instantiate(nodePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.Euler(0, 0, 0));
                        
                        node.GetComponent<SpriteRenderer>().color = Color.red;
                        Node wt = node.GetComponent<Node>();
                        wt.gridX = gridX;
                        wt.gridY = gridY;
                        wt.walkable = false;
                        wt.modifier = 1000;

                        foundTileOnLastPass = true;
                        unsortedNodes.Add(node);
                        node.name = "UNWALKABLE NODE " + gridX.ToString() + " : " + gridY.ToString();
                        node.transform.parent = parent.transform;
                    }
                    else
                    {
                        int weight = 10;
                        foreach (Tilemap t in weightIncrease)
                        {
                            Vector3Int tilePos = new Vector3Int(x, y, 0);
                            TileBase tb2 = t.GetTile(tilePos);

                            if (tb2 == null)
                            {

                            }
                            else if (floor.GetSprite(tilePos) == SpikeTile)
                            {
                                Debug.Log(tilePos + "weighted tile");
                                weight += 150;
                            }
                            else
                            {

                            }
                        }


                        GameObject node = (GameObject)Instantiate(nodePrefab, new Vector3(x + 0.5f, y + 0.5f, 0), Quaternion.Euler(0, 0, 0));
                        Node wt = node.GetComponent<Node>();
                        wt.gridX = gridX;
                        wt.gridY = gridY;
                        foundTileOnLastPass = true; //incrementar index töluna
                        unsortedNodes.Add(node);
                        wt.modifier = weight;

                        node.name = "NODE " + gridX.ToString() + " : " + gridY.ToString();
                        node.transform.parent = parent.transform;

                    }

                    gridY++;
                    if (gridX > boundX)
                    {
                        boundX = gridX;
                    }

                    if (gridY > boundY)
                    {
                        boundY = gridY;
                    }
                }
            }

            if (foundTileOnLastPass == true)
            {
                gridX++;
                gridY = 0;
                foundTileOnLastPass = false;
            }
        }

        widthOfNodes = boundX+1;
        heightOfNodes = boundY+1;

        Nodes = new GameObject[boundX + 1, boundY + 1];
        

        foreach (GameObject g in unsortedNodes)
        {
            Node wt = g.GetComponent<Node>();
            Nodes[wt.gridX, wt.gridY] = g;
        }

        foreach (GameObject g in unsortedNodes)
        {
            Node wt = g.GetComponent<Node>();
            List<Node> neigbours = new List<Node>();
            foreach (GameObject g2 in unsortedNodes)
            {
                if (g2 == g)
                {
                    continue;
                }

                float d = Vector2.Distance(g.transform.position, g2.transform.position);
                if (d < 1.1f)
                {
                    neigbours.Add(g2.GetComponent<Node>());
                }
            }
            wt.myNeighbours = neigbours;

        }

    }

    
    static void GeneratePathfinding()
    {
        if (FindObjectOfType<CreateNodesFromTilemaps>() == null)
        {
            Debug.LogError("ERROR! An instance of the script CreateNodesFromTilemaps is needed to generate pathfinding from tilemaps");
            return;
        }

        FindObjectOfType<CreateNodesFromTilemaps>().createNodeFromTilemaps();
    }


    void clearExistingNodes()//clears any existing nodes when the pathfinding is generated
    {
       

        foreach(GameObject g in unsortedNodes)
        {
            DestroyImmediate(g);
        }
        Nodes = new GameObject[0,0];
        unsortedNodes.Clear();
    }


}
