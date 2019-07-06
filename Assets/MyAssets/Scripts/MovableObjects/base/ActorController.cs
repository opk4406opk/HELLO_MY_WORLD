using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActorController : MonoBehaviour
{
    public void Move(Vector3 dir, float speed)
    {
        Vector3 newPos = gameObject.transform.position;
        newPos += dir.normalized * Time.deltaTime * speed;
        gameObject.transform.position = newPos;
    }
    public void LookAt(Vector3 dir)
    {
        float theta = Vector3.Dot(dir, transform.forward) / (transform.forward.magnitude * dir.magnitude);
        float angle = Mathf.Acos(theta) * Mathf.Rad2Deg;
        Vector3 cross = Vector3.Cross(dir, transform.forward);
        if (cross.z < 0.0f) angle = 360.0f - angle;
        transform.Rotate(transform.up, angle);
    }
    abstract public void Init(World world, Actor instance);
    abstract public void StartController();
    abstract public void StopController();
    abstract public Transform GetActorTransform();
    abstract public void Tick(float deltaTime);
    abstract public void ChangeActorState(ActorStateType state);

    protected ActorStateType CurStateType;
    protected BoxCollider BoxColliderInstance;
    protected StateMachineController StateMachineControllerInstance = new StateMachineController();
    protected World ContainedWorld;
    protected Animator AnimatorInstance;


}
