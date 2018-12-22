using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovableObjectSpawner : MonoBehaviour
{
    public abstract void Init();
    public abstract void GenerateToWorld();
}
