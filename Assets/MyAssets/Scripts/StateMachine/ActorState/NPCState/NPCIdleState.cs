using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCIdleState : AActorState, IState
{
    public NPCIdleState(Actor instance)
    {
        ActorInstance = instance;
    }

    public void InitState()
    {
        throw new System.NotImplementedException();
    }

    public void ReleaseState()
    {
        throw new System.NotImplementedException();
    }

    public void UpdateState(float deltaTime)
    {
        throw new System.NotImplementedException();
    }
}
