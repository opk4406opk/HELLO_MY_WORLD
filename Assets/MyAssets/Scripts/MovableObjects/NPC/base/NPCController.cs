using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : ActorController
{
    //states.
    private NPCIdleState IdleState;
    private NPCWalkState WalkState;
    private NPCRunState RunState;

    public override void Init(World world, Actor instance)
    {
        ContainedWorld = world;
        AnimatorInstance = gameObject.GetComponent<Animator>();
        BoxColliderInstance = gameObject.GetComponent<BoxCollider>();

        IdleState = new NPCIdleState(instance);
        WalkState = new NPCWalkState(instance);
        RunState = new NPCRunState(instance);
    }

    public override void Tick(float deltaTime)
    {
        StateMachineControllerInstance.Tick(deltaTime);
    }

    public override void StartController()
    {
    }

    public override void StopController()
    {
    }

    public override Transform GetActorTransform()
    {
        return transform;
    }
    public override void ChangeActorState(ActorStateType state)
    {
        CurStateType = state;
        switch(state)
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
