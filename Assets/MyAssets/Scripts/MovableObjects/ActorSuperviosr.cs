using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RequestSpawnActorData
{
    public string WorldUniqueID;
    public int ActorUniqueID;
    public Vector3 SpawnPosition;
    public ACTOR_TYPE ActorType;
    public int Num;

    public RequestSpawnActorData(string worldUniqueID, int actorUniqueID, Vector3 spawnPosition, ACTOR_TYPE actorType, int num)
    {
        WorldUniqueID = worldUniqueID;
        ActorUniqueID = actorUniqueID;
        SpawnPosition = spawnPosition;
        ActorType = actorType;
        Num = num;
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
    public bool IsRunningTick { get; private set; }
    public NPCManager NPCManagerInstance { get; private set; }
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
    public void Init()
    {
        // to do
        _Instance = this;
        RequestSpawnMessages = new Queue<RequestSpawnActorMessage>();
        NPCManagerInstance = gameObject.GetComponentInChildren<NPCManager>();
        NPCManagerInstance.Init();
    }

    public void Begin()
    {
        //
        StartCoroutine(Tick());
    }

    public void End()
    {
        IsRunningTick = false;
    }

    public void RequestSpawnActor(RequestSpawnActorMessage message)
    {
        RequestSpawnMessages.Enqueue(message);
    }

    public void RequestSpawnRandomNPC(NPC_TYPE npcType, string worldUniqueID, int num)
    {
        WorldManager.Instance.WholeWorldStates.TryGetValue(worldUniqueID, out WorldState worldState);
        var data = new RequestSpawnActorData(worldUniqueID,
            KojeomUtility.RandomInteger(0, NPCDataFile.Instance.NpcSpawnDatas.Count - 1),
            worldState.subWorldInstance.RandomPosAtSurface(), ACTOR_TYPE.NPC, num);
        RequestSpawnNPCMessage msg = new RequestSpawnNPCMessage(data, npcType);
        RequestSpawnMessages.Enqueue(msg);
    }

    public void RequestSpawnRandomMonster(MONSTER_TYPE monsterType, string worldUniqueID, int num)
    {
        // to do.
    }

    private IEnumerator Tick()
    {
        IsRunningTick = true;
        KojeomLogger.DebugLog("ActorSuperviosr::Tick Start.", LOG_TYPE.NORMAL);
        while(IsRunningTick == true)
        {
            if(RequestSpawnMessages.Count > 0)
            {
                var message = RequestSpawnMessages.Dequeue();
                switch (message.SpawnData.ActorType)
                {
                    case ACTOR_TYPE.MONSTER:
                        break;
                    case ACTOR_TYPE.NPC:
                        NPCManagerInstance.SpawnActor(message.SpawnData.ActorUniqueID,
                            message.SpawnData.WorldUniqueID,
                            message.SpawnData.SpawnPosition,
                            message.SpawnData.Num);
                        break;
                }
            }
            yield return null;
        }
        KojeomLogger.DebugLog("ActorSuperviosr::Tick End.", LOG_TYPE.NORMAL);
    }
}
