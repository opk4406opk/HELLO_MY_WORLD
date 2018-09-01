using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameCharacter : MonoBehaviour {
    [SerializeField]
    private Animator characterAnimator;
    [SerializeField]
    private QuerySDMecanimController queryMecanimController;
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

    public QuerySDMecanimController GetQueryMecanimController()
    {
        return queryMecanimController;
    }

    public BoxCollider GetBoxCollider()
    {
        return boxCollider;
    }

    public CustomAABB GetCustomAABB()
    {
        CustomAABB aabb = new CustomAABB();
        aabb.MakeAABB(boxCollider);
        return aabb;
    }
}
