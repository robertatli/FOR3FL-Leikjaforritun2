using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollower : MonoBehaviour {
    public List<Vector3> currentPath;
    int index = 1;
    GameObject target;
    private float moveTime = 0.1f;
    public bool isMoving = false;
    private bool onExit = false;
    private bool onCooldown = false;

    private void Start()
    {
        getPath();
    }

    // Update is called once per frame
    void Update() {
        float distanceFromPlayer = Vector3.Distance(this.transform.position, Pathfinding.me.player.transform.position); 
        if (isMoving || onCooldown) return;
        if (distanceFromPlayer < 5f)//  ef player er nálægt enemy
        {
            getPath(); // þá pathfindar hann að playerinum
            StartCoroutine(actionCooldown(0.5f));
        }

        StartCoroutine(actionCooldown(0.5f));
        if (index < 0) return; // svo að það spammist ekki console
        Vector3 dir = currentPath[index];
        StartCoroutine(SmoothMovement(dir));
	}


    private IEnumerator SmoothMovement(Vector3 end) // eins og í playerController bara fyrir enemy
    {
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

    void getPath() // pathfinding
    {
        target = Pathfinding.me.player;
        currentPath = Pathfinding.me.findPath(this.gameObject, target);
        index = 0;
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

}
