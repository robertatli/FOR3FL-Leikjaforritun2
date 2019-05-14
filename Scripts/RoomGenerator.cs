using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

public class RoomGenerator : MonoBehaviour
{
    public GameObject spiketrap;
    public GameObject Enemy1;
    public GameObject exitLadder;
    public GameObject CNFT;

    [SerializeField]// þetta er fyrir veggja generation, fyrir tilemap.
    private Tile SpikeTile;
    [SerializeField]
    private Tile groundTile;
    [SerializeField]
    private Tile pitTile;
    [SerializeField]
    private Tile rightWallTile;
    [SerializeField]
    private Tile leftWallTile;
    [SerializeField]
    private Tile topWallTile;
    [SerializeField]
    private Tile slabWallTile;
    [SerializeField]
    private Tile slabCornerRightTile;
    [SerializeField]
    private Tile slabCornerLeftTile;
    [SerializeField]
    private Tile slabWallTileRight;
    [SerializeField]
    private Tile slabWallTileLeft;
    [SerializeField]
    private Tile CornerRightTile;
    [SerializeField]
    private Tile CornerLeftTile;
    [SerializeField]
    private Tile botWallTile;
    [SerializeField]
    private Tile leftWallPit;
    [SerializeField]
    private Tile rightWallPit;
    [SerializeField]
    private Tile pitCornerRight;
    [SerializeField]
    private Tile pitCornerLeft;
    [SerializeField]
    private Tilemap groundMap; // tilemap sem er hægt að ganga á
    [SerializeField]
    private Tilemap pitMap; // tilemap sem er bara fyrir asthetics
    [SerializeField]
    private Tilemap wallMap; // tilemap sem er fyrir veggi
    [SerializeField]
    private Tilemap bottomSlabMap;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private int deviationRate = 10; // hversu miklar líkur það eru til að beygja í aðra átt/ búa til herbergi
    [SerializeField]
    private int roomRate = 15; // býr til herbergi ef ekkert herbergi er búið að vera búið til eftir x tilraunir
    [SerializeField]
    private int maxRouteLength; // svo að þetta keyri ekki endalaust
    [SerializeField]
    private int maxRoutes = 20; // svo að þetta keyri ekki endalaust

    System.Random rand = new System.Random();

    private int routeCount = 0;

    List<Vector3Int> groundTileList = new List<Vector3Int>(); // Holds all ground tiles for reference.

    void Awake()
    {
        //FindObjectOfType<CreateNodesFromTilemap>().generateNodes();
    }

    private void Start()
    {

        int x = 0;
        int y = 0;
        int routeLength = 0;
        GenerateSquare(x, y, 1, false);
        Vector2Int previousPos = new Vector2Int(x, y);
        y += 3;
        GenerateSquare(x, y, 1, false);
        NewRoute(x, y, routeLength, previousPos); // byrjar room gen
        FillWalls(); // eftir room gen, fyllir í veggi
        createExit(); // býr til endann á levelinu, sem verður notanlegur eftir að allir óvinir eru dánir
    }



