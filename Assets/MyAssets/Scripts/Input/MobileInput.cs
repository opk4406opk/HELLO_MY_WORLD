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
    public override InputData GetInputData()
    {
        return curInputData;
    }

    public override Queue<InputData> GetOverlappedInputData()
    {
        return overlappedInputs;
    }

    public override void Init(ModifyTerrain modifyTerrain)
    {
        this.modifyTerrain = modifyTerrain;
        curInputData.state = INPUT_STATE.NONE;
        curInputData.keyCode = KeyCode.None;
        overlappedInputs = new Queue<InputData>();
    }

    public void OnTouchJump()
    {
        InputData input;
        input.state = INPUT_STATE.CHARACTER_JUMP;
        input.keyCode = KeyCode.None;
        input.mobileInputType = MOBILE_INPUT_TYPE.JUMP_BUTTON;
        overlappedInputs.Enqueue(input);
    }
    public void OnTouchCreateBlock()
    {
        curInputData.state = INPUT_STATE.CREATE;
        curInputData.keyCode = KeyCode.None;
        curInputData.mobileInputType = MOBILE_INPUT_TYPE.CREATE_BUTTON;
    }
    public void OnTouchDeleteBlock()
    {
        curInputData.state = INPUT_STATE.DELETE;
        curInputData.keyCode = KeyCode.None;
        curInputData.mobileInputType = MOBILE_INPUT_TYPE.DELETE_BUTTON;
    }

    public void OnTouchMenu()
    {
        curInputData.state = INPUT_STATE.MENU_OPEN;
        curInputData.keyCode = KeyCode.None;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMenu);
    }
    public void OnTouchInventory()
    {
        curInputData.state = INPUT_STATE.INVEN_OPEN;
        curInputData.keyCode = KeyCode.None;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.inven);
    }
    public void OnTouchCraftItem()
    {
        curInputData.state = INPUT_STATE.CRAFT_ITEM_OPEN;
        curInputData.keyCode = KeyCode.None;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.craftItem);
    }

    public void OnTouchMoveStick()
    {
        curInputData.state = INPUT_STATE.CHARACTER_MOVE;
        curInputData.keyCode = KeyCode.None;
        curInputData.mobileInputType = MOBILE_INPUT_TYPE.MOVE_STICK;
    }

    public override void UpdateProcess()
    {
    }

    protected override void CheckOverlapInputState()
    {
    }

    protected override void CheckSingleInputState()
    {
    }
}
