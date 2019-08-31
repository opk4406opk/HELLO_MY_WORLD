using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : ActorController
{
    //states.
    private AnimalIdleState IdleState;
    private AnimalWalkState WalkState;
    private AnimalRunState RunState;

    public override void Init(SubWorld world, Actor instance)
    {
        ContainedWorld = world;
        AnimatorInstance = gameObject.GetComponent<Animator>();
        BoxColliderInstance = gameObject.GetComponent<BoxCollider>();

        IdleState = new AnimalIdleState(instance);
        WalkState = new AnimalWalkState(instance);
        RunState = new AnimalRunState(instance);
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

    public override void ChangeActorState(ActorStateType state)
    {
        CurStateType = state;
        switch (state)
        {
            case ActorStateType.Idle:
                StateMachineControllerInstance.SetState(IdleState);
                break;
            case ActorStateType.Walk:
                StateMachineControllerInstance.SetState(WalkState);
                break;
            case ActorStateType.Run:
                StateMachineControllerInstance.SetState(RunState);
                break;
        }
    }
}
