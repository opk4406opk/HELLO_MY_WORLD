﻿using UnityEngine;
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
public class GamePlayerController : MonoBehaviour
{
    private GamePlayer GamePlayerInstance;
    [Range(3.5f, 15.5f)]
    public float MoveSpeed;
    private bool bControllProcessOn = false;
    private Quaternion PlayerOrigRotation;

    private PlayerMoveState MoveState;
    private PlayerIdleState IdleState;
    private PlayerJumpState JumpState;
    private StateMachineController MoveStateController;
    private StateMachineController JumpStateController;
    private StateMachineController PoseStateController;
    private GAMEPLAYER_CHAR_STATE CurPlayerState;

    public GameCharacterInstance CharacterInstance { get; private set; }

    public void Init(GamePlayer gamePlayer, GameCharacterInstance characterInstance)
    {
        GamePlayerInstance = gamePlayer;
        CharacterInstance = characterInstance;
        // 게임 플레이어가 아닌, 하위 오브젝트인 캐릭터 인스턴스 방향을 변경해야한다.
        PlayerOrigRotation = CharacterInstance.transform.localRotation;
        //
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
        bControllProcessOn = true;
    }
    public void StopControllProcess()
    {
        bControllProcessOn = false;
    }

    public GAMEPLAYER_CHAR_STATE GetPlayerState()
    {
        return CurPlayerState;
    }

    private void FixedUpdate()
    {
        if (CharacterInstance == null || WorldAreaManager.Instance == null || GamePlayerCameraManager.Instance == null)
        {
            return;
        }
        //
        Vector3 playerPos = CharacterInstance.transform.position;
        playerPos.y += 2.0f;
        GamePlayerCameraManager.Instance.SetPosition(playerPos);

        SubWorld containWorld = WorldAreaManager.Instance.ContainedSubWorld(CharacterInstance.transform.position);
        if (containWorld == null)
        {
            return;
        }
        else if(containWorld != null && containWorld.bLoadFinish == false)
        {
            return;
        }

        KojeomLogger.DebugLog(string.Format("Player's contain world : {0}, position : {1}", 
            containWorld.name, containWorld.OffsetCoordinate), LOG_TYPE.DEBUG_TEST);
        //
        if (bControllProcessOn == false)
        {
            return;
        }

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
                    CurPlayerState = GAMEPLAYER_CHAR_STATE.MOVE;
                }
                else if (inputData.InputState == INPUT_STATE.NONE)
                {
                    CurPlayerState = GAMEPLAYER_CHAR_STATE.IDLE;
                }
                else if (inputData.InputState == INPUT_STATE.CHARACTER_JUMP &&
                    CurPlayerState == GAMEPLAYER_CHAR_STATE.MOVE && bPlayerInGroundOrWater == true)
                {
                    CurPlayerState = GAMEPLAYER_CHAR_STATE.MOVING_JUMP;
                }
                else if (inputData.InputState == INPUT_STATE.CHARACTER_JUMP &&
                    CurPlayerState != GAMEPLAYER_CHAR_STATE.MOVE && bPlayerInGroundOrWater == true)
                {
                    CurPlayerState = GAMEPLAYER_CHAR_STATE.JUMP;
                }
                KojeomLogger.DebugLog(string.Format("current PlayerState : {0}", CurPlayerState), LOG_TYPE.USER_INPUT);
                switch (CurPlayerState)
                {
                    case GAMEPLAYER_CHAR_STATE.MOVE:
                        MoveState = new PlayerMoveState(GamePlayerInstance, inputData);
                        MoveStateController.SetState(MoveState);
                        MoveStateController.Tick(Time.deltaTime);
                        break;
                    case GAMEPLAYER_CHAR_STATE.JUMP:
                        JumpState = new PlayerJumpState(GamePlayerInstance, inputData);
                        JumpStateController.SetState(JumpState);
                        JumpStateController.Tick(Time.deltaTime);
                        break;
                    case GAMEPLAYER_CHAR_STATE.IDLE:
                        IdleState = new PlayerIdleState(GamePlayerInstance, inputData);
                        PoseStateController.SetState(IdleState);
                        PoseStateController.Tick(Time.deltaTime);
                        break;
                    case GAMEPLAYER_CHAR_STATE.MOVING_JUMP:
                        JumpState = new PlayerJumpState(GamePlayerInstance, inputData);
                        JumpStateController.SetState(JumpState);
                        JumpStateController.Tick(Time.deltaTime);
                        //
                        MoveState = new PlayerMoveState(GamePlayerInstance, inputData);
                        MoveStateController.SetState(MoveState);
                        MoveStateController.Tick(Time.deltaTime);
                        break;
                }
                // rot player (좌우)
                CharacterInstance.transform.localRotation = PlayerOrigRotation * GamePlayerCameraManager.Instance.CameraXQuaternion;
            }
        }
    }
}
