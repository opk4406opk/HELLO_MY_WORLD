using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public struct InputData
{
    public INPUT_STATE state;
    public KeyCode keyCode;
}

public enum INPUT_STATE
{
    NONE = 0,
    CREATE = 1,
    DELETE = 2,
    ATTACK = 3,
    INVEN_OPEN = 4,
    MENU_OPEN = 5,
    CRAFT_ITEM_OPEN = 6,
    TALK_NPC_KEYBORAD = 7,
    TALK_NPC_MOUSE = 8,
    CHARACTER_MOVE = 9,
    CHARACTER_JUMP = 10
}
/// <summary>
/// 게임내 입력관리를 하는 클래스.
/// </summary>
public class InputManager : MonoBehaviour {

    [SerializeField]
    private ModifyTerrain modifyTerrian;
    [SerializeField]
    private ActorCollideManager actorCollideManager;

    private Vector3 clickPos;
    private Ray ray;

    private InputData curInputData;
    private Queue<InputData> overlappedInputs;

    private static InputManager _singleton = null;
    public static InputManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("InputManager 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }

    public void Init()
    {
        _singleton = this;
        curInputData.state = INPUT_STATE.NONE;
        curInputData.keyCode = KeyCode.None;
        overlappedInputs = new Queue<InputData>();
        modifyTerrian.Init();
    }

    void Update ()
    {
        CheckSingleInputState();
        CheckOverlapInputState();
        MouseInputProcess();
        KeyBoardInputProcess();
    }

    public InputData GetInputData()
    {
        return curInputData;
    }
    public Queue<InputData> GetOverlappedInputData()
    {
        return overlappedInputs;
    }

    private void CheckOverlapInputState()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InputData input;
            input.state = INPUT_STATE.CHARACTER_JUMP;
            input.keyCode = KeyCode.Space;
            overlappedInputs.Enqueue(input);
        }
    }

    private void CheckSingleInputState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMouseInput();
            if (actorCollideManager.IsNpcCollide(ray))
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
        //else if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    curInputData.state = INPUT_STATE.CHARACTER_JUMP;
        //    curInputData.keyCode = KeyCode.Space;
        //}
        else
        {
            curInputData.state = INPUT_STATE.NONE;
            curInputData.keyCode = KeyCode.None;
        }
        KojeomLogger.DebugLog(string.Format("input_data :: state : {0}, keyCode : {1}",
            curInputData.state, curInputData.keyCode));
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
                if(UIPopupManager.isAllpopupClose == true)
                    modifyTerrian.AddBlockCursor(ray, clickPos, BeltItemSelector.singleton.curSelectBlockType);
                break;
            case INPUT_STATE.DELETE:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                if (UIPopupManager.isAllpopupClose == true)
                    modifyTerrian.ReplaceBlockCursor(ray, clickPos, (byte)TileType.NONE);
                break;
            case INPUT_STATE.TALK_NPC_MOUSE:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupManager.OpenPopupUI(POPUP_TYPE.shop);
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
                UIPopupManager.OpenPopupUI(POPUP_TYPE.inven);
                break;
            case INPUT_STATE.MENU_OPEN:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupManager.OpenPopupUI(POPUP_TYPE.gameMenu);
                break;
            case INPUT_STATE.CRAFT_ITEM_OPEN:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupManager.OpenPopupUI(POPUP_TYPE.craftItem);
                break;
            case INPUT_STATE.TALK_NPC_KEYBORAD:
                curInputData.state = INPUT_STATE.NONE;
                curInputData.keyCode = KeyCode.None;
                UIPopupManager.OpenPopupUI(POPUP_TYPE.shop);
                break;
            default:
                break;
        }
    }

}
