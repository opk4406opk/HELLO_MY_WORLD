using UnityEngine;

public class GameCharacterInstance : MonoBehaviour
{

    public Animator AnimatorInstance { get; private set; }
    public QuerySDMecanimController QueryMecanimController { get; private set; }
    public BoxCollider BoxColliderInstance { get; private set; }
    public Rigidbody RigidBodyInstance { get; private set; }

    public void Init()
    {
        QueryMecanimController = gameObject.GetComponent<QuerySDMecanimController>();
        BoxColliderInstance = gameObject.GetComponent<BoxCollider>();
        AnimatorInstance = gameObject.GetComponentInChildren<Animator>();
        RigidBodyInstance = gameObject.GetComponent<Rigidbody>();

        if (QueryMecanimController == null) KojeomLogger.DebugLog(string.Format("QueryMecanimController nullptr!"), LOG_TYPE.ERROR);
        if (BoxColliderInstance == null) KojeomLogger.DebugLog(string.Format("BoxColliderInstance nullptr!"), LOG_TYPE.ERROR);
        if (AnimatorInstance == null) KojeomLogger.DebugLog(string.Format("AnimatorInstance nullptr!"), LOG_TYPE.ERROR);
        if(RigidBodyInstance == null) KojeomLogger.DebugLog(string.Format("RigidBodyInstance nullptr!"), LOG_TYPE.ERROR);

        //
        RigidBodyInstance.useGravity = false;
    }

    public CustomAABB GetCustomAABB(Vector3 moveSpeed)
    {
        CustomAABB aabb = new CustomAABB();
        aabb.MakeAABB(BoxColliderInstance, moveSpeed.x, moveSpeed.y, moveSpeed.z);
        return aabb;
    }

    public CustomAABB GetCustomAABB()
    {
        CustomAABB aabb = new CustomAABB();
        aabb.MakeAABB(BoxColliderInstance);
        return aabb;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
