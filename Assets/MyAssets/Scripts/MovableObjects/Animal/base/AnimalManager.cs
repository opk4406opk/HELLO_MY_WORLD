using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalManager : AnimalSpawner
{
    public override void Init()
    {
    }

    public override void RandomSpawn()
    {
    }

    public override void SpawnActor(int uniqueID, string worldUniqueID, Vector3 spawnPos, int num, bool initShow = false)
    {
        for (int spawnNum = 0; spawnNum < num; spawnNum++)
        {
            AnimalDataFile.Instance.AnimalSpawnDatas.TryGetValue(uniqueID, out AnimalSpawnData spawnData);
            Actor instance = Instantiate(GameResourceSupervisor.GetInstance().ActorPrefabs[(int)ACTOR_TYPE.ANIMAL]
                .Group[KojeomUtility.GetResourceNumberFromID(spawnData.ResourceID)]
                .LoadSynchro(), spawnPos, Quaternion.identity)
                .GetComponent<Actor>();
            instance.transform.parent = ActorSuperviosr.Instance.GetSpawnedGroupTransform();
            //
            WorldManager.Instance.WholeWorldStates.TryGetValue(worldUniqueID, out WorldState worldState);
            if (worldState.RealTimeStatus == WorldRealTimeStatus.Loading ||
               worldState.RealTimeStatus == WorldRealTimeStatus.LoadFinish)
            {
                worldState.SubWorldInstance.RegisterActor(instance);
                int spanwID = instance.GetHashCode();
                //
                switch (spawnData.AnimalType)
                {
                    case ANIMAL_TYPE.Chiken:
                    case ANIMAL_TYPE.Dog:
                    case ANIMAL_TYPE.Fox:
                    case ANIMAL_TYPE.Lion:
                    case ANIMAL_TYPE.Pig:
                    case ANIMAL_TYPE.Cow:
                        instance.Init(spawnData, worldState.SubWorldInstance, spanwID);
                        if (initShow == false)
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
