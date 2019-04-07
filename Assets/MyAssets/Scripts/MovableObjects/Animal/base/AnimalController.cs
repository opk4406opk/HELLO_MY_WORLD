using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalController : ActorController
{
    //states.
    private AnimalIdleState IdleState;
    private AnimalWalkState WalkState;
    private AnimalRunState RunState;

    public override void Init(World world, Actor instance)
    {
        CurStateType = ActorStateType.Idle;
        ContainedWorld = world;
        AnimatorInstance = gameObject.GetComponent<Animator>();
        BoxColliderInstance = gameObject.GetComponent<BoxCollider>();

        IdleState = new AnimalIdleState(instance);
        WalkState = new AnimalWalkState(instance);
        RunState = new AnimalRunState(instance);
    }

    public override void LookAt(Vector3 dir)
    {
    }

    public override void Move(Vector3 dir, float speed)
    {
    }

    public override void StartController()
    {
    }

    public override void StopController()
    {
    }

    public override void Tick(float deltaTime)
    {
        switch (CurStateType)
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
        StateMachineControllerInstance.Tick(deltaTime);
    }

    public override Transform GetActorTransform()
    {
        return transform;
    }
}
