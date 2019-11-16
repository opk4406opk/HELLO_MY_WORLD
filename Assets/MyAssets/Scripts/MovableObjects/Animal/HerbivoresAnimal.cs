using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerbivoresAnimal : AnimalActor
{
    public override event del_OnClickActor OnClickedActor;
    public override ActorController GetController()
    {
        return Controller;
    }

    public override void Init(ActorSpawnData spawnData, SubWorld world, int spawnID)
    {
        ActorType = spawnData.ActorType;
        HealthPoint = spawnData.HP;
        MagicaPoint = spawnData.MP;
        AttackPoint = spawnData.AP;
        Name = spawnData.NAME;
        AnimalType = ((AnimalSpawnData)spawnData).AnimalType;
        ResourceID = spawnData.ResourceID;
        SpawnID = spawnID;
        UniqueID = spawnData.UniqueID;
        //
        Controller = gameObject.GetComponent<AnimalController>();
        Controller.Init(world, this);
        Controller.StartAI();
    }

    public override void Update()
    {
        if (Controller != null)
        {
            Controller.Tick(Time.deltaTime);
        }
    }
}
