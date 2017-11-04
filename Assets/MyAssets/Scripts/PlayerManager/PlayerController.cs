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

	[Range(1.5f, 3.5f)]
	public float moveSpeed = 1.5f;

    private IEnumerator controllProcess;

    public void Init(Camera mainCam)
    {
        playerCamera = mainCam;
        camOrigRotation = playerCamera.transform.localRotation;
        playerOrigRotation = gameObject.transform.localRotation;
        controllProcess = ControllProcess();
    }

    public void StartControllProcess()
    {
        StartCoroutine(controllProcess);
    }
    public void StopControllProcess()
    {
        StopCoroutine(controllProcess);
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

	private void MovePlayer()
	{
		Vector3 dir;
		if (Input.GetKeyDown(KeyCode.W))
		{
			dir = transform.forward;
			Vector3 newPos = transform.position;
			transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
		}
		else if (Input.GetKeyDown(KeyCode.S))
		{
			dir = -transform.forward;
			Vector3 newPos = transform.position;
			transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
		}
		else if (Input.GetKeyDown(KeyCode.A))
		{
			dir = transform.right;
			Vector3 newPos = transform.position;
			transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
		}
		else if (Input.GetKeyDown(KeyCode.D))
		{
			dir = -transform.right;
			Vector3 newPos = transform.position;
			transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
		}
	}
    
    private IEnumerator ControllProcess()
    {
        while (true)
        {
            CamFollowPlayer();
			if (UIPopupManager.isAllpopupClose)
			{
				RotationCamAndPlayer();
				MovePlayer();
			}
            yield return null;
        }
    }

}
