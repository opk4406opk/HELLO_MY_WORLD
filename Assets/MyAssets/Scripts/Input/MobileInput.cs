using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MOBILE_INPUT_TYPE
{
    NONE = 0,
    MOVE_STICK = 1,
    LOOK_STICK = 2,
    JUMP_BUTTON = 3,
    CREATE_BUTTON = 4,
    DELETE_BUTTON = 5
}
public class MobileInput : AInput
{
    public override void Init(ModifyWorldManager modifyTerrain)
    {
        ModifyTerrainInstance = modifyTerrain;
        InputDeviceType = INPUT_DEVICE_TYPE.MOBILE;
    }

    public void OnTouchJump()
    {
        CreateMobileInputData(INPUT_STATE.CHARACTER_JUMP, MOBILE_INPUT_TYPE.JUMP_BUTTON);
    }
    public void OnTouchCreateBlock()
    {
        CreateMobileInputData(INPUT_STATE.CREATE, MOBILE_INPUT_TYPE.CREATE_BUTTON);
    }
    public void OnTouchDeleteBlock()
    {
        CreateMobileInputData(INPUT_STATE.DELETE, MOBILE_INPUT_TYPE.DELETE_BUTTON);
    }
    public void OnTouchMoveStick()
    {
        CreateMobileInputData(INPUT_STATE.CHARACTER_MOVE, MOBILE_INPUT_TYPE.MOVE_STICK);
    }

    public void OnTouchMenu()
    {
        UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.GameMenu);
    }
    public void OnTouchInventory()
    {
        UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.Inventory);
    }
    public void OnTouchCraftItem()
    {
        UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.CraftItem);
    }

    public override void UpdateProcess()
    {
    }

    protected override void CheckInputState()
    {
    }
}
