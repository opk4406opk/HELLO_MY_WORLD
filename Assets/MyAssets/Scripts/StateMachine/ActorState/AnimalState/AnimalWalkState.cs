﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalWalkState : AActorState, IState
{
    public AnimalWalkState(Actor instance)
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
