using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;


public class playerController : MonoBehaviour
{
    public playerController me;

    public Tilemap groundTileMap;
    public Tilemap wallTileMap;

    public bool isMoving = false;
    private bool onExit = false;
    private bool onCooldown = false;

    private float moveTime = 0.1f;
    public float maxShotSpeed = 25f;
    public GameObject dagger;

    void Awake()
    {
        me = this;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update() // fyrir movement keys og attack keys
    {
        if ( isMoving || onCooldown || onExit ) return;

        int horizontal = 0, vertical = 0;

        
        horizontal = (int)(Input.GetAxisRaw("Horizontal"));
        vertical = (int)(Input.GetAxisRaw("Vertical"));

        if (horizontal != 0)
        {
            vertical = 0;
        }
        else if (vertical != 0)
        {
            horizontal = 0;
        }

        if (horizontal != 0 || vertical != 0)
        {
            StartCoroutine(actionCooldown(0.2f));
            Move(horizontal, vertical);
        }
        
        if (Input.GetButton("Fire1") && !isMoving) Fire(0f, dagger);
        if (Input.GetButton("Fire2") && !isMoving) Fire(180f, dagger);
        if (Input.GetButton("Fire3") && !isMoving) Fire(90f, dagger);
        if (Input.GetButton("Fire4") && !isMoving) Fire(270f, dagger);

    }

    void Fire(float angle, GameObject projectile) // kastar hníf
    {
        StartCoroutine(actionCooldown(0.5f));
        Quaternion toAngle = Quaternion.Euler(transform.eulerAngles + new Vector3(0, 0, angle));
        GameObject newProjectile = Instantiate(projectile, transform.position, toAngle);
        newProjectile.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        Rigidbody2D rigidbody = newProjectile.GetComponent<Rigidbody2D>();
        rigidbody.AddRelativeForce(Vector3.up * maxShotSpeed);
    }

    private void Move(int xDir, int yDir) // hreyfir characterinn eftir grid.
    {
        Vector2 startCell = transform.position;
        Vector2 targetCell = startCell + new Vector2(xDir, yDir);

        bool isOnGround = getCell(groundTileMap, startCell) != null; //ef playerinn er á ground tile
        bool hasGroundTile = getCell(groundTileMap, targetCell) != null; // ef tileið sem playerinn ætlar á er með ground tile
        bool hasWallTile = getCell(wallTileMap, targetCell) != null;


        if (isOnGround)
        {
            if (hasGroundTile && !hasWallTile)
            {
                if (doorCheck(targetCell))
                {
                    StartCoroutine(SmoothMovement(targetCell));
                }
                else
                    StartCoroutine(BlockedMovement(targetCell));
            }
        }
    }
    private IEnumerator SmoothMovement(Vector3 end) // til að gera hreyfingu milli tiles þægilegri
    {
        //while (isMoving) yield return null;

        isMoving = true;

        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = 1 / moveTime;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }
    //Blocked animation
    private IEnumerator BlockedMovement(Vector3 end) // þar sem eru veggir þar getur playerinn ekki labbað
    {
        //while (isMoving) yield return null;

        isMoving = true;

        Vector3 originalPos = transform.position;

        end = transform.position + ((end - transform.position) / 3);
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;
        float inverseMoveTime = (1 / (moveTime * 2));

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, end, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;

            yield return null;
        }

        sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, originalPos, inverseMoveTime * Time.deltaTime);
            transform.position = newPosition;
            sqrRemainingDistance = (transform.position - originalPos).sqrMagnitude;

            yield return null;
        }

        isMoving = false;
    }


    private IEnumerator actionCooldown(float cooldown) // svo playerinn getur ekki kastað milljón hnífum á sekúndu, né þegar hann er að hreyfa sig
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
    private bool doorCheck(Vector2 targetCell) 
    {
        Collider2D coll = whatsThere(targetCell);

        //No obstacle, we can walk there
        if (coll == null)
        {
            return true;
        }
        else
        {
            return false;
        }


    }


    public Collider2D whatsThere(Vector2 targetPos) // returnar hvað er í tileinum fyrir framan sig
    {
        RaycastHit2D hit;
        hit = Physics2D.Linecast(targetPos, targetPos);
        return hit.collider;
    }

    private TileBase getCell(Tilemap tilemap, Vector2 cellWorldPos)
    {
        return tilemap.GetTile(tilemap.WorldToCell(cellWorldPos));
    }
    private bool hasTile(Tilemap tilemap, Vector2 cellWorldPos)
    {
        return tilemap.HasTile(tilemap.WorldToCell(cellWorldPos));
    }
}
