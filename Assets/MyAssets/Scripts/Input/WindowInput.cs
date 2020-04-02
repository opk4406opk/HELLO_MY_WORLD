using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInput : AInput
{
    private Vector3 ClickPosition;
    private Ray RayInstance;

    public override void Init(ModifyWorldManager modifyTerrain)
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
        bool bAnyMouseButton = false;
        if (Input.GetMouseButtonDown(0))
        {
            bAnyMouseButton = true;
            GetMouseInput();
            var actorCollideMgr = ActorCollideManager.singleton;
            if (actorCollideMgr != null && actorCollideMgr.IsNpcCollide(RayInstance))
            {
                UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.Shop);
            }
            else
            {
                //Create block
                if (UIPopupSupervisor.bInGameAllPopupClose == true)
                {
                    ModifyTerrainInstance.AddBlockByInput(RayInstance, ClickPosition, BeltItemSelector.singleton.curSelectBlockType);
                }
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            bAnyMouseButton = true;
            GetMouseInput();
            //Delete block
            if (UIPopupSupervisor.bInGameAllPopupClose == true)
            {
                ModifyTerrainInstance.DeleteBlockByInput(RayInstance, ClickPosition, (byte)BlockTileType.EMPTY);
            }
        }

        //// 마우스 아무 버튼이나 누르면 Confined 상태로 전환.
        //if (bAnyMouseButton == true)
        //{
        //    Cursor.lockState = CursorLockMode.Confined;
        //}
        //// 키보드 아무 버튼이나 누르면 Confined 상태로 전환.
        //if (Input.anyKey || Input.anyKeyDown)
        //{
        //    Cursor.lockState = CursorLockMode.Confined;
        //}

        ///////////////////////////////////////////////////////////////////////////////////////
        if (Input.GetKeyDown(KeyCode.I))
        {
            UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.Inventory);
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.GameMenu);
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.CraftItem);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.Shop);
        }
        else if (Input.GetKey(KeyCode.BackQuote))
        {
            if (InGameUISupervisor.Singleton != null)
            {
                InGameUISupervisor.Singleton.ToggleChattingLog();
            }
        }
        else if(Input.GetKey(KeyCode.Escape))
        {
            // 마우스 커서에 대한 잠금을 해제.
            Cursor.lockState = CursorLockMode.None;
        }
        ///////////////////////////////////////////////////////////////////////////////////////
        List<KeyCode> moveKeyCodes = new List<KeyCode>();
        if (Input.GetKey(KeyCode.W)) moveKeyCodes.Add(KeyCode.W);
        if (Input.GetKey(KeyCode.S)) moveKeyCodes.Add(KeyCode.S);
        if (Input.GetKey(KeyCode.A)) moveKeyCodes.Add(KeyCode.A);
        if (Input.GetKey(KeyCode.D)) moveKeyCodes.Add(KeyCode.D);
        if (moveKeyCodes.Count > 0) CreateWindowInputData(INPUT_STATE.CHARACTER_MOVE, moveKeyCodes);
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
