using System.Collections;
using UnityEngine;

public class PlayerJumpState : APlayerState, IState
{
    private float JumpSpeed;

    public PlayerJumpState(GamePlayer player, InputData inputData)
    {
        GamePlayer = player;
        InputData = inputData;
        JumpSpeed = 60.0f;
    }
    public void InitState()
    {
        // play animation
        if(GamePlayer.Controller.CharacterInstance.QueryMecanimController != null)
        {
            GamePlayer.Controller.CharacterInstance.QueryMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_FLY_UP);
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
