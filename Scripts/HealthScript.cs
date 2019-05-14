using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScript : MonoBehaviour
{
    public int health;
    public GameObject bloodParticle;
    public Vector3 respawnPoint;

    void Awake()
    {
        respawnPoint = transform.position;    
    }

    public void TakeDamage(int damage)
    {
        Instantiate(bloodParticle, transform.position, Quaternion.identity);
        health -= damage;
        AudioScript audio = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioScript>(); //Check to see if the person who got hurt was enemy or player, and play the right audio.
        if (gameObject.transform.CompareTag("Player"))  
            audio.TDamageSound();
        else
            audio.DDamageSound();
    }

    void Update()
    {
        if (health <= 0)
        {
            playerController PC = gameObject.GetComponent<playerController>();
            if (gameObject.transform.CompareTag("Player") && !PC.isMoving)
            {
                Debug.Log("respawn");
                gameObject.transform.position = respawnPoint;
                health = 6;
            }
            else if (!gameObject.transform.CompareTag("Player"))
            {
                Destroy(gameObject);
            }
        }
    }
}
