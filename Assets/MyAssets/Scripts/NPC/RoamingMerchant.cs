using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bug issue # 1 : http://answers.unity3d.com/questions/745685/nullreferenceexception-on-startcoroutine.html

public class RoamingMerchant : Actor, INpc
{
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
        get { return _sellingItemIDs; }
    }

    public override void Init(Vector3 pos, World world)
    {
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
        SceneToScene_Data.shopItemIDs = new List<int>(_sellingItemIDs);
        UIPopupManager.OpenShop();
    }
}
