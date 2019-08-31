using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalIdleState : AActorState, IState
{
    public AnimalIdleState(Actor instance)
    {
        ActorInstance = instance;
    }

    public void InitState()
    {
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
    }
}
