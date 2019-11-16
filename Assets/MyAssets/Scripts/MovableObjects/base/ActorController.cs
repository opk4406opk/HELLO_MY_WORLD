using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimTypeString
{
    public static readonly string Walking = "Walking";
    public static readonly string Running = "Running";
    public static readonly string Jumping = "Jumping";
    public static readonly string Standing = "Standing";
}

public abstract class ActorController : MonoBehaviour
{
    abstract public void Init(SubWorld world, Actor instance);
    abstract public Transform GetActorTransform();
    abstract public void Tick(float deltaTime);
    abstract public void ChangeActorState(ActorStateType state);

    protected ActorStateType CurStateType;
    protected AITypes CurAIType = AITypes.Common;
    protected BoxCollider BoxColliderInstance;
    protected StateMachineController StateMachineControllerInstance = new StateMachineController();
    protected SubWorld ContainedWorld;
    protected Animator AnimatorInstance;
    protected BehaviorTree[] AIGroup = new BehaviorTree[(int)AITypes.COUNT];
    protected bool bContactTerrain = false;
    protected bool bContactEnvrioment = false;
    protected bool bContactActor = false;
    protected bool bContactWater = false;

    public void PlayAnimation(string animName)
    {
        if(AnimatorInstance != null)
        {
            AnimatorInstance.Play(animName);
        }
    }

    public ActorStateType GetCurrentState()
    {
        return CurStateType;
    }

    public bool IsContactTerrain()
    {
        return bContactTerrain;
    }
    public bool IsContactEnvrioment()
    {
        return bContactEnvrioment;
    }
    public bool IsContactActor()
    {
        return bContactActor;
    }

    public bool IsContactWater()
    {
        return bContactWater;
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.TERRAIN_CHUNK_LAYER))
        {
            bContactTerrain = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ENVIROMENT_CHUNK_LAYER))
        {
            bContactEnvrioment = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.WATER_CHUNK_LAYER))
        {
            bContactWater = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_NPC_LAYER) ||
                 collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_ANIMAL_LAYER) ||
                 collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_MONSTER_LAYER))
        {
            bContactActor = true;
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.TERRAIN_CHUNK_LAYER))
        {
            bContactTerrain = false;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ENVIROMENT_CHUNK_LAYER))
        {
            bContactEnvrioment = false;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.WATER_CHUNK_LAYER))
        {
            bContactWater = false;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_NPC_LAYER) ||
                 collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_ANIMAL_LAYER) ||
                 collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_MONSTER_LAYER))
        {
            bContactActor = false;
        }
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.TERRAIN_CHUNK_LAYER))
        {
            bContactTerrain = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ENVIROMENT_CHUNK_LAYER))
        {
            bContactEnvrioment = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.WATER_CHUNK_LAYER))
        {
            bContactWater = true;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_NPC_LAYER) ||
                 collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_ANIMAL_LAYER) ||
                 collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.ACTOR_MONSTER_LAYER))
        {
            bContactActor = true;
        }
    }
    public void Teleport(Vector3 toPosition)
    {
        gameObject.transform.position = toPosition;
    }
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

    public Vector3 GetContainedSubWorldOffset()
    {
        return ContainedWorld.OffsetCoordinate;
    }

    public Vector3 GetContainedWorldAreaOffset()
    {
        return ContainedWorld.GetWorldAreaOffset();
    }

    public void ChangeAI(AITypes type)
    {
        AIGroup[(int)CurAIType].StopBT();
        if (AIGroup[(int)type] != null)
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
}
