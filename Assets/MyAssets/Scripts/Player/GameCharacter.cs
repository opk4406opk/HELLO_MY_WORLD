using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameCharacter : MonoBehaviour {
    private QuerySDMecanimController aniController;
    public void Init()
    {
        aniController = gameObject.GetComponent<QuerySDMecanimController>();
    }

    public QuerySDMecanimController GetAniController()
    {
        return aniController;
    }
}
