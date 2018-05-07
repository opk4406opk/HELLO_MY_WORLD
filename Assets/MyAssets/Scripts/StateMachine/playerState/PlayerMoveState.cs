using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IState
{
    private float moveSpeed;
    private GamePlayer gamePlayer;
    private QuerySDMecanimController aniController;
    private BoxCollider boxCollider;
    private KeyCode curPressedKey;

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
        curPressedKey = KeyCode.None;
    }

    public void UpdateState()
    {
        Move();
    }

    private void Move()
    {
        curPressedKey = InputManager.singleton.GetInputData().keyCode;
        KojeomLogger.DebugLog(string.Format("curPressedKey : {0}", curPressedKey));
        Vector3 dir, newPos = Vector3.zero;
        Vector3 originPos = gamePlayer.transform.position;
        if (curPressedKey == KeyCode.W)
        {
            dir = gamePlayer.transform.forward;
        }
        else if (curPressedKey == KeyCode.S)
        {
            dir = -gamePlayer.transform.forward;
        }
        else if (curPressedKey == KeyCode.D)
        {
            dir = gamePlayer.transform.right;
        }
        else if (curPressedKey == KeyCode.A)
        {
            dir = -gamePlayer.transform.right;
        }
        else
        {
            return;
        }
        KojeomLogger.DebugLog("player moving..");
        newPos = gamePlayer.transform.position + (dir * moveSpeed * Time.deltaTime);
        gamePlayer.transform.position = newPos;

        World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
        var collideInfo = containWorld.customOctree.Collide(gamePlayer.charInstance.GetCustomAABB());
        if (collideInfo.isCollide)
        {
            KojeomLogger.DebugLog(string.Format("player가 이동하는 위치에 Block (pos : {0})이 존재합니다. 이동하지 않습니다.", 
                collideInfo.hitBlockCenter));
            gamePlayer.transform.position = originPos;
        }
    }
}
