using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectileCollision : MonoBehaviour
{
    public float timeToDestroy;
    public int damage;

    private void Update()
    {
        transform.Rotate(0, 0, -1200 * Time.deltaTime);
        timeToDestroy -= Time.deltaTime;
        if (timeToDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("projectile") && !other.CompareTag("Player"))
        {
            other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
            Debug.Log("Collided with object" + other.gameObject);
        }  
    }
}
