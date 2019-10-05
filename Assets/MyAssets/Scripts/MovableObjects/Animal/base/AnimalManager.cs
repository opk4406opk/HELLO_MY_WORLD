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

    public override void SpawnActor(int uniqueID, string subWorldUniqueID, string worldAreaUniqueID, Vector3 spawnPos, int num, bool initShow = false)
    {
        for (int spawnNum = 0; spawnNum < num; spawnNum++)
        {
            AnimalDataFile.Instance.AnimalSpawnDatas.TryGetValue(uniqueID, out AnimalSpawnData spawnData);
            Actor instance = Instantiate(GameResourceSupervisor.GetInstance().ActorPrefabs[(int)ACTOR_TYPE.ANIMAL]
                .Group[(int)KojeomUtility.GetResourceEnumFromID<ANIMAL_TYPE>(spawnData.ResourceID)]
                .LoadSynchro(), spawnPos, Quaternion.identity)
                .GetComponent<Actor>();
            instance.transform.parent = ActorSuperviosr.Instance.GetSpawnedGroupTransform();
            //
            WorldAreaManager.Instance.GetWorldArea(worldAreaUniqueID).SubWorldStates.TryGetValue(subWorldUniqueID, out SubWorldState worldState);
            if (worldState.RealTimeStatus == SubWorldRealTimeStatus.Loading ||
               worldState.RealTimeStatus == SubWorldRealTimeStatus.LoadFinish)
            {
                worldState.SubWorldInstance.RegisterActor(instance);
                int spanwID = instance.GetHashCode();
                //
                switch (spawnData.AnimalType)
                {
                    case ANIMAL_TYPE.Chick:
                    case ANIMAL_TYPE.Chiken:
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
                KojeomLogger.DebugLog(string.Format("World Id : {0} is Not Loaded..So, Actor Spawn Failed.", subWorldUniqueID), LOG_TYPE.ERROR);
            }
        }
    }
}