    private void FillWalls() // fyllir í veggi
    {
        BoundsInt bounds = groundMap.cellBounds;
        for (int xMap = bounds.xMin - 10; xMap <= bounds.xMax + 10; xMap++)
        {
            for (int yMap = bounds.yMin - 10; yMap <= bounds.yMax + 10; yMap++)
            {
                Vector3Int pos = new Vector3Int(xMap, yMap, 0); // tekur inn position sem verður miðað við tiles á tilemap
                Vector3Int posRight = new Vector3Int(xMap + 1, yMap, 0);
                Vector3Int posLeft = new Vector3Int(xMap - 1, yMap, 0);
                Vector3Int posBelow = new Vector3Int(xMap, yMap - 1, 0);
                Vector3Int posBelow2 = new Vector3Int(xMap, yMap - 2, 0);
                Vector3Int posBelowLeft = new Vector3Int(xMap + 1, yMap - 2, 0);
                Vector3Int posBelowRight = new Vector3Int(xMap - 1, yMap - 2, 0);
                Vector3Int posBelowLeft2 = new Vector3Int(xMap + 1, yMap - 1, 0);
                Vector3Int posBelowRight2 = new Vector3Int(xMap - 1, yMap - 1, 0);
                Vector3Int posAboveRight = new Vector3Int(xMap - 1, yMap + 1, 0);
                Vector3Int posAboveLeft = new Vector3Int(xMap + 1, yMap + 1, 0);
                Vector3Int posAbove = new Vector3Int(xMap, yMap + 1, 0);
                
                TileBase tile = groundMap.GetTile(pos); // miðar positionin við tilemap tiles, til að geta sett rétta veggi á rétta staði
                TileBase tileRight = groundMap.GetTile(posRight);
                TileBase tileLeft = groundMap.GetTile(posLeft);
                TileBase tileBelow = groundMap.GetTile(posBelow);
                TileBase tileBelow2 = groundMap.GetTile(posBelow2);
                TileBase tileBelowLeft = groundMap.GetTile(posBelowLeft);
                TileBase tileBelowRight = groundMap.GetTile(posBelowRight);
                TileBase tileBelowLeft2 = groundMap.GetTile(posBelowLeft2);
                TileBase tileBelowRight2 = groundMap.GetTile(posBelowRight2);
                TileBase tileAboveRight = groundMap.GetTile(posAboveRight);
                TileBase tileAboveLeft = groundMap.GetTile(posAboveLeft);
                TileBase tileAbove = groundMap.GetTile(posAbove);


                
                if (tile == null)
                {
                    pitMap.SetTile(pos, pitTile);// setur pitmap allstaðar sem er ekki gólf
                    if (tileBelowLeft != null) wallMap.SetTile(pos, slabCornerLeftTile);//      restin setur niður ákveðið tile við ákveðið situation, 
                    if (tileBelowRight != null) wallMap.SetTile(pos, slabCornerRightTile); //   ekki 100% tilbúið vantar fleyri tile textures og 
                    if (tileBelowLeft2 != null) wallMap.SetTile(pos, rightWallTile); //         fleyri arguments til að þetta verður nánast fullkomið
                    if (tileBelowRight2 != null) wallMap.SetTile(pos, leftWallTile);
                    if (tileAboveLeft != null) wallMap.SetTile(pos, pitCornerRight);
                    if (tileAboveRight != null) wallMap.SetTile(pos, pitCornerLeft);
                    if (tileRight != null) wallMap.SetTile(pos, rightWallTile);
                    if (tileLeft != null) wallMap.SetTile(pos, leftWallTile);
                    if (tileBelow2 != null) wallMap.SetTile(pos, slabWallTile);
                    if (tileLeft != null && tileBelow2 != null) wallMap.SetTile(pos, CornerRightTile);
                    if (tileRight != null && tileBelow2 != null) wallMap.SetTile(pos, CornerLeftTile);
                    if (tileBelow != null) wallMap.SetTile(pos, topWallTile);
                    if (tileAbove != null) wallMap.SetTile(pos, botWallTile);
                    if (tileAbove != null) bottomSlabMap.SetTile(posAbove, slabWallTile);
                    if (tileAbove != null && tileRight != null) bottomSlabMap.SetTile(posAbove, slabWallTileRight);
                    if (tileAbove != null && tileLeft != null) bottomSlabMap.SetTile(posAbove, slabWallTileLeft);
                    if (tileAbove != null && tileLeft != null) wallMap.SetTile(pos, leftWallPit);
                    if (tileAbove != null && tileRight != null) wallMap.SetTile(pos, rightWallPit);
                    
                    
                }
            }
        }
        // FindObjectOfType<CreateNodesFromTilemap>().generateNodes();
        CreateNodesFromTilemaps CNFT = gameObject.GetComponent<CreateNodesFromTilemaps>();
        CNFT.createNodeFromTilemaps();
    }

