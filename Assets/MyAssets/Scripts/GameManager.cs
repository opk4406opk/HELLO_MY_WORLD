using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    [SerializeField]
    private World worldA;
    [SerializeField]
    private World worldB;
    // Use this for initialization
    void Start ()
    {
        worldA.Init(0, 0);
        worldB.Init(4, 0);
    }
	
}
