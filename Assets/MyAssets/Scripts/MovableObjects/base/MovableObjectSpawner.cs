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
    public abstract void Spawn(NPC_TYPE npcType, World world, int num, bool initShow = false);
}

public abstract class MonsterSpawner : MovableObjectSpawner
{
    public abstract void Spawn(MONSTER_TYPE monType, World world, int num, bool initShow = false);
}
