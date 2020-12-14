using UnityEngine;
using UnityEngine.Networking;

public class PlayerMoveState : APlayerState, IState
{
    public PlayerMoveState(GamePlayer gamePlayer, InputData inputData)
    {
        GamePlayer = gamePlayer;
        InputData = inputData;
    }
    public void InitState()
    {
        if (GamePlayer.Controller.CharacterInstance.QueryMecanimController != null)
        {
            GamePlayer.Controller.CharacterInstance.QueryMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
        }
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
    }
}
