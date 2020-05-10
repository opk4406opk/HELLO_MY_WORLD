using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// ref # 1 : http://wiki.unity3d.com/index.php/SmoothMouseLook

public enum GamePlayerState
{
    MOVE = 0,
    IDLE = 1,
    JUMP = 2,
    MOVING_JUMP = 3,
    NONE,
    COUNT = NONE
}
public class GamePlayerController : MonoBehaviour
{
    private GamePlayer GamePlayerInstance;
    [Range(3.5f, 15.5f)]
    public float MoveSpeed;
    public bool bControllProcessOn { get; private set; }
    private Quaternion PlayerOriginRotation;

    private PlayerMoveState MoveState;
    private PlayerIdleState IdleState;
    private PlayerJumpState JumpState;
    private StateMachineController MoveStateController;
    private StateMachineController JumpStateController;
    private StateMachineController PoseStateController;
    private GamePlayerState CurPlayerState;

    public GameCharacterInstance CharacterInstance { get; private set; }

    public void Init(GamePlayer gamePlayer, GameCharacterInstance characterInstance)
    {
        GamePlayerInstance = gamePlayer;
        CharacterInstance = characterInstance;
        // 게임 플레이어가 아닌, 하위 오브젝트인 캐릭터 인스턴스 방향을 변경해야한다.
        PlayerOriginRotation = CharacterInstance.transform.localRotation;
        //
        MoveStateController = new StateMachineController();
        JumpStateController = new StateMachineController();
        PoseStateController = new StateMachineController();
        CurPlayerState = GamePlayerState.IDLE;
        //
        if(GamePlayerCameraManager.Instance != null)
        {
            GamePlayerCameraManager.Instance.AttachTo(CharacterInstance.transform);
        }
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
    public GamePlayerState GetPlayerState()
    {
        return CurPlayerState;
    }

    public void EnableTick(bool bEnable)
    {
        if (bEnable == true)
        {
            StartCoroutine(Tick());
        }
        else
        {
            StartCoroutine(Tick());
        }
        //
        CharacterInstance.EnableComponents(bEnable);
        bControllProcessOn = bEnable;
    }

    private IEnumerator Tick()
    {
        while(true)
        {
            if (bControllProcessOn == false) yield return null;
            if (CharacterInstance == null || WorldAreaManager.Instance == null || GamePlayerCameraManager.Instance == null) yield return null;
            //
            Vector3 camPosition = CharacterInstance.transform.position;
            camPosition.y += 2.0f;
            GamePlayerCameraManager.Instance.SetPosition(camPosition);

            SubWorld containWorld = WorldAreaManager.Instance.ContainedSubWorld(CharacterInstance.transform.position);
            if (containWorld == null)
            {
                yield return null;
            }
            else if (containWorld != null && containWorld.bLoadFinish == false)
            {
                yield return null;
            }

            // KojeomLogger.DebugLog(string.Format("Player's contain world : {0}, position : {1}", 
            //     containWorld.name, containWorld.OffsetCoordinate), LOG_TYPE.DEBUG_TEST);

            if (InGameUISupervisor.Singleton != null)
            {
                var state = InGameUISupervisor.Singleton.ChattingBoardState;
                if (state == CHATTING_BOARD_STATE.CLOSE && UIPopupSupervisor.bInGameAllPopupClose)
                {
                    bool bPlayerInGroundOrWater = GamePlayerInstance.Controller.CharacterInstance.bContactGround == true ||
                                                  GamePlayerInstance.Controller.CharacterInstance.bContactWater == true;
                    var inputData = InputManager.Instance.GetInputData();
                    if (inputData.InputState == INPUT_STATE.CHARACTER_MOVE)
                    {
                        CurPlayerState = GamePlayerState.MOVE;
                    }
                    else if (inputData.InputState == INPUT_STATE.NONE)
                    {
                        CurPlayerState = GamePlayerState.IDLE;
                    }
                    else if (inputData.InputState == INPUT_STATE.CHARACTER_JUMP &&
                        CurPlayerState == GamePlayerState.MOVE && bPlayerInGroundOrWater == true)
                    {
                        CurPlayerState = GamePlayerState.MOVING_JUMP;
                    }
                    else if (inputData.InputState == INPUT_STATE.CHARACTER_JUMP &&
                        CurPlayerState != GamePlayerState.MOVE && bPlayerInGroundOrWater == true)
                    {
                        CurPlayerState = GamePlayerState.JUMP;
                    }
                    // 로그 주석처리.
                    //KojeomLogger.DebugLog(string.Format("current PlayerState : {0}", CurPlayerState), LOG_TYPE.USER_INPUT);
                    switch (CurPlayerState)
                    {
                        case GamePlayerState.MOVE:
                            MoveState = new PlayerMoveState(GamePlayerInstance, inputData);
                            MoveStateController.SetState(MoveState);
                            MoveStateController.Tick(Time.deltaTime);
                            break;
                        case GamePlayerState.JUMP:
                            JumpState = new PlayerJumpState(GamePlayerInstance, inputData);
                            JumpStateController.SetState(JumpState);
                            JumpStateController.Tick(Time.deltaTime);
                            break;
                        case GamePlayerState.IDLE:
                            IdleState = new PlayerIdleState(GamePlayerInstance, inputData);
                            PoseStateController.SetState(IdleState);
                            PoseStateController.Tick(Time.deltaTime);
                            break;
                        case GamePlayerState.MOVING_JUMP:
                            JumpState = new PlayerJumpState(GamePlayerInstance, inputData);
                            JumpStateController.SetState(JumpState);
                            JumpStateController.Tick(Time.deltaTime);
                            //
                            MoveState = new PlayerMoveState(GamePlayerInstance, inputData);
                            MoveStateController.SetState(MoveState);
                            MoveStateController.Tick(Time.deltaTime);
                            break;
                    }

                    // 캐릭터 좌우 회전.
                    CharacterInstance.transform.localRotation *= GamePlayerCameraManager.Instance.CameraXQuaternion;
                }
            }
            yield return null;
        }
    }

}
