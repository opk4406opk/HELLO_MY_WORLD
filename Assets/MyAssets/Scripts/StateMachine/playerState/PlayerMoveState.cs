using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IState
{
    private float moveSpeed;
    private GamePlayer gamePlayer;
    private QuerySDMecanimController aniController;
    private BoxCollider boxCollider;
    private InputData curPressedInput;

    public PlayerMoveState(GamePlayer player)
    {
        gamePlayer = player;
        //
        aniController = gamePlayer.charInstance.GetAniController();
        boxCollider = gamePlayer.charInstance.GetBoxCollider();
        //
        moveSpeed = 3.5f;
    }
    public void InitState()
    {
        aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
    }

    public void ReleaseState()
    {
        curPressedInput.keyCode = KeyCode.None;
        curPressedInput.mobileInputType = MOBILE_INPUT_TYPE.NONE;
    }

    public void UpdateState()
    {
        Move();
    }

    private Vector3 MobileMove()
    {
        var virtualJoystick = VirtualJoystickManager.singleton;
        if(virtualJoystick != null)
        {
            return virtualJoystick.GetMoveDirection();
        }
        return Vector3.zero;
    }
    private Vector3 WindowMove()
    {
        if (curPressedInput.keyCode == KeyCode.W)
        {
            return gamePlayer.transform.forward;
        }
        else if (curPressedInput.keyCode == KeyCode.S)
        {
            return -gamePlayer.transform.forward;
        }
        else if (curPressedInput.keyCode == KeyCode.D)
        {
            return gamePlayer.transform.right;
        }
        else if (curPressedInput.keyCode == KeyCode.A)
        {
            return -gamePlayer.transform.right;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void Move()
    {
        curPressedInput = InputManager.singleton.GetInputData();
        KojeomLogger.DebugLog(string.Format("curPressedKey : {0}", curPressedInput.keyCode), LOG_TYPE.USER_INPUT);
        Vector3 dir = Vector3.zero, newPos = Vector3.zero;
        Vector3 originPos = gamePlayer.transform.position;

        if(Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            dir = WindowMove();
        }
        else if(Application.platform == RuntimePlatform.Android)
        {
            dir = MobileMove();
        }
        else if(dir == Vector3.zero)
        {
            return;
        }
        KojeomLogger.DebugLog("player moving..", LOG_TYPE.USER_INPUT);
        newPos = gamePlayer.transform.position + (dir * moveSpeed * Time.deltaTime);
        gamePlayer.transform.position = newPos;

        World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
        var collideInfo = containWorld.customOctree.Collide(gamePlayer.charInstance.GetCustomAABB());
        if (collideInfo.isCollide)
        {
            KojeomLogger.DebugLog(string.Format("player가 이동하는 위치에 Block (pos : {0})이 존재합니다. 이동하지 않습니다.", 
                collideInfo.hitBlockCenter), LOG_TYPE.USER_INPUT);
            gamePlayer.transform.position = originPos;
        }
    }
}
