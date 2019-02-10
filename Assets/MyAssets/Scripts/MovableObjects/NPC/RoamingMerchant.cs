using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bug issue # 1 : http://answers.unity3d.com/questions/745685/nullreferenceexception-on-startcoroutine.html

public class RoamingMerchant : Actor, INpc, IMerchantNPC
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

    public override void Init(Vector3 pos, World world, ACTOR_TYPE actorType)
    {
        ActorType = actorType;
        Controller = gameObject.AddComponent<NPCController>();
        Controller.Init(world);
        gameObject.transform.position = pos;
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
}
