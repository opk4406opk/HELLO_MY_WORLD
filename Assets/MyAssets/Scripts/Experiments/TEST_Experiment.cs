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

    public float SkySpinSpeed = 3.0f;

    public GameObject InverseNormalTarget;

    void Start()
    {
        InitMainCamera();
        if(InverseNormalTarget != null)
        {
            InverseNormal();
        }
    }

    // Update is called once per frame
    void Update()
    {
        RotationMainCamera();
        SpiningSky();
    }

    private void InitMainCamera()
    {
        MainCameraInstance.transform.position = GamePlayer.position + DeltaCamPosition;
    }

    private void SpiningSky()
    {
        InverseNormalTarget.transform.Rotate(Vector3.up, SkySpinSpeed * Time.deltaTime);
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

    private void InverseNormal()
    {
        MeshFilter filter = InverseNormalTarget.GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;

            Vector3[] normals = mesh.normals;
            for (int i = 0; i < normals.Length; i++)
                normals[i] = -normals[i];
            mesh.normals = normals;

            for (int m = 0; m < mesh.subMeshCount; m++)
            {
                int[] triangles = mesh.GetTriangles(m);
                for (int i = 0; i < triangles.Length; i += 3)
                {
                    int temp = triangles[i + 0];
                    triangles[i + 0] = triangles[i + 1];
                    triangles[i + 1] = temp;
                }
                mesh.SetTriangles(triangles, m);
            }
        }
    }
}
