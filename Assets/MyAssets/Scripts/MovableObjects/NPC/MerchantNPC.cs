using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bug issue # 1 : http://answers.unity3d.com/questions/745685/nullreferenceexception-on-startcoroutine.html

public class MerchantNPC : NPCActor, IMerchantNPC
{
    public override event del_OnClickActor OnClickedActor;

    private List<int> SellingItemIds;

    public List<int> GetSellingItemIDList()
    {
        return SellingItemIds;
    }

    public void SetSellingItemIDList(List<int> ids)
    {
        SellingItemIds = ids;
    }

    public override void Init(ActorSpawnData spawnData, SubWorld world, int spawnID)
    {
        ActorType = spawnData.ActorType;
        HealthPoint = spawnData.HP;
        MagicaPoint = spawnData.MP;
        AttackPoint = spawnData.AP;
        Name = spawnData.NAME;
        NpcType = ((NPCSpawnData)spawnData).NpcType;
        ResourceID = spawnData.ResourceID;
        SpawnID = spawnID;
        UniqueID = spawnData.UniqueID;
        //
        Controller = gameObject.GetComponent<NPCController>();
        Controller.Init(world, this);
        Controller.StartAI();
    }


    public override void Update()
    {
        if(Controller != null)
        {
            Controller.Tick(Time.deltaTime);
        }
    }

    public override ActorController GetController()
    {
        return Controller;
    }

    void INpc.Talk()
    {
        // to do.
        OnClickedActor(this);
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.shop);
    }
}
