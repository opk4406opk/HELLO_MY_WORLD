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
    private List<float> CamRotArrayX = new List<float>();
    private List<float> CamRotArrayY = new List<float>();
    private float CamRotationX = 0F;
    private float CamRotationY = 0F;
    public float minimumX = -360F;
    public float maximumX = 360F;
    public float minimumY = -60F;
    public float maximumY = 60F;
    private int FrameCounter = 20;
    [Range(1.0f, 100.0f)]
    public float camSensitivityY = 1.0f;
    [Range(1.0f, 100.0f)]
    public float camSensitivityX = 1.0f;
    private Quaternion camOrigRotation;
    private Quaternion playerOrigRotation;
    #endregion
    [Range(3.5f, 15.5f)]
    public float MoveSpeed;
    private bool IsControllProcessOn = false;

    private PlayerMoveState MoveState;
    private PlayerIdleState IdleState;
    private PlayerJumpState JumpState;
    private StateMachineController MoveStateController;
    private StateMachineController JumpStateController;
    private StateMachineController PoseStateController;
    private GAMEPLAYER_CHAR_STATE CurPlayerState;

    public GameCharacterInstance CharacterInstance { get; private set; }

    public void Init(Camera mainCam, GamePlayer gamePlayer, GameCharacterInstance characterInstance)
    {
        CharacterInstance = characterInstance;
        PlayerCamera = mainCam;
        PlayerCamera.transform.parent = CharacterInstance.transform;
        //
        camOrigRotation = PlayerCamera.transform.localRotation;
        // 게임 플레이어가 아닌, 하위 오브젝트인 캐릭터 인스턴스 방향을 변경해야한다.
        playerOrigRotation = CharacterInstance.transform.localRotation;
        //
        MoveState = new PlayerMoveState(gamePlayer);
        IdleState = new PlayerIdleState(gamePlayer);
        JumpState = new PlayerJumpState(gamePlayer);
        MoveStateController = new StateMachineController();
        JumpStateController = new StateMachineController();
        PoseStateController = new StateMachineController();
        CurPlayerState = GAMEPLAYER_CHAR_STATE.IDLE;
        //
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
        IsControllProcessOn = true;
    }
    public void StopControllProcess()
    {
        IsControllProcessOn = false;
    }

    public GAMEPLAYER_CHAR_STATE GetPlayerState()
    {
        return CurPlayerState;
    }

    private void RotationCamAndPlayer()
    {
        float camRotAverageY = 0f;
        float camRotAverageX = 0f;
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            CamRotationY += Input.GetAxis("Mouse Y") * camSensitivityY;
            CamRotationX += Input.GetAxis("Mouse X") * camSensitivityX;
        }
        else if(Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            var virtualJoystick = VirtualJoystickManager.singleton;
            if(virtualJoystick != null)
            {
                CamRotationY += virtualJoystick.GetLookDirection().y * camSensitivityY;
                CamRotationX += virtualJoystick.GetLookDirection().x * camSensitivityX;
            }
        }
        CamRotArrayY.Add(CamRotationY);
        CamRotArrayX.Add(CamRotationX);

        if (CamRotArrayY.Count >= FrameCounter)
        {
            CamRotArrayY.RemoveAt(0);
        }
        if (CamRotArrayX.Count >= FrameCounter)
        {
            CamRotArrayX.RemoveAt(0);
        }

        for (int j = 0; j < CamRotArrayY.Count; j++)
        {
            camRotAverageY += CamRotArrayY[j];
        }
        for (int i = 0; i < CamRotArrayX.Count; i++)
        {
            camRotAverageX += CamRotArrayX[i];
        }

        camRotAverageY /= CamRotArrayY.Count;
        camRotAverageX /= CamRotArrayX.Count;

        camRotAverageY = Mathf.Clamp(camRotAverageY, minimumY, maximumY);
        // 좌우 움직임은 최대값/최소값 제한두지 않고 적용.
        //camRotAverageX = Mathf.Clamp(camRotAverageX, minimumX, maximumX);

        //KojeomLogger.DebugLog(string.Format("CamRotAvgX : {0}, CamRotAvgY : {1}", camRotAverageX, camRotAverageY), LOG_TYPE.DEBUG_TEST );

        Quaternion yQuaternion = Quaternion.AngleAxis(camRotAverageY, Vector3.left);
        Quaternion xQuaternion = Quaternion.AngleAxis(camRotAverageX, Vector3.up);

        // rot cam (상하)
        PlayerCamera.transform.localRotation = camOrigRotation * yQuaternion;
        // rot player (좌우)
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
        else if(containWorld != null && containWorld.bLoadFinish == false)
        {
            return;
        }

        KojeomLogger.DebugLog(string.Format("Player's contain world : {0}, position : {1}", 
            containWorld.name, containWorld.WorldCoordinate), LOG_TYPE.DEBUG_TEST);
        //
        if (IsControllProcessOn == false)
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
                    CurPlayerState = GAMEPLAYER_CHAR_STATE.MOVE;
                }
                else if (inputData.state == INPUT_STATE.NONE)
                {
                    CurPlayerState = GAMEPLAYER_CHAR_STATE.IDLE;
                }

                var overlappedInputs = InputManager.singleton.GetOverlappedInputData();
                if (overlappedInputs.Count > 0)
                {
                    var input = overlappedInputs.Dequeue();
                    if (input.state == INPUT_STATE.CHARACTER_JUMP && CurPlayerState == GAMEPLAYER_CHAR_STATE.MOVE)
                    {
                        CurPlayerState = GAMEPLAYER_CHAR_STATE.MOVING_JUMP;
                    }
                    else if (input.state == INPUT_STATE.CHARACTER_JUMP && CurPlayerState != GAMEPLAYER_CHAR_STATE.MOVE)
                    {
                        CurPlayerState = GAMEPLAYER_CHAR_STATE.JUMP;
                    }
                }
                KojeomLogger.DebugLog(string.Format("current PlayerState : {0}", CurPlayerState), LOG_TYPE.USER_INPUT);

                switch (CurPlayerState)
                {
                    case GAMEPLAYER_CHAR_STATE.MOVE:
                        MoveStateController.SetState(MoveState);
                        MoveStateController.Tick(Time.deltaTime);
                        break;
                    case GAMEPLAYER_CHAR_STATE.JUMP:
                        JumpStateController.SetState(JumpState);
                        JumpStateController.Tick(Time.deltaTime);
                        break;
                    case GAMEPLAYER_CHAR_STATE.IDLE:
                        PoseStateController.SetState(IdleState);
                        PoseStateController.Tick(Time.deltaTime);
                        break;
                    case GAMEPLAYER_CHAR_STATE.MOVING_JUMP:
                        JumpStateController.SetState(JumpState);
                        JumpStateController.Tick(Time.deltaTime);
                        MoveStateController.SetState(MoveState);
                        MoveStateController.Tick(Time.deltaTime);
                        break;
                }
                RotationCamAndPlayer();
            }
        }
    }
}
