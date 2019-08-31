using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalRunState : AActorState, IState
{
    public AnimalRunState(Actor instance)
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
