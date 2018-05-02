using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveState : IState
{
    private float moveSpeed;
    private GameCharacter playerGameCharacter;
    private QuerySDMecanimController aniController;
    private BoxCollider boxCollider;
    public PlayerMoveState(GameCharacter player)
    {
        playerGameCharacter = player;
        //
        aniController = playerGameCharacter.GetAniController();
        boxCollider = playerGameCharacter.GetBoxCollider();
        //
        moveSpeed = 3.5f;
    }
    public void InitState()
    {
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
        aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
        Vector3 dir;
        if (Input.GetKey(KeyCode.W))
        {
            dir = playerGameCharacter.transform.forward;
            Vector3 newPos = playerGameCharacter.transform.position + (dir * moveSpeed * Time.deltaTime);
            playerGameCharacter.transform.position = newPos;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            dir = -playerGameCharacter.transform.forward;
            Vector3 newPos = playerGameCharacter.transform.position + (dir * moveSpeed * Time.deltaTime);
            playerGameCharacter.transform.position = newPos;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            dir = playerGameCharacter.transform.right;
            Vector3 newPos = playerGameCharacter.transform.position + (dir * moveSpeed * Time.deltaTime);
            playerGameCharacter.transform.position = newPos;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            dir = -playerGameCharacter.transform.right;
            Vector3 newPos = playerGameCharacter.transform.position + (dir * moveSpeed * Time.deltaTime);
            playerGameCharacter.transform.position = newPos;
        }
    }
}
