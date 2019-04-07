using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorController : MonoBehaviour
{
    abstract public void Move(Vector3 dir, float speed);
    abstract public void LookAt(Vector3 dir);
    abstract public void Init(World world, Actor instance);
    abstract public void StartController();
    abstract public void StopController();
    abstract public Transform GetActorTransform();
    abstract public void Tick(float deltaTime);

    protected ActorStateType CurStateType;
    protected BoxCollider BoxColliderInstance;
    protected StateMachineController StateMachineControllerInstance = new StateMachineController();
    protected World ContainedWorld;
    protected Animator AnimatorInstance;


    public void SetActorStateType(ActorStateType state)
    {
        CurStateType = state;
    }
}
