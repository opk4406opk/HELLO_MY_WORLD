using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInput : AInput
{
    private Vector3 clickPos;
    private Ray ray;

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

    public override void UpdateProcess()
    {
        CheckSingleInputState();
        CheckOverlapInputState();
        MouseInputProcess();
        KeyBoardInputProcess();
    }

    protected override void CheckOverlapInputState()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InputData input;
            input.state = INPUT_STATE.CHARACTER_JUMP;
            input.keyCode = KeyCode.Space;
            overlappedInputs.Enqueue(input);
        }
    }

    protected override void CheckSingleInputState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMouseInput();
            var actorCollideMgr = ActorCollideManager.singleton;
            if (actorCollideMgr != null && actorCollideMgr.IsNpcCollide(ray))
            {
                curInputData.state = INPUT_STATE.TALK_NPC_MOUSE;
                curInputData.keyCode = KeyCode.Mouse0;
            }
            else
            {
                curInputData.state = INPUT_STATE.CREATE;
                curInputData.keyCode = KeyCode.Mouse0;
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            GetMouseInput();
            curInputData.state = INPUT_STATE.DELETE;
            curInputData.keyCode = KeyCode.Mouse1;
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            curInputData.state = INPUT_STATE.INVEN_OPEN;
            curInputData.keyCode = KeyCode.I;
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            curInputData.state = INPUT_STATE.MENU_OPEN;
            curInputData.keyCode = KeyCode.F10;
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            curInputData.state = INPUT_STATE.CRAFT_ITEM_OPEN;
            curInputData.keyCode = KeyCode.U;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            curInputData.state = INPUT_STATE.TALK_NPC_KEYBORAD;
            curInputData.keyCode = KeyCode.F;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            curInputData.state = INPUT_STATE.CHARACTER_MOVE;
            curInputData.keyCode = KeyCode.W;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            curInputData.state = INPUT_STATE.CHARACTER_MOVE;
            curInputData.keyCode = KeyCode.S;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            curInputData.state = INPUT_STATE.CHARACTER_MOVE;
            curInputData.keyCode = KeyCode.A;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            curInputData.state = INPUT_STATE.CHARACTER_MOVE;
            curInputData.keyCode = KeyCode.D;
        }
        else if (Input.GetKey(KeyCode.BackQuote))
        {
            curInputData.state = INPUT_STATE.CHATTING_TOGGLE;
            curInputData.keyCode = KeyCode.BackQuote;
        }
        else
        {
            curInputData.state = INPUT_STATE.NONE;
            curInputData.keyCode = KeyCode.None;
        }
        KojeomLogger.DebugLog(string.Format("input_data :: state : {0}, keyCode : {1}",
            curInputData.state, curInputData.keyCode), LOG_TYPE.USER_INPUT);
    }

    private void GetMouseInput()
    {
        clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void MouseInputProcess()
    {
        switch (curInputData.state)
        {
            case INPUT_STATE.CREATE:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                if (UIPopupSupervisor.isAllpopupClose == true)
                    modifyTerrain.AddBlockCursor(ray, clickPos, BeltItemSelector.singleton.curSelectBlockType);
                break;
            case INPUT_STATE.DELETE:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                if (UIPopupSupervisor.isAllpopupClose == true)
                    modifyTerrain.ReplaceBlockCursor(ray, clickPos, (byte)TileType.NONE);
                break;
            case INPUT_STATE.TALK_NPC_MOUSE:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.shop);
                break;
            default:
                break;
        }
    }

    private void KeyBoardInputProcess()
    {
        switch (curInputData.state)
        {
            case INPUT_STATE.INVEN_OPEN:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.inven);
                break;
            case INPUT_STATE.MENU_OPEN:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMenu);
                break;
            case INPUT_STATE.CRAFT_ITEM_OPEN:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.craftItem);
                break;
            case INPUT_STATE.TALK_NPC_KEYBORAD:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.shop);
                break;
            case INPUT_STATE.CHATTING_TOGGLE:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                if (InGameUISupervisor.singleton != null)
                {
                    InGameUISupervisor.singleton.ToggleChattingLog();
                }
                break;
            default:
                break;
        }
    }
}
