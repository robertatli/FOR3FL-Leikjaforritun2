using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExtensions
{
    public static GameObject GetChild(this GameObject inside, string wanted, bool recursive = false)
    {
        foreach (Transform child in inside.transform)
        {
            if (child.name == wanted) return child.gameObject;
            if (recursive)
            {
                var within = GetChild(child.gameObject, wanted, true);
                if (within) return within;
            }
        }
        return null;
    }
}
