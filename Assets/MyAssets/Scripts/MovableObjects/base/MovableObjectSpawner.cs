using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovableObjectSpawner : MonoBehaviour
{
    public abstract void Init();
    public abstract void RandomSpawn();
}

public abstract class NPCSpawner : MovableObjectSpawner
{
    public abstract void Spawn(int uniqueID, World world, int num, bool initShow = false);
}

public abstract class MonsterSpawner : MovableObjectSpawner
{
    public abstract void Spawn(int uniqueID, World world, int num, bool initShow = false);
}
