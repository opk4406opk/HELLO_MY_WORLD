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