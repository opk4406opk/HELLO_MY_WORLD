using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bug issue # 1 : http://answers.unity3d.com/questions/745685/nullreferenceexception-on-startcoroutine.html

public class RoamingMerchant : NPCActor, IMerchantNPC
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

    public override void Init(ActorSpawnData spawnData, World world)
    {
        ActorType = spawnData.ActorType;
        HealthPoint = spawnData.HP;
        MagicaPoint = spawnData.MP;
        AttackPoint = spawnData.AP;
        Name = spawnData.NAME;
        NpcType = ((NPCSpawnData)spawnData).NpcType;
        //
        Controller = gameObject.AddComponent<NPCController>();
        Controller.Init(world);
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

    public override ACTOR_TYPE GetActorType()
    {
        return ActorType;
    }

    public override void Tick(float deltaTime)
    {
        // to do
    }

    public override void Show()
    {
        gameObject.SetActive(true);
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
    }

}
