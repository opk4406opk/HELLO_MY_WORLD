using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ref # 1 : http://wiki.unity3d.com/index.php/SmoothMouseLook


public class PlayerController : MonoBehaviour {

    private Camera playerCamera;
    private GameObject player;
    //
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

    [Range(3.5f, 15.5f)]
    public float moveSpeed;

    private IEnumerator controllProcess;

    private QuerySDMecanimController aniController;

    public void Init(Camera mainCam, GameObject playerObject)
    {
        //
        player = playerObject;
        playerCamera = mainCam;
        //
        camOrigRotation = playerCamera.transform.localRotation;
        playerOrigRotation = player.transform.localRotation;
        controllProcess = ControllProcess();
        aniController = player.GetComponent<GamePlayer>().charInstance.GetAniController();
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
        Vector3 playerPos = player.transform.position;
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
        player.transform.localRotation = playerOrigRotation * xQuaternion;
    }

	private void Move()
	{
        aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
        Vector3 dir;
		if (Input.GetKey(KeyCode.W))
		{
            dir = player.transform.forward;
			Vector3 newPos = player.transform.position;
            player.transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.S))
		{
			dir = -player.transform.forward;
			Vector3 newPos = player.transform.position;
            player.transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			dir = player.transform.right;
			Vector3 newPos = player.transform.position;
            player.transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
		}
		else if (Input.GetKey(KeyCode.A))
		{
			dir = -player.transform.right;
			Vector3 newPos = player.transform.position;
            player.transform.position = newPos + (dir * moveSpeed * Time.deltaTime);
        }
        else
        {
            // 움직임이 없는 상태.
            aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.FUKUOKA_DANCE_1);
        }
	}
    
    private IEnumerator ControllProcess()
    {
        while (true)
        {
            CamFollowPlayer();
            SimpleGravityForce();
            if (UIPopupManager.isAllpopupClose)
			{
				RotationCamAndPlayer();
				Move();
			}
            yield return null;
        }
    }

    private void SimpleGravityForce()
    {
        World containWorld = WorldManager.instance.ContainedWorld(player.transform.position);
        CollideInfo collideInfo = containWorld.customOctree.Collide(player.transform.position);
        if (!collideInfo.isCollide)
        {
            player.transform.position = new Vector3(player.transform.position.x,
                player.transform.position.y - 0.1f,
                player.transform.position.z);
        }
    }

}
