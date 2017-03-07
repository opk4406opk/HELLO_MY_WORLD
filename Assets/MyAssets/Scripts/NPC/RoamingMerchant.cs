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
    private ActorStatData status;

    public override void Init(ActorStatData statData, PathFinderInitData pathData, Vector3 pos, World world)
    {
        ai.Init(actorController, pathData);
        status = statData;
        ((ActorController)actorController).Init(world);
        gameObject.transform.position = pos;
        ai.StartBT();
    }

    void INpc.Talk()
    {
        // to do.
    }
}
