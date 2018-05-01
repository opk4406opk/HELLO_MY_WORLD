using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameCharacter : MonoBehaviour {
    private QuerySDMecanimController aniController;
    private BoxCollider boxCollider;
    public void Init()
    {
        aniController = gameObject.GetComponent<QuerySDMecanimController>();
        boxCollider = gameObject.GetComponent<BoxCollider>();
    }

    public QuerySDMecanimController GetAniController()
    {
        return aniController;
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
