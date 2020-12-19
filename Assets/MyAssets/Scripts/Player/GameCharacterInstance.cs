using UnityEngine;
using ECM.Controllers;
using ECM.Components;

public class GameCharacterInstance : MonoBehaviour
{
    /// <summary>
    /// 캐릭터들이 가지고 있는 애니메이터 인스턴스.
    /// </summary>
    public Animator AnimatorInstance { get; private set; }
    /// <summary>
    /// Query 캐릭터들이 가지고 있는 메카님 컨트롤러. ( 다른 캐릭터들은 없다. )
    /// </summary>
    public QuerySDMecanimController QueryMecanimController { get; private set; }
    public BoxCollider BoxColliderInstance { get; private set; }
    public Rigidbody RigidBodyInstance { get; private set; }
    public CharacterMovement ECM_CharacterMovmentComp { get; private set; }
    public GroundDetection ECM_GroundDetectionComp { get; private set; }
    public BaseFirstPersonController ECM_BaseCharController { get; private set; }
    public MouseLook ECM_MouseLookComp { get; private set; }

    public bool bContactGround
    {
        get { return ECM_GroundDetectionComp.isOnGround; }
    }
    public bool bContactWater { get; private set; } = false;

    public SubWorld ContainedWorld { get; private set; }

    public void Init()
    {
        QueryMecanimController = gameObject.GetComponent<QuerySDMecanimController>();
        BoxColliderInstance = gameObject.GetComponent<BoxCollider>();
        AnimatorInstance = gameObject.GetComponentInChildren<Animator>();
        RigidBodyInstance = gameObject.GetComponent<Rigidbody>();

        ECM_GroundDetectionComp = gameObject.GetComponent<GroundDetection>();
        ECM_CharacterMovmentComp = gameObject.GetComponent<CharacterMovement>();
        ECM_BaseCharController = gameObject.GetComponent<BaseFirstPersonController>();
        ECM_MouseLookComp = gameObject.GetComponent<MouseLook>();

        if (QueryMecanimController == null) KojeomLogger.DebugLog(string.Format("QueryMecanimController nullptr!"), LOG_TYPE.ERROR);
        if (BoxColliderInstance == null) KojeomLogger.DebugLog(string.Format("BoxColliderInstance nullptr!"), LOG_TYPE.ERROR);
        if (AnimatorInstance == null) KojeomLogger.DebugLog(string.Format("AnimatorInstance nullptr!"), LOG_TYPE.ERROR);
        if (RigidBodyInstance == null) KojeomLogger.DebugLog(string.Format("RigidBodyInstance nullptr!"), LOG_TYPE.ERROR);

        if (ECM_CharacterMovmentComp == null) KojeomLogger.DebugLog(string.Format("ECM_CharacterMovmentComp nullptr!"), LOG_TYPE.ERROR);
        if (ECM_GroundDetectionComp == null) KojeomLogger.DebugLog(string.Format("ECM_GroundDetectionComp nullptr!"), LOG_TYPE.ERROR);
        if (ECM_BaseCharController == null) KojeomLogger.DebugLog(string.Format("ECM_BaseCharController nullptr!"), LOG_TYPE.ERROR);
        if (ECM_MouseLookComp == null) KojeomLogger.DebugLog(string.Format("ECM_MouseLookComp nullptr!"), LOG_TYPE.ERROR);

        ECM_GroundDetectionComp.groundMask = LayerMask.GetMask(new string[] { "WaterChunk", "EnviromentChunk", "TerrainChunk"});
        ECM_CharacterMovmentComp.snapToGround = false;

        EnableComponents(false);
    }

    public void TriggerActive(bool bActive)
    {
        gameObject.SetActive(bActive);
    }

    public void EnableComponents(bool bEnable)
    {
        if (QueryMecanimController != null) QueryMecanimController.enabled = bEnable;
        if (BoxColliderInstance != null) BoxColliderInstance.enabled = bEnable;
        if (AnimatorInstance != null) AnimatorInstance.enabled = bEnable;
        if (RigidBodyInstance != null) RigidBodyInstance.useGravity = bEnable;
        if (ECM_CharacterMovmentComp == null) ECM_CharacterMovmentComp.enabled = bEnable;
        if (ECM_GroundDetectionComp == null) ECM_GroundDetectionComp.enabled = bEnable;
        if (ECM_BaseCharController == null)  ECM_BaseCharController.enabled = bEnable;
        if (ECM_MouseLookComp == null) ECM_MouseLookComp.enabled = bEnable;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.WATER_CHUNK_LAYER) == true)
        {
            bContactWater = true;
        }
        AChunk chunkComp = collision.gameObject.GetComponent<AChunk>();
        if (chunkComp != null)
        {
            SubWorld world = collision.gameObject.GetComponent<AChunk>().SubWorldInstance;
            ContainedWorld = world;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.WATER_CHUNK_LAYER) == true)
        {
            bContactWater = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(GameObjectLayerDefines.WATER_CHUNK_LAYER) == true)
        {
            bContactWater = true;
        }
        AChunk chunkComp = collision.gameObject.GetComponent<AChunk>();
        if (chunkComp != null)
        {
            SubWorld world = collision.gameObject.GetComponent<AChunk>().SubWorldInstance;
            ContainedWorld = world;
        }
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
