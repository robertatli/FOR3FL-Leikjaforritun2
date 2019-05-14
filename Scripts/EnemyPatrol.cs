using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using TMPro;

public class EnemyPatrol : MonoBehaviour
{

    public Tilemap groundTileMap;
    public Tilemap wallTileMap;

    private bool isMoving = false;
    private bool onExit = false;
    private bool onCooldown = false;


    [SerializeField]
    Transform[] points;

    [SerializeField]
    private float moveTime = 0.1f;

    int destPoint = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = points[destPoint].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving || onCooldown || onExit) 
            return;

        StartCoroutine(actionCooldown(0.5f));
        Move();
        checkDestination();

    }

    void Move()
    {
        int xDir = 0, yDir = 0;
        Vector3 nextLocation = new Vector3(points[destPoint].transform.position.x, points[destPoint].transform.position.y, points[destPoint].transform.position.z);

        Vector2 startCell = transform.position;
        if (nextLocation.x > transform.position.x)
            xDir = 1;
        else if (nextLocation.x < transform.position.x)
            xDir = -1;
        else if (nextLocation.y > transform.position.y)
            yDir = 1;
        else if (nextLocation.y < transform.position.y)
            yDir = -1;

        Vector2 targetCell = startCell + new Vector2(xDir, yDir);

        StartCoroutine(SmoothMovement(targetCell));
    }

    private IEnumerator SmoothMovement(Vector3 end)
    {
        //while (isMoving) yield return null;

        isMoving = true;

        float inverseMoveTime = 1 / moveTime;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }
    

    private IEnumerator actionCooldown(float cooldown)
    {
        onCooldown = true;

        //float cooldown = 0.2f;
        while (cooldown > 0f)
        {
            cooldown -= Time.deltaTime;
            yield return null;
        }

        onCooldown = false;
    }

    void checkDestination()
    {

        Vector3 currentLocation = transform.position;

        if (currentLocation == points[destPoint].transform.position)
            destPoint += 1;


        if (destPoint == points.Length)
            destPoint = 0;
    }

}
