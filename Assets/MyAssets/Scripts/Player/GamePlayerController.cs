using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ECM.Controllers;

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
        //
        MoveStateController = new StateMachineController();
        JumpStateController = new StateMachineController();
        PoseStateController = new StateMachineController();
        CurPlayerState = GamePlayerState.IDLE;
    }

    public Vector3 GetPosition()
    {
        return CharacterInstance.GetPosition();
    }

    public void SetPosition(Vector3 newPos)
    {
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
        CharacterInstance.EnableComponents(bEnable);
        bControllProcessOn = bEnable;
    }

    private void Update()
    {
        if(CharacterInstance != null && GamePlayerCameraManager.Instance != null)
        {
            CharacterInstance.ECM_MouseLookComp.LookRotation(CharacterInstance.ECM_BaseCharController.movement, GamePlayerCameraManager.Instance.GetPlayerCamera().transform);
        }
    }

    private void FixedUpdate()
    {
        if (bControllProcessOn == false) return;
        if (CharacterInstance == null || WorldAreaManager.Instance == null || GamePlayerCameraManager.Instance == null) return;
       
        SubWorld containWorld = WorldAreaManager.Instance.ContainedSubWorld(CharacterInstance.transform.position);
        if (containWorld == null)
        {
            return;
        }
        else if (containWorld != null && containWorld.bLoadFinish == false)
        {
            return;
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
                else if (inputData.InputState == INPUT_STATE.CHARACTER_JUMP && CurPlayerState == GamePlayerState.MOVE && bPlayerInGroundOrWater == true)
                {
                    CurPlayerState = GamePlayerState.MOVING_JUMP;
                }
                else if (inputData.InputState == INPUT_STATE.CHARACTER_JUMP && CurPlayerState != GamePlayerState.MOVE && bPlayerInGroundOrWater == true)
                {
                    CurPlayerState = GamePlayerState.JUMP;
                }
                // 로그 주석처리.
                //KojeomLogger.DebugLog(string.Format("current PlayerState : {0}", CurPlayerState), LOG_TYPE.USER_INPUT);
                switch (CurPlayerState)
                {
                    case GamePlayerState.MOVE:
                        MoveState = new PlayerMoveState(GamePlayerInstance, inputData);
                        MoveStateController.ChangeState(MoveState);
                        MoveStateController.Tick(Time.deltaTime);
                        break;
                    case GamePlayerState.JUMP:
                        JumpState = new PlayerJumpState(GamePlayerInstance, inputData);
                        JumpStateController.ChangeState(JumpState);
                        JumpStateController.Tick(Time.deltaTime);
                        break;
                    case GamePlayerState.IDLE:
                        IdleState = new PlayerIdleState(GamePlayerInstance, inputData);
                        PoseStateController.ChangeState(IdleState);
                        PoseStateController.Tick(Time.deltaTime);
                        break;
                    case GamePlayerState.MOVING_JUMP:
                        JumpState = new PlayerJumpState(GamePlayerInstance, inputData);
                        JumpStateController.ChangeState(JumpState);
                        JumpStateController.Tick(Time.deltaTime);
                        //
                        MoveState = new PlayerMoveState(GamePlayerInstance, inputData);
                        MoveStateController.ChangeState(MoveState);
                        MoveStateController.Tick(Time.deltaTime);
                        break;
                }
            }
        }
    }

}
