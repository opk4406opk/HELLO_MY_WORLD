﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : ActorController
{
    public override void Init(SubWorld world, Actor instance)
    {
        PreInit();
        ActorInstance = instance;
        ContainedWorld = world;
        // AI 초기화 세팅.
        AIGroup[(int)AITypes.Common] = new CommonNpcAI();
        AIGroup[(int)AITypes.Common].Initialize(this);
    }

    public override void Tick(float deltaTime)
    {
        StateMachineControllerInstance.Tick(deltaTime);
    }

    public override Transform GetActorTransform()
    {
        return transform;
    }

    public override void StartRun(Vector3 targetPosition)
    {
        CurStateType = ActorStateType.Run;
        StateMachineControllerInstance.ChangeState(new NPCRunState(ActorInstance, targetPosition));
    }

    public override void StartIdle()
    {
        CurStateType = ActorStateType.Idle;
        StateMachineControllerInstance.ChangeState(new NPCIdleState(ActorInstance));
    }

    public override void StartWalking(Vector3 targetPosition)
    {
        CurStateType = ActorStateType.Walk;
        StateMachineControllerInstance.ChangeState(new NPCWalkState(ActorInstance, targetPosition));
    }
}
