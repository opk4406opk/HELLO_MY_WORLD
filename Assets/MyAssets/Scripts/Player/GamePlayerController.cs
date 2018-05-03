using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ref # 1 : http://wiki.unity3d.com/index.php/SmoothMouseLook

public class GamePlayerController : MonoBehaviour {

    private Camera playerCamera;
    private GamePlayer gamePlayer;
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

    public void Init(Camera mainCam, GamePlayer player)
    {
        //
        gamePlayer = player;
        playerCamera = mainCam;
        //
        camOrigRotation = playerCamera.transform.localRotation;
        // 게임 플레이어가 아닌, 하위 오브젝트인 캐릭터 오브젝트의 방향을 변경해야한다.
        playerOrigRotation = gamePlayer.transform.localRotation;
        controllProcess = ControllProcess();
        //
        moveState = new PlayerMoveState(gamePlayer);
        idleState = new PlayerIdleState(gamePlayer);
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
        Vector3 playerPos = gamePlayer.transform.position;
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
        gamePlayer.transform.localRotation = playerOrigRotation * xQuaternion;
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
        World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
        CollideInfo collideInfo = containWorld.customOctree.Collide(gamePlayer.charInstance.GetCustomAABB());
        if (!collideInfo.isCollide)
        {
            gamePlayer.transform.position = new Vector3(gamePlayer.transform.position.x,
                gamePlayer.transform.position.y - 0.1f,
                gamePlayer.transform.position.z);
        }
        else
        {
            KojeomLogger.DebugLog(string.Format("캐릭터가 밟고 서있는 Node center : {0}", collideInfo.hitBlockCenter));
        }
    }
}
