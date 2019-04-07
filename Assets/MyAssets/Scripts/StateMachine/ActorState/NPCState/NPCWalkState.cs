using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCWalkState : AActorState, IState
{
    public NPCWalkState(Actor instance)
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