    private void NewRoute(int x, int y, int routeLength, Vector2Int previousPos)
    {
        if (routeCount < maxRoutes)
        {
            routeCount++;
            while (++routeLength < maxRouteLength)
            {
                //Initialize
                bool routeUsed = false;
                int xOffset = x - previousPos.x; //0
                int yOffset = y - previousPos.y; //3
                int roomSize = 1; //Hallway size
                if (rand.Next(1,101) <= roomRate)
                    roomSize = rand.Next(4, 8);
                previousPos = new Vector2Int(x, y);

                //fara beint
                if (rand.Next(1,101) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x + xOffset, previousPos.y + yOffset, roomSize, true);
                    
                        NewRoute(previousPos.x + xOffset, previousPos.y + yOffset, rand.Next(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        x = previousPos.x + xOffset;
                        y = previousPos.y + yOffset;
                        GenerateSquare(x, y, roomSize, false);
                        routeUsed = true;
                    }
                }

                //beygja til vinstri
                if (rand.Next(1, 101) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x - yOffset, previousPos.y + xOffset, roomSize, true);
                        NewRoute(previousPos.x - yOffset, previousPos.y + xOffset, rand.Next(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y + xOffset;
                        x = previousPos.x - yOffset;
                        GenerateSquare(x, y, roomSize, false);
                        routeUsed = true;
                    }
                }
                //beygja til hægri
                if (rand.Next(1, 101) <= deviationRate)
                {
                    if (routeUsed)
                    {
                        GenerateSquare(previousPos.x + yOffset, previousPos.y - xOffset, roomSize, true);
                        NewRoute(previousPos.x + yOffset, previousPos.y - xOffset, rand.Next(routeLength, maxRouteLength), previousPos);
                    }
                    else
                    {
                        y = previousPos.y - xOffset;
                        x = previousPos.x + yOffset;
                        GenerateSquare(x, y, roomSize, false);
                        routeUsed = true;
                    }
                }

                if (!routeUsed)
                {
                    x = previousPos.x + xOffset;
                    y = previousPos.y + yOffset;
                    GenerateSquare(x, y, roomSize, false);
                }
            }
        }
    }
    private void GenerateSquare(int x, int y, int radius, bool routeused2) // setur niður groundtiles allstaðar sem er beðið um það
    {
        for (int tileX = x - radius; tileX <= x + radius; tileX++)
        {
            for (int tileY = y - radius; tileY <= y + radius; tileY++)
            {
                Vector3Int tilePos = new Vector3Int(tileX, tileY, 0);
                if (rand.Next(1, 101) <= 5) // 5% líkur að það verði spiketrap í stað groundtile
                {
                    groundMap.SetTile(tilePos, SpikeTile);
                    groundTileList.Add(tilePos);
                    float floatX = tileX + 0.5f, floatY = tileY + 0.5f;
                    Instantiate(spiketrap, new Vector3(floatX, floatY, 0) ,Quaternion.identity);// using float so the spike trap spawns in the middle of the tile
                }
                if (!groundMap.GetSprite(tilePos) == SpikeTile) // ef ekki var sett spiketrap, þá setum við groundtile
                {
                    groundMap.SetTile(tilePos, groundTile);
                    groundTileList.Add(tilePos);
                    if (rand.Next(1, 101) <= 1)
                    {
                        float floatX = tileX + 0.5f, floatY = tileY + 0.5f;
                        Instantiate(Enemy1, new Vector3(floatX, floatY, 0), Quaternion.identity);
                    }
                }

            }
        }
    }

    private void createExit() // býr til endann á dungeoninu
    {
        Debug.Log(groundTileList.Count);

        int exitTile = rand.Next(1, groundTileList.Count-1);
        if (groundTileList[exitTile] == player.transform.position)
        {
            createExit();
        }
        else
        {
            groundMap.SetTile(groundTileList[exitTile], null);
            Vector3 ladderPos = groundTileList[exitTile];
            ladderPos.x += 0.5f; ladderPos.y += 0.5f;
            Instantiate(exitLadder, ladderPos, Quaternion.identity);
            
        }
    }
}