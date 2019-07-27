using System.Collections;
using UnityEngine;

public class PlayerJumpState : APlayerState, IState
{
    private float JumpScale;
    private float JumpSpeed;

    public PlayerJumpState(GamePlayer player, InputData inputData)
    {
        GamePlayer = player;
        InputData = inputData;
        JumpSpeed = 15.0f;
        JumpScale = 25.0f;
    }
    public void InitState()
    {
        // play animation
        if(GamePlayer.Controller.CharacterInstance.QueryMecanimController != null)
        {
            GamePlayer.Controller.CharacterInstance.QueryMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_FLY_UP);
        }
        // jump
        if(GamePlayer.Controller.CharacterInstance.RigidBodyInstance != null)
        {
            Vector3 dir = GamePlayer.Controller.CharacterInstance.transform.up;
            GamePlayer.Controller.CharacterInstance.RigidBodyInstance.AddForce(dir * JumpSpeed * JumpScale);
        }
    }

    public void ReleaseState()
    {
        if (GamePlayer.Controller.CharacterInstance.QueryMecanimController != null)
        {
            GamePlayer.Controller.CharacterInstance.QueryMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_IDLE);
        }
    }

    public void UpdateState(float deltaTime)
    {
    }
}
