using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovableObjectSpawner : MonoBehaviour
{
    public abstract void Init();
    public abstract void RandomSpawn();
    public abstract void SpawnActor(int uniqueID, string worldUniqueID, Vector3 spawnPos, int num, bool initShow = false);

    protected Dictionary<int, Actor> ActorGroup { get; private set; } = new Dictionary<int, Actor>();

    public void DestroyActor(int spawnID)
    {
        if (ActorGroup.TryGetValue(spawnID, out Actor actor) == true)
        {
            Destroy(actor);
            ActorGroup.Remove(spawnID);
            KojeomLogger.DebugLog(string.Format("spawnID : {0}, type : {1} Destroyed in World", spawnID, actor.GetActorType()), LOG_TYPE.NORMAL);
        }
        else
        {
            KojeomLogger.DebugLog(string.Format("spawnID : {0} Acotr Don't Find in World", spawnID), LOG_TYPE.NORMAL);
        }
    }
}

public abstract class NPCSpawner : MovableObjectSpawner
{
}

public abstract class MonsterSpawner : MovableObjectSpawner
{
}
