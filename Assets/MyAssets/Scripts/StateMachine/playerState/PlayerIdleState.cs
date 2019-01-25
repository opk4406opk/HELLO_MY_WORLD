using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : IState
{
    private GamePlayer gamePlayer;
    private QuerySDMecanimController aniController;

    public PlayerIdleState(GamePlayer player)
    {
        gamePlayer = player;
        aniController = gamePlayer.controller.characterObject.queryMecanimController;
    }
    public void InitState()
    {
        // idle 상태 애니메이션.
        aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.FUKUOKA_DANCE_1);
    }

    public void ReleaseState()
    {
    }

    public void UpdateState()
    {
        
    }
}
