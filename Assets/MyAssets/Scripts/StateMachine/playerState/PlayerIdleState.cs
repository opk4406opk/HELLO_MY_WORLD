using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : APlayerState, IState
{
    private QuerySDMecanimController AniController;

    public PlayerIdleState(GamePlayer player)
    {
        GamePlayer = player;
        AniController = GamePlayer.Controller.CharacterInstance.QueryMecanimController;
    }
    public void InitState()
    {
        // idle 상태 애니메이션.
        AniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.FUKUOKA_DANCE_1);
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
        
    }
}
