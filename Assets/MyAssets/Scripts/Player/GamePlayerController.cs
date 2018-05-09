using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ref # 1 : http://wiki.unity3d.com/index.php/SmoothMouseLook

public enum GAMEPLAYER_STATE
{
    move = 0,
    idle = 1,
    jump = 2,
    moving_jump = 3
}
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
    private PlayerJumpState jumpState;
    private PlayerStateController moveStateController;
    private PlayerStateController jumpStateController;
    private PlayerStateController poseStateController;
    private GAMEPLAYER_STATE curPlayerState;

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
        jumpState = new PlayerJumpState(gamePlayer);
        moveStateController = new PlayerStateController();
        jumpStateController = new PlayerStateController();
        poseStateController = new PlayerStateController();
        curPlayerState = GAMEPLAYER_STATE.idle;
    }

    public void StartControllProcess()
    {
        StartCoroutine(controllProcess);
    }
    public void StopControllProcess()
    {
        StopCoroutine(controllProcess);
    }

    public void SetPlayerState(GAMEPLAYER_STATE state)
    {
        curPlayerState = state;
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
            if (UIPopupSupervisor.isAllpopupClose)
			{
                var inputData = InputManager.singleton.GetInputData();
                if (inputData.state == INPUT_STATE.CHARACTER_MOVE)
                {
                    curPlayerState = GAMEPLAYER_STATE.move;
                }
                else if(inputData.state == INPUT_STATE.NONE)
                {
                    curPlayerState = GAMEPLAYER_STATE.idle;
                }

                var overlappedInputs = InputManager.singleton.GetOverlappedInputData();
                if(overlappedInputs.Count > 0)
                {
                    var input = overlappedInputs.Dequeue();
                    if(input.state == INPUT_STATE.CHARACTER_JUMP && curPlayerState == GAMEPLAYER_STATE.move)
                    {
                        curPlayerState = GAMEPLAYER_STATE.moving_jump;
                    }
                    else if (input.state == INPUT_STATE.CHARACTER_JUMP && curPlayerState != GAMEPLAYER_STATE.move)
                    {
                        curPlayerState = GAMEPLAYER_STATE.jump;
                    }
                }
                KojeomLogger.DebugLog(string.Format("curPlayerState : {0}", curPlayerState));

                switch (curPlayerState)
                {
                    case GAMEPLAYER_STATE.move:
                        moveStateController.SetState(moveState);
                        moveStateController.UpdateState();
                        break;
                    case GAMEPLAYER_STATE.jump:
                        jumpStateController.SetState(jumpState);
                        jumpStateController.UpdateState();
                        break;
                    case GAMEPLAYER_STATE.idle:
                        poseStateController.SetState(idleState);
                        poseStateController.UpdateState();
                        break;
                    case GAMEPLAYER_STATE.moving_jump:
                        jumpStateController.SetState(jumpState);
                        jumpStateController.UpdateState();
                        moveStateController.SetState(moveState);
                        moveStateController.UpdateState();
                        break;
                }

                RotationCamAndPlayer();
			}
            yield return null;
        }
    }

    private void SimpleGravityForce()
    {
        World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
        Vector3 bottomOffsetedPos = gamePlayer.transform.position;
        bottomOffsetedPos -= new Vector3(0.0f, 0.1f, 0.0f);

        CollideInfo collideInfo = containWorld.customOctree.Collide(bottomOffsetedPos);
        var collidedBlock = collideInfo.GetBlock();
        if (!collideInfo.isCollide)
        {
            gamePlayer.transform.position = new Vector3(gamePlayer.transform.position.x,
                gamePlayer.transform.position.y - 0.1f,
                gamePlayer.transform.position.z);
        }
        else if(collidedBlock.type != (byte)TileType.NONE)
        {
            KojeomCoroutineHelper.singleton.ReleaseRoutine("Jump");
        }
    }
}
