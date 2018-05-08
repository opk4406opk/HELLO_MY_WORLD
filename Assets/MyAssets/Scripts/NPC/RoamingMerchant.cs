using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bug issue # 1 : http://answers.unity3d.com/questions/745685/nullreferenceexception-on-startcoroutine.html

public class RoamingMerchant : Actor, INpc, IMerchantNPC
{
    public override event del_OnClickActor OnClickedActor;
    private ActorController actorController;
    [SerializeField]
    private RoamingNpcAI ai;
    [SerializeField]
    private TextMeshController _textMeshController;
    public TextMeshController textMeshController
    {
        get { return _textMeshController; }
    }

    private List<int> _sellingItemIDs;
    public List<int> sellingItemIDs
    {
        set { _sellingItemIDs = value; }
    }

    public List<int> GetSellingItemIds()
    {
        return _sellingItemIDs;
    }

    public override void Init(Vector3 pos, World world, ACTOR_TYPE actorType)
    {
        this.actorType = actorType;
        actorController = gameObject.GetComponent<ActorController>();
        actorController.Init(world);
        gameObject.transform.position = pos;
    }

    public override ActorController GetActorController()
    {
        return actorController;
    }

    void INpc.Talk()
    {
        // to do.
        OnClickedActor(this);
        UIPopupManager.OpenPopupUI(POPUP_TYPE.shop);
    }

    public override ACTOR_TYPE GetActorType()
    {
        return actorType;
    }
}
