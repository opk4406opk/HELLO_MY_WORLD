using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : ActorController
{
    public override void Init(SubWorld world, Actor instance)
    {
        ActorInstance = instance;
        ContainedWorld = world;
        AnimatorInstance = gameObject.GetComponent<Animator>();
        BoxColliderInstance = gameObject.GetComponent<BoxCollider>();
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
        StateMachineControllerInstance.SetState(new NPCRunState(ActorInstance, targetPosition));
    }

    public override void StartIdle()
    {
        StateMachineControllerInstance.SetState(new NPCIdleState(ActorInstance));
    }

    public override void StartWalking(Vector3 targetPosition)
    {
        StateMachineControllerInstance.SetState(new NPCWalkState(ActorInstance, targetPosition));
    }
}
