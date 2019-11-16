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
        ActorInstance.GetController().PlayAnimation(ActorAnimTypeString.Standing);
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
    }
}
