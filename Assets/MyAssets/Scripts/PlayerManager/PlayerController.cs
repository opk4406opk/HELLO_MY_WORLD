using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ref # 1 : http://wiki.unity3d.com/index.php/SmoothMouseLook


public class PlayerController : MonoBehaviour {

    private Camera playerCamera;
    private List<float> camRotArrayX = new List<float>();
    private List<float> camRotArrayY = new List<float>();
    private float camRotationX = 0F;
    private float camRotationY = 0F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    private int frameCounter = 20;
    [Range(1.0f, 100.0f)]
    public float camSensitivityY = 1.0f;
    [Range(1.0f, 100.0f)]
    public float camSensitivityX = 1.0f;
    private Quaternion camOrigRotation;
    private Quaternion playerOrigRotation;

    public void Init(Camera mainCam)
    {
        playerCamera = mainCam;
        camOrigRotation = playerCamera.transform.localRotation;
        playerOrigRotation = gameObject.transform.localRotation;
        StartCoroutine(ControllProcess());
    }

    private void CamFollowPlayer()
    {
        Vector3 playerPos =  gameObject.transform.position;
        playerPos.y += 5.0f;
        playerCamera.transform.position = playerPos;
    }

    private void RotationCamAndPlayer()
    {
        float camRotAverageY = 0f;
        float camRotAverageX = 0f;

        camRotationY += Input.GetAxis("Mouse Y") * camSensitivityY;
        camRotationX += Input.GetAxis("Mouse X") * camSensitivityX;

        camRotArrayY.Add(camRotationY);
        camRotArrayX.Add(camRotationX);

        if (camRotArrayY.Count >= frameCounter)
        {
            camRotArrayY.RemoveAt(0);
        }
        if (camRotArrayX.Count >= frameCounter)
        {
            camRotArrayX.RemoveAt(0);
        }

        for (int j = 0; j < camRotArrayY.Count; j++)
        {
            camRotAverageY += camRotArrayY[j];
        }
        for (int i = 0; i < camRotArrayX.Count; i++)
        {
            camRotAverageX += camRotArrayX[i];
        }

        camRotAverageY /= camRotArrayY.Count;
        camRotAverageX /= camRotArrayX.Count;

        camRotAverageY = Mathf.Clamp(camRotAverageY, minimumY, maximumY);
        camRotAverageX = Mathf.Clamp(camRotAverageX, minimumX, maximumX);

        Quaternion yQuaternion = Quaternion.AngleAxis(camRotAverageY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(camRotAverageX, Vector3.up);

        // rot cam
        playerCamera.transform.localRotation = camOrigRotation * xQuaternion * yQuaternion;
        // rot player
        gameObject.transform.localRotation = playerOrigRotation * xQuaternion;
    }
    
    private IEnumerator ControllProcess()
    {
        while (true)
        {
            CamFollowPlayer();
            RotationCamAndPlayer();
            yield return null;
        }
    }
}
