﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRunState : AActorState, IState
{

    public NPCRunState(Actor instance)
    {
        ActorInstance = instance;
    }

    public void InitState()
    {
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
