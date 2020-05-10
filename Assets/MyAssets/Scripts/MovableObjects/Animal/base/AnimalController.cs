using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : ActorController
{

    public override void Init(SubWorld world, Actor instance)
    {
        PreInit();
        ActorInstance = instance;
        ContainedWorld = world;
        // AI 초기화 세팅.
        AIGroup[(int)AITypes.Common] = new CommonAnimalAI();
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
        StateMachineControllerInstance.SetState(new AnimalRunState(ActorInstance, targetPosition));
    }

    public override void StartIdle()
    {
        CurStateType = ActorStateType.Idle;
        StateMachineControllerInstance.SetState(new AnimalIdleState(ActorInstance));
    }

    public override void StartWalking(Vector3 targetPosition)
    {
        CurStateType = ActorStateType.Walk;
        StateMachineControllerInstance.SetState(new AnimalWalkState(ActorInstance, targetPosition));
    }
}
