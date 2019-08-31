﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : NPCSpawner
{
    private Actor LastestClickedActor;

    public override void Init()
    {
        // to do
    }

    /// <summary>
    /// 가장 최근에 선택한 Actor를 가져온다.
    /// </summary>
    /// <returns></returns>
    public Actor GetLastestClickedActor()
    {
        return LastestClickedActor;
    }

    private void OnClickedActor(Actor actor)
    {
        KojeomLogger.DebugLog(string.Format("OnClicked Actor name : {0}, type : {1}", actor.name, actor.GetActorType()));
        LastestClickedActor = actor;
    }

    /// <summary>
    /// 무작위로 NPC들을 월드에 생성합니다.
    /// </summary>
    public override void RandomSpawn()
    { 
        var gameConfig = GameConfigDataFile.Instance.GetGameConfigData();
        for(int loopCount = 0; loopCount < 10; loopCount++)
        {
            
        }
    }

    public override void SpawnActor(int uniqueID, string worldUniqueID, Vector3 spawnPos, int num, bool initShow = false)
    {
        for(int spawnNum = 0; spawnNum < num; spawnNum++)
        {
            NPCDataFile.Instance.NpcSpawnDatas.TryGetValue(uniqueID, out NPCSpawnData spawnData);
            Actor instance = Instantiate(GameResourceSupervisor.GetInstance().ActorPrefabs[(int)ACTOR_TYPE.NPC]
                .Group[KojeomUtility.GetResourceNumberFromID(spawnData.ResourceID)]
                .LoadSynchro(), spawnPos, Quaternion.identity)
                .GetComponent<Actor>();
            instance.transform.parent = ActorSuperviosr.Instance.GetSpawnedGroupTransform();
            //
            WorldManager.Instance.WholeWorldStates.TryGetValue(worldUniqueID, out WorldState worldState);
            if(worldState.RealTimeStatus == WorldRealTimeStatus.Loading ||
               worldState.RealTimeStatus == WorldRealTimeStatus.LoadFinish)
            {
                worldState.SubWorldInstance.RegisterActor(instance);
                int spanwID = instance.GetHashCode();
                //
                switch (spawnData.NpcType)
                {
                    case NPC_TYPE.Merchant:
                    case NPC_TYPE.Guard:
                        instance.Init(spawnData, worldState.SubWorldInstance, spanwID);
                        if(initShow == false)
                        {
                            instance.Hide();
                        }
                        else
                        {
                            instance.Show();
                        }
                        break;
                }
                ActorGroup.Add(spanwID, instance);
            }
            else
            {
                KojeomLogger.DebugLog(string.Format("World Id : {0} is Not Loaded..So, Actor Spawn Failed.", worldUniqueID), LOG_TYPE.ERROR);
            }
        }
    }

}
