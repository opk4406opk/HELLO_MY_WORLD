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
        CurStateType = ActorStateType.Idle;
        ContainedWorld = world;
        AnimatorInstance = gameObject.GetComponent<Animator>();
        BoxColliderInstance = gameObject.GetComponent<BoxCollider>();

        IdleState = new NPCIdleState(instance);
        WalkState = new NPCWalkState(instance);
        RunState = new NPCRunState(instance);
    }

    public override void Tick(float deltaTime)
    {
        //
        switch(CurStateType)
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

    public override void Move(Vector3 dir, float speed)
    {
        Vector3 newPos = gameObject.transform.position;
        newPos += dir.normalized * Time.deltaTime * speed;
        gameObject.transform.position = newPos;
    }

    public override void StartController()
    {
    }

    public override void StopController()
    {
    }

    public override void LookAt(Vector3 dir)
    {
        float theta = Vector3.Dot(dir, transform.forward) / (transform.forward.magnitude * dir.magnitude);
        float angle = Mathf.Acos(theta) * Mathf.Rad2Deg;
        Vector3 cross = Vector3.Cross(dir, transform.forward);
        if (cross.z < 0.0f) angle = 360.0f - angle;
        transform.Rotate(transform.up, angle);
    }

    public override Transform GetActorTransform()
    {
        return transform;
    }

}
