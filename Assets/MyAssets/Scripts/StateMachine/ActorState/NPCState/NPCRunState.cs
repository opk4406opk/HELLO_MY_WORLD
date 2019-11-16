using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRunState : AActorState, IState
{
    private Vector3 TargetPosition;
    private Vector3 DirectionToTarget;
    public NPCRunState(Actor instance, Vector3 targetPosition)
    {
        ActorInstance = instance;
        TargetPosition = targetPosition;
        DirectionToTarget = targetPosition - ActorInstance.GetController().GetActorTransform().position;
    }

    public void InitState()
    {
        ActorInstance.GetController().PlayAnimation(ActorAnimTypeString.Running);
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
        float dist = Vector3.Distance(ActorInstance.GetController().GetActorTransform().position, TargetPosition);
        if (dist >= 1.0f)
        {
            ActorInstance.GetController().Move(DirectionToTarget, 2.0f, deltaTime);
        }
    }
}
