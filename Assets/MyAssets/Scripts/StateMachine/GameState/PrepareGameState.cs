using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrepareGameState : AGameState
{
    public PrepareGameState(GameStateManager stateManager)
    {
        StateManagerInstance = stateManager;
    }
    private Camera CameraInstance = null;
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void StartState()
    {
        StateType = GameStateType.Prepare;
        base.StartState();

        var newCamera = InstancingHelper.Instance.GetNewInstance(GameResourceSupervisor.GetInstance().InPrepareCameraPrefab.LoadSynchro());
        CameraInstance = newCamera.GetComponent<Camera>();
        CameraInstance.name = "PrepareState Camera";

        UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.LoadMapGameMessage);
    }

    public override void EndState()
    {
        base.EndState();
        InstancingHelper.Instance.DestroyInstance(CameraInstance.gameObject);
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
        GamePlayerManager playerManager = GamePlayerManager.Instance;
        if(playerManager != null)
        {
            if(playerManager.bFinishMake == true)
            {
                UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.LoadMapGameMessage);
                // 스테이트 전환.
                StateManagerInstance.ChangeState(GameStateType.Start);
            }
        }
    }
}
