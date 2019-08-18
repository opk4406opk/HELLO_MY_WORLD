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

    public Block[,,] GetContainedWorldBlockData()
    {
        return ContainedWorld.WorldBlockData;
    }

    public Vector3 GetSubWorldOffset()
    {
        return ContainedWorld.WorldCoordinate;
    }

    public void ChangeAI(AITypes type)
    {
        AIGroup[(int)CurAIType].StopBT();
        if(AIGroup[(int)type] != null)
        {
            CurAIType = type;
            AIGroup[(int)type].StartBT();
        }
    }
    public void StartAI()
    {
        AIGroup[(int)CurAIType].StartBT();
    }
    public void StopAI()
    {
        AIGroup[(int)CurAIType].StopBT();
    }
    public void StartController()
    {

    }
    public void StopController()
    {

    }
    abstract public void Init(World world, Actor instance);
    abstract public Transform GetActorTransform();
    abstract public void Tick(float deltaTime);
    abstract public void ChangeActorState(ActorStateType state);

    protected ActorStateType CurStateType;
    protected AITypes CurAIType = AITypes.Common;
    protected BoxCollider BoxColliderInstance;
    protected StateMachineController StateMachineControllerInstance = new StateMachineController();
    protected World ContainedWorld;
    protected Animator AnimatorInstance;
    protected BehaviorTree[] AIGroup = new BehaviorTree[(int)AITypes.COUNT];
}
