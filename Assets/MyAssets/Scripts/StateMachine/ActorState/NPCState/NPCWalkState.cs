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
        ActorInstance.GetController().PlayAnimation(ActorAnimTypeString.Walking);
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
    }
}
