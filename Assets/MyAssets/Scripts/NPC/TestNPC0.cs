using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bug issue # 1 : http://answers.unity3d.com/questions/745685/nullreferenceexception-on-startcoroutine.html

public class TestNPC0 : Actor, INpc
{
    [SerializeField]
    private Transform minExtent;
    [SerializeField]
    private Transform maxExtent;
    [SerializeField]
    private ActorController actorController;
    [SerializeField]
    private TestNpcAI ai;

    private World containWorld;
    private CustomAABB aabb;
    private ActorStatData status;

    public override void Init(ActorStatData statData, PathFinderInitData pathData, Vector3 pos, World world)
    {
        containWorld = world;
        ai.Init(actorController, pathData);
        status = statData;
        aabb.MakeAABB(minExtent.position, maxExtent.position);
        gameObject.transform.position = pos;
        
        StartCoroutine(SimpleGravityForce());
        StartCoroutine(ReMakeAABB());
        ai.StartBT();
    }

    private IEnumerator SimpleGravityForce()
    {
        while (true)
        {
            CollideInfo collideInfo = containWorld.customOctree.Collide(aabb);
            if (!collideInfo.isCollide) transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            yield return null;
        }
    }
    private IEnumerator ReMakeAABB()
    {
        while (true)
        {
            aabb.MakeAABB(minExtent.position, maxExtent.position);
            yield return null;
        }
    }

    void INpc.Talk()
    {
        throw new NotImplementedException();
    }
}
