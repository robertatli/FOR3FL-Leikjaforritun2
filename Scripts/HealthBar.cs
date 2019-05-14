using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    List<bool[]> healthSpriteOrderList = new List<bool[]>(); // A list of lists filled with booleans.

    public int previousHP = 6; 

    
    void Update()
    {
        InitSpriteOrder();
        ChangeHealthSprite();

    }

    private void InitSpriteOrder() // Premade boolean list determining what sprite should be on and off when the character is at a specific health.
    {
        healthSpriteOrderList.Add(new bool[] { false, false, false, false, false, false, true, true, true });
        healthSpriteOrderList.Add(new bool[] { false, false, false, true, false, false, false, true, true });
        healthSpriteOrderList.Add(new bool[] { true, false, false, false, false, false, false, true, true });
        healthSpriteOrderList.Add(new bool[] { true, false, false, false, true, false, false, false, true });
        healthSpriteOrderList.Add(new bool[] { true, true, false, false, false, false, false, false, true });
        healthSpriteOrderList.Add(new bool[] { true, true, false, false, false, true, false, false, false });
        healthSpriteOrderList.Add(new bool[] { true, true, true, false, false, false, false, false, false });
    }



    public void ChangeHealthSprite()
    {
        HealthScript HS = gameObject.GetComponentInParent<HealthScript>();
        if (HS.health == previousHP)
        {
            return;
        }

        for (int i = 0; i < 9; i++)
        {
            string childName = "H" + i.ToString(); // Gets the child by name (hearts are named H1-7), and then sets them active depending on character health.
            gameObject.GetChild(childName).SetActive(healthSpriteOrderList[HS.health][i]);

        }

        previousHP = HS.health; //current hp becomes the previous hp.


    }
}
