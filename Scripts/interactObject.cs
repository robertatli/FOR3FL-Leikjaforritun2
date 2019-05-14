using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class interactObject : MonoBehaviour
{

    bool onCooldown = false;

    void Update()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
            #elif UNITY_WEBPLAYER
            Application.OpenURL(webplayerQuitURL);
            #else
            Application.Quit();
            #endif
        }


        playerController PC = gameObject.GetComponent<playerController>();

        if (Input.GetButtonDown("Interact") && !PC.isMoving)
        {
            GameObject doorOpen = InRangeOfInteract("Door");
            GameObject Stairs = InRangeOfInteract("Stairs");
            GameObject Ladder = InRangeOfInteract("Ladder");
            GameObject Sign = InRangeOfInteract("Sign");

            if (Ladder != null)
            {
                GameObject[] enemyCount = GameObject.FindGameObjectsWithTag("Enemies"); //Checks if all enemies are killed before going to next level.
                if (enemyCount.Length == 0)
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                else
                {
                    Debug.Log("Need this many enemies to kill: " + enemyCount.Length);
                }
            }

            else if (Stairs != null)
            {
                ChangeLevel levelChange = Stairs.GetComponent<ChangeLevel>();
                HealthScript HS = gameObject.GetComponent<HealthScript>();
                Transform newArea = levelChange.areaLocation.GetChild("Location").transform;
                gameObject.transform.position = new Vector3(newArea.position.x+1,newArea.position.y+0.1f,newArea.position.z);
                HS.respawnPoint = gameObject.transform.position;
                Stairs = null;
            }


            else if (doorOpen != null)
            {
                if (doorOpen.GetChild("Door Closed").activeSelf)//ef hurðin er lokuð
                {
                    doorOpen.GetChild("Door Closed").SetActive(false);// setur lokuðu hurðina sem ekki active
                    doorOpen.GetChild("Door Open").SetActive(true); // setur opnuðu hurðina sem active
                }
                else if (!doorOpen.GetChild("Door Closed").activeSelf) // ef hurðin er opin
                {
                    doorOpen.GetChild("Door Open").SetActive(false); // setur opnuðu hurðina sem ekki active
                    doorOpen.GetChild("Door Closed").SetActive(true);// setur lokuðu hurðina sem active  
                }
            }

            else if (Sign != null)
            {
                    if (!Sign.GetChild("SignCanvas").activeInHierarchy)
                        Sign.GetChild("SignCanvas").SetActive(true);
                    else
                        Sign.GetChild("SignCanvas").SetActive(false);


            }
            
        }
    }
    
    GameObject InRangeOfInteract(string tag)
    {
        GameObject[] goWithTag = GameObject.FindGameObjectsWithTag(tag);

        for (int i = 0; i < goWithTag.Length; ++i)
        {
            if (Vector3.Distance(transform.position, goWithTag[i].GetChild("Location").transform.position) <= 1.5) // Looks for tiles in a minimum distance of 2.
            {
                Debug.Log(goWithTag[i]);
                return goWithTag[i];
            }
        }

        return null;
    }



}




