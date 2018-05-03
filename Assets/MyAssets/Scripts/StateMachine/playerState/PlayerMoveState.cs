using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IState
{
    private float moveSpeed;
    private GamePlayer gamePlayer;
    private QuerySDMecanimController aniController;
    private BoxCollider boxCollider;
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
    }

    public void UpdateState()
    {
        Move();
    }

    private void Move()
    {
        Vector3 dir, newPos = Vector3.zero;
        Vector3 origin = gamePlayer.transform.position;
        if (Input.GetKey(KeyCode.W))
        {
            dir = gamePlayer.transform.forward;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dir = -gamePlayer.transform.forward;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir = gamePlayer.transform.right;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dir = -gamePlayer.transform.right;
        }
        else
        {
            return;
        }
        newPos = gamePlayer.transform.position + (dir * moveSpeed * Time.deltaTime);
        gamePlayer.transform.position = newPos;
    }
}
