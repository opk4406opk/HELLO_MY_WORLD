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

        GamePlayerCameraManager.Instance.TriggerActive(false);
        InputManager.Instance.TriggerActive(false);
        // state camera on.
        var newCamera = InstancingHelper.Instance.GetNewInstance(GameResourceSupervisor.GetInstance().InPrepareCameraPrefab.LoadSynchro());
        CameraInstance = newCamera.GetComponent<Camera>();
        CameraInstance.name = "PrepareState Camera";

        CameraInstance.transform.position = new Vector3(0, 200.0f, -100.0f);
        CameraInstance.transform.LookAt(new Vector3(0, 0, 0), Vector3.up);

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
                InputManager.Instance.TriggerActive(true);
                GamePlayerCameraManager.Instance.TriggerActive(true);
                // 스테이트 전환.
                StateManagerInstance.ChangeState(GameStateType.Start);
            }
        }
    }
}
