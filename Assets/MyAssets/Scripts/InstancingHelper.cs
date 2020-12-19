using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstancingHelper : MonoBehaviour
{
    public static InstancingHelper Instance = null;
    public void Init()
    {
        Instance = this;
        
    }

    public GameObject GetNewInstance(GameObject resourcePrefab)
    {
        return Instantiate(resourcePrefab);
    }

    public void DestroyInstance(GameObject target)
    {
        Destroy(target);
    }

}
