using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TEST_Experiment : MonoBehaviour
{
    // Start is called before the first frame update

    public Transform GamePlayer;
    public Transform[] Enemyies;
    public Transform EnemyCenterTarget;

    public GameObject MainCameraInstance;
    //
    public float CamFOV = 60.0f;
    public Vector3 DeltaCamPosition;
    public Vector3 DeltaCamRotation;

    void Start()
    {
        InitMainCamera();
    }

    // Update is called once per frame
    void Update()
    {
        RotationMainCamera();
    }

    private void InitMainCamera()
    {
        MainCameraInstance.transform.position = GamePlayer.position + DeltaCamPosition;
    }

    private void RotationMainCamera()
    {
        Vector3 dir = EnemyCenterTarget.position - GamePlayer.position;
        dir.Normalize();
        //
        MainCameraInstance.transform.rotation.SetLookRotation(dir + DeltaCamRotation);
        MainCameraInstance.transform.SetPositionAndRotation(MainCameraInstance.transform.position, Quaternion.FromToRotation(GamePlayer.forward, dir) * Quaternion.Euler(DeltaCamRotation));
        MainCameraInstance.GetComponent<Camera>().fieldOfView = CamFOV;
    }
}
