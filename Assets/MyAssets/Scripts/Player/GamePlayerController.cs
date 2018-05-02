using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ref # 1 : http://wiki.unity3d.com/index.php/SmoothMouseLook

public class GamePlayerController : MonoBehaviour {

    private Camera playerCamera;
    private GameCharacter playerGameCharacter;
    //
    #region cam_option
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
    #endregion
    [Range(3.5f, 15.5f)]
    public float moveSpeed;
    private IEnumerator controllProcess;

    private PlayerMoveState moveState;
    private PlayerIdleState idleState;
    private PlayerStateController stateController;

    public void Init(Camera mainCam, GameCharacter gameChar)
    {
        //
        playerGameCharacter = gameChar;
        playerCamera = mainCam;
        //
        camOrigRotation = playerCamera.transform.localRotation;
        playerOrigRotation = playerGameCharacter.transform.localRotation;
        controllProcess = ControllProcess();
        //
        moveState = new PlayerMoveState(playerGameCharacter);
        idleState = new PlayerIdleState(playerGameCharacter);
        stateController = new PlayerStateController();
        stateController.SetState(idleState);
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
        Vector3 playerPos = playerGameCharacter.transform.position;
        playerPos.y += 2.0f;
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
        playerGameCharacter.transform.localRotation = playerOrigRotation * xQuaternion;
    }

    private IEnumerator ControllProcess()
    {
        while (true)
        {
            CamFollowPlayer();
            SimpleGravityForce();
            if (UIPopupManager.isAllpopupClose)
			{
                switch (InputManager.singleton.GetInputState())
                {
                    case INPUT_STATE.CHARACTER_MOVE:
                        stateController.SetState(moveState);
                        break;
                    case INPUT_STATE.CHARACTER_IDLE:
                        stateController.SetState(idleState);
                        break;
                }
                stateController.UpdateState();
				RotationCamAndPlayer();
			}
            yield return null;
        }
    }

    private void SimpleGravityForce()
    {
        World containWorld = WorldManager.instance.ContainedWorld(playerGameCharacter.transform.position);
        CollideInfo collideInfo = containWorld.customOctree.Collide(playerGameCharacter.GetCustomAABB());
        if (!collideInfo.isCollide)
        {
            playerGameCharacter.transform.position = new Vector3(playerGameCharacter.transform.position.x,
                playerGameCharacter.transform.position.y - 0.1f,
                playerGameCharacter.transform.position.z);
        }
    }
}
