using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ref # 1 : http://wiki.unity3d.com/index.php/SmoothMouseLook

public enum GAMEPLAYER_CHAR_STATE
{
    MOVE = 0,
    IDLE = 1,
    JUMP = 2,
    MOVING_JUMP = 3,
    NONE,
    COUNT = NONE
}
public class GamePlayerController : MonoBehaviour {

    private Camera PlayerCamera;
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
    private bool isControllProcessOn = false;

    private PlayerMoveState moveState;
    private PlayerIdleState idleState;
    private PlayerJumpState jumpState;
    private PlayerStateController moveStateController;
    private PlayerStateController jumpStateController;
    private PlayerStateController poseStateController;
    private GAMEPLAYER_CHAR_STATE curPlayerState;

    public GameCharacterInstance CharacterInstance { get; private set; }

    public void Init(Camera mainCam, GamePlayer gamePlayer)
    {
        PlayerCamera = mainCam;
        //
        camOrigRotation = PlayerCamera.transform.localRotation;
        // 게임 플레이어가 아닌, 하위 오브젝트인 캐릭터 인스턴스 방향을 변경해야한다.
        playerOrigRotation = CharacterInstance.transform.localRotation;
        //
        moveState = new PlayerMoveState(gamePlayer);
        idleState = new PlayerIdleState(gamePlayer);
        jumpState = new PlayerJumpState(gamePlayer);
        moveStateController = new PlayerStateController();
        jumpStateController = new PlayerStateController();
        poseStateController = new PlayerStateController();
        curPlayerState = GAMEPLAYER_CHAR_STATE.IDLE;
    }

    public void RegisterCharacter(GameCharacterInstance character)
    {
        CharacterInstance = character;
    }

    public Vector3 GetPosition()
    {
        return CharacterInstance.GetPosition();
    }

    public void SetPosition(Vector3 newPos)
    {
        CharacterInstance.transform.position = newPos;
    }

    public void AddPostion(Vector3 addPos)
    {
        CharacterInstance.transform.position += addPos;
    }

    public void LerpPosition(Vector3 addPos)
    {
        Vector3 newPos = Vector3.Lerp(CharacterInstance.transform.position, CharacterInstance.transform.position + addPos, Time.deltaTime);
        CharacterInstance.transform.position = newPos;
    }

    public Vector3 GetLerpValue(Vector3 addPos)
    {
        return Vector3.Lerp(CharacterInstance.transform.position, CharacterInstance.transform.position + addPos, Time.deltaTime);
    }

    public void StartControllProcess()
    {
        isControllProcessOn = true;
    }
    public void StopControllProcess()
    {
        isControllProcessOn = false;
    }

    public GAMEPLAYER_CHAR_STATE GetPlayerState()
    {
        return curPlayerState;
    }

    private void RotationCamAndPlayer()
    {
        float camRotAverageY = 0f;
        float camRotAverageX = 0f;
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            camRotationY += Input.GetAxis("Mouse Y") * camSensitivityY;
            camRotationX += Input.GetAxis("Mouse X") * camSensitivityX;
        }
        else if(Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var virtualJoystick = VirtualJoystickManager.singleton;
            if(virtualJoystick != null)
            {
                camRotationY += virtualJoystick.GetLookDirection().y * camSensitivityY;
                camRotationX += virtualJoystick.GetLookDirection().x * camSensitivityX;
            }
        }
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
        PlayerCamera.transform.localRotation = camOrigRotation * xQuaternion * yQuaternion;
        // rot player
        CharacterInstance.transform.localRotation = playerOrigRotation * xQuaternion;
    }

    private void FixedUpdate()
    {
        if (CharacterInstance == null || WorldManager.Instance == null || PlayerCamera == null)
        {
            return;
        }
        //
        Vector3 playerPos = CharacterInstance.transform.position;
        playerPos.y += 2.0f;
        PlayerCamera.transform.position = playerPos;

        World containWorld = WorldManager.Instance.ContainedWorld(CharacterInstance.transform.position);
        if (containWorld == null)
        {
            return;
        }
        else if(containWorld != null && containWorld.IsLoadFinish == false)
        {
            return;
        }

        KojeomLogger.DebugLog(string.Format("Player's contain world : {0}, position : {1}", 
            containWorld.name, containWorld.Position), LOG_TYPE.DEBUG_TEST);
        //
        if (isControllProcessOn == false)
        {
            return;
        }

        if (InGameUISupervisor.singleton != null)
        {
            var state = InGameUISupervisor.singleton.chattingBoardState;
            if (state == CHATTING_BOARD_STATE.CLOSE && UIPopupSupervisor.isInGameAllPopupClose)
            {
                var inputData = InputManager.singleton.GetInputData();
                if (inputData.state == INPUT_STATE.CHARACTER_MOVE)
                {
                    curPlayerState = GAMEPLAYER_CHAR_STATE.MOVE;
                }
                else if (inputData.state == INPUT_STATE.NONE)
                {
                    curPlayerState = GAMEPLAYER_CHAR_STATE.IDLE;
                }

                var overlappedInputs = InputManager.singleton.GetOverlappedInputData();
                if (overlappedInputs.Count > 0)
                {
                    var input = overlappedInputs.Dequeue();
                    if (input.state == INPUT_STATE.CHARACTER_JUMP && curPlayerState == GAMEPLAYER_CHAR_STATE.MOVE)
                    {
                        curPlayerState = GAMEPLAYER_CHAR_STATE.MOVING_JUMP;
                    }
                    else if (input.state == INPUT_STATE.CHARACTER_JUMP && curPlayerState != GAMEPLAYER_CHAR_STATE.MOVE)
                    {
                        curPlayerState = GAMEPLAYER_CHAR_STATE.JUMP;
                    }
                }
                KojeomLogger.DebugLog(string.Format("current PlayerState : {0}", curPlayerState), LOG_TYPE.USER_INPUT);

                switch (curPlayerState)
                {
                    case GAMEPLAYER_CHAR_STATE.MOVE:
                        moveStateController.SetState(moveState);
                        moveStateController.Tick();
                        break;
                    case GAMEPLAYER_CHAR_STATE.JUMP:
                        jumpStateController.SetState(jumpState);
                        jumpStateController.Tick();
                        break;
                    case GAMEPLAYER_CHAR_STATE.IDLE:
                        poseStateController.SetState(idleState);
                        poseStateController.Tick();
                        break;
                    case GAMEPLAYER_CHAR_STATE.MOVING_JUMP:
                        jumpStateController.SetState(jumpState);
                        jumpStateController.Tick();
                        moveStateController.SetState(moveState);
                        moveStateController.Tick();
                        break;
                }
                RotationCamAndPlayer();
            }
        }
    }
}
