using UnityEngine;

public class GameCharacterInstance : MonoBehaviour {
    [SerializeField]
    private Animator characterAnimator;
    [SerializeField]
    public QuerySDMecanimController queryMecanimController { get; private set; }
    [SerializeField]
    private BoxCollider boxCollider;
    public void Init()
    {
        queryMecanimController = gameObject.GetComponent<QuerySDMecanimController>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
        characterAnimator = gameObject.GetComponentInChildren<Animator>();

        if (queryMecanimController == null) KojeomLogger.DebugLog(string.Format("queryMecanimController nullptr!"), LOG_TYPE.ERROR);
        if (boxCollider == null) KojeomLogger.DebugLog(string.Format("boxCollider nullptr!"), LOG_TYPE.ERROR);
        if (characterAnimator == null) KojeomLogger.DebugLog(string.Format("characterAnimator nullptr!"), LOG_TYPE.ERROR);
    }

    public Animator GetAnimator()
    {
        return characterAnimator;
    }

    public BoxCollider GetBoxCollider()
    {
        return boxCollider;
    }

    public CustomAABB GetCustomAABB(Vector3 moveSpeed)
    {
        CustomAABB aabb = new CustomAABB();
        aabb.MakeAABB(boxCollider, moveSpeed.x, moveSpeed.y, moveSpeed.z);
        return aabb;
    }

    public CustomAABB GetCustomAABB()
    {
        CustomAABB aabb = new CustomAABB();
        aabb.MakeAABB(boxCollider);
        return aabb;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
