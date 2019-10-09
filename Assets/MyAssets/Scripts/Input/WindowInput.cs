using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInput : AInput
{
    private Vector3 ClickPosition;
    private Ray RayInstance;

    public override void Init(ModifyTerrain modifyTerrain)
    {
        ModifyTerrainInstance = modifyTerrain;
        InputDeviceType = INPUT_DEVICE_TYPE.WINDOW;
    }

    public override void UpdateProcess()
    {
        CheckInputState();
    }


    protected override void CheckInputState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMouseInput();
            var actorCollideMgr = ActorCollideManager.singleton;
            if (actorCollideMgr != null && actorCollideMgr.IsNpcCollide(RayInstance))
            {
                UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.shop);
            }
            else
            {
                //Create block
                if (UIPopupSupervisor.bInGameAllPopupClose == true)
                {
                    ModifyTerrainInstance.AddBlockCursor(RayInstance, ClickPosition, BeltItemSelector.singleton.curSelectBlockType);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            GetMouseInput();
            //Delete block
            if (UIPopupSupervisor.bInGameAllPopupClose == true)
            {
                ModifyTerrainInstance.ReplaceBlockCursor(RayInstance, ClickPosition, (byte)BlockTileType.EMPTY);
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.inven);
        }
        if (Input.GetKeyDown(KeyCode.F10))
        {
            UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMenu);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.craftItem);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.shop);
        }
        if (Input.GetKey(KeyCode.BackQuote))
        {
            if (InGameUISupervisor.Singleton != null)
            {
                InGameUISupervisor.Singleton.ToggleChattingLog();
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        List<KeyCode> moveKeyCodes = new List<KeyCode>();
        if (Input.GetKey(KeyCode.W)) moveKeyCodes.Add(KeyCode.W);
        if (Input.GetKey(KeyCode.S)) moveKeyCodes.Add(KeyCode.S);
        if (Input.GetKey(KeyCode.A)) moveKeyCodes.Add(KeyCode.A);
        if (Input.GetKey(KeyCode.D)) moveKeyCodes.Add(KeyCode.D);
        CreateWindowInputData(INPUT_STATE.CHARACTER_MOVE, moveKeyCodes);
        ////////////////////////////////////////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateWindowInputData(INPUT_STATE.CHARACTER_JUMP, KeyCode.Space);
        }
        
    }

    private void GetMouseInput()
    {
        ClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RayInstance = Camera.main.ScreenPointToRay(Input.mousePosition);
    }
}
