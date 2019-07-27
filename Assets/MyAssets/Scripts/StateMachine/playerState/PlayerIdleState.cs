using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : APlayerState, IState
{

    public PlayerIdleState(GamePlayer player, InputData inputData)
    {
        GamePlayer = player;
        InputData = inputData;
    }
    public void InitState()
    {
        // idle 상태 애니메이션.
        if(GamePlayer.Controller.CharacterInstance.QueryMecanimController != null)
        {
            GamePlayer.Controller.CharacterInstance.QueryMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.FUKUOKA_DANCE_1);
        }
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
        
    }
}
