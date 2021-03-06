﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RequestSpawnActorData
{
    public string WorldAreaUniqueID;
    public string SubWorldUniqueID;
    public int ActorUniqueID;
    public Vector3 SpawnPosition;
    public ACTOR_TYPE ActorType;
    public int Quantity;
    public bool SpawnAndShow;

    public RequestSpawnActorData(string subWorldUniqueID, string worldAreaUniqueID, int actorUniqueID, Vector3 spawnPosition, ACTOR_TYPE actorType, int quantityNumber, bool spawnAndShow)
    {
        SubWorldUniqueID = subWorldUniqueID;
        WorldAreaUniqueID = worldAreaUniqueID;
        ActorUniqueID = actorUniqueID;
        SpawnPosition = spawnPosition;
        ActorType = actorType;
        Quantity = quantityNumber;
        SpawnAndShow = spawnAndShow;
    }
}

public abstract class RequestSpawnActorMessage
{
    public RequestSpawnActorData SpawnData;
}
public class RequestSpawnNPCMessage : RequestSpawnActorMessage
{
    public NPC_TYPE NPCType;
    public RequestSpawnNPCMessage(RequestSpawnActorData spawnData, NPC_TYPE npcType)
    {
        SpawnData = spawnData;
        NPCType = npcType;
    }
}
public class RequestSpawnAnimalMessage : RequestSpawnActorMessage
{
    public ANIMAL_TYPE AnimalType;
    public RequestSpawnAnimalMessage(RequestSpawnActorData spawnData, ANIMAL_TYPE animalType)
    {
        SpawnData = spawnData;
        AnimalType = animalType;
    }
}
public class RequestSpawnMonsterMessage : RequestSpawnActorMessage
{
    public MONSTER_TYPE MonsterType;
    public RequestSpawnMonsterMessage(RequestSpawnActorData spawnData, MONSTER_TYPE monsterType)
    {
        SpawnData = spawnData;
        MonsterType = monsterType;
    }
}

/// <summary>
/// 런타임중에 게임내에 생성되는 모든 Actor들을 관리/스포닝을 하는 클래스.
/// </summary>
public class ActorSuperviosr : MonoBehaviour
{
    [SerializeField]
    private Transform SpawnedGroupTransform;
    public bool bRunningTick { get; private set; }
    public NPCManager NPCManagerInstance { get; private set; }
    public AnimalManager AnimalManagerInstance { get; private set; }
    //
    private Queue<RequestSpawnActorMessage> RequestSpawnMessages;

    private static ActorSuperviosr _Instance = null;
    public static ActorSuperviosr Instance
    {
        get
        {
            if (_Instance == null) KojeomLogger.DebugLog("ActorSuperviosr Initialize Failed", LOG_TYPE.ERROR);
            return _Instance;
        }
    }

    public Transform GetSpawnedGroupTransform()
    {
        return SpawnedGroupTransform;
    }

    public void Init()
    {
        // to do
        _Instance = this;
        RequestSpawnMessages = new Queue<RequestSpawnActorMessage>();
        NPCManagerInstance = gameObject.GetComponentInChildren<NPCManager>();
        NPCManagerInstance.Init();
        //
        AnimalManagerInstance = gameObject.GetComponentInChildren<AnimalManager>();
        AnimalManagerInstance.Init();
    }

    public void Begin()
    {
        //
        StartCoroutine(Tick());
    }

    public void End()
    {
        bRunningTick = false;
    }

    public void RequestSpawnActor(RequestSpawnActorMessage message)
    {
        RequestSpawnMessages.Enqueue(message);
    }

    public void RequestSpawnRandomNPC(NPC_TYPE npcType, string subWorldUniqueID, string worldAreaUniqueID, int quantity, bool spawnAndShow)
    {
        WorldAreaManager.Instance.GetWorldArea(worldAreaUniqueID).SubWorldStates.TryGetValue(subWorldUniqueID, out SubWorldState worldState);
        var data = new RequestSpawnActorData(subWorldUniqueID, worldAreaUniqueID,
                                            KojeomUtility.RandomInteger(0, NPCDataFile.Instance.NpcSpawnDatas.Count - 1),
                                            worldState.SubWorldInstance.GetRandomRealPositionAtSurface(), ACTOR_TYPE.NPC, quantity, spawnAndShow);
        RequestSpawnNPCMessage msg = new RequestSpawnNPCMessage(data, npcType);
        RequestSpawnMessages.Enqueue(msg);
    }

    public void RequestSpawnRandomMonster(MONSTER_TYPE monsterType, string subWorldUniqueID, string worldAreaUniqueID, int quantity, bool spawnAndShow)
    {
        // to do.
    }

    public void RequestSpawnRandomAnimal(ANIMAL_TYPE animalType, string subWorldUniqueID, string worldAreaUniqueID, int quantity, bool spawnAndShow)
    {
        WorldAreaManager.Instance.GetWorldArea(worldAreaUniqueID).SubWorldStates.TryGetValue(subWorldUniqueID, out SubWorldState worldState);
        var data = new RequestSpawnActorData(subWorldUniqueID, worldAreaUniqueID,
                                            KojeomUtility.RandomInteger(0, AnimalDataFile.Instance.AnimalSpawnDatas.Count - 1),
                                            worldState.SubWorldInstance.GetRandomRealPositionAtSurface(), ACTOR_TYPE.ANIMAL, quantity, spawnAndShow);
        RequestSpawnAnimalMessage msg = new RequestSpawnAnimalMessage(data, animalType);
        RequestSpawnMessages.Enqueue(msg);
    }

    private IEnumerator Tick()
    {
        bRunningTick = true;
        KojeomLogger.DebugLog("ActorSuperviosr::Tick Start.", LOG_TYPE.NORMAL);
        while(bRunningTick == true)
        {
            if(RequestSpawnMessages.Count > 0)
            {
                var message = RequestSpawnMessages.Dequeue();
                switch (message.SpawnData.ActorType)
                {
                    case ACTOR_TYPE.ANIMAL:
                        AnimalManagerInstance.SpawnActor(message.SpawnData.ActorUniqueID,
                            message.SpawnData.SubWorldUniqueID,
                            message.SpawnData.WorldAreaUniqueID,
                            message.SpawnData.SpawnPosition,
                            message.SpawnData.Quantity,
                            message.SpawnData.SpawnAndShow);
                        break;
                    case ACTOR_TYPE.MONSTER:
                        break;
                    case ACTOR_TYPE.NPC:
                        NPCManagerInstance.SpawnActor(message.SpawnData.ActorUniqueID,
                            message.SpawnData.SubWorldUniqueID,
                            message.SpawnData.WorldAreaUniqueID,
                            message.SpawnData.SpawnPosition,
                            message.SpawnData.Quantity,
                            message.SpawnData.SpawnAndShow);
                        break;
                }
            }
            yield return null;
        }
        KojeomLogger.DebugLog("ActorSuperviosr::Tick End.", LOG_TYPE.NORMAL);
    }
}
