using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        overlappedInputs.Enqueue(input);
    }
    public void OnTouchCreateBlock()
    {
        curInputData.state = INPUT_STATE.CREATE;
        curInputData.keyCode = KeyCode.None;
    }
    public void OnTouchDeleteBlock()
    {
        curInputData.state = INPUT_STATE.DELETE;
        curInputData.keyCode = KeyCode.None;
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
