using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public int damage;
    bool invincible = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!invincible)
        {
            if (other.CompareTag("Player")) // gerir damage við player ef það collidar við hann
            {
                other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);
                Debug.Log("Collided with object" + other.gameObject);
                invincible = true;
                StartCoroutine(Cooldown());
                invincible = false;
            }
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(1);
    }

}
