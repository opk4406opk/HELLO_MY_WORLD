using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bug issue # 1 : http://answers.unity3d.com/questions/745685/nullreferenceexception-on-startcoroutine.html

public class RoamingMerchant : Actor, INpc
{
    [SerializeField]
    private NPCController actorController;
    [SerializeField]
    private RoamingNpcAI ai;
    [SerializeField]
    private TextMeshController _textMeshController;
    public TextMeshController textMeshController
    {
        get { return _textMeshController; }
    }

    public override void Init(Vector3 pos, World world)
    {
        ((ActorController)actorController).Init(world);
        gameObject.transform.position = pos;
    }

    void INpc.Talk()
    {
        // to do.
    }
}
