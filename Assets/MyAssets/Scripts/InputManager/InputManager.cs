using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
    CHARACTER_IDLE = 10
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

    private INPUT_STATE inputState = INPUT_STATE.NONE;

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
        modifyTerrian.Init();
    }

    void Update ()
    {
        ChkInputState();
        MouseInputProcess();
        KeyBoardInputProcess();
    }

    public INPUT_STATE GetInputState()
    {
        return inputState;
    }

    private void ChkInputState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetMouseInput();
            if (actorCollideManager.IsNpcCollide(ray))
            {
                inputState = INPUT_STATE.TALK_NPC_MOUSE;
            }
            else inputState = INPUT_STATE.CREATE;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            GetMouseInput();
            inputState = INPUT_STATE.DELETE;
        }
        else if(Input.GetKeyDown(KeyCode.I))
        {
            inputState = INPUT_STATE.INVEN_OPEN;
        }
        else if (Input.GetKeyDown(KeyCode.F10))
        {
            inputState = INPUT_STATE.MENU_OPEN;
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            inputState = INPUT_STATE.CRAFT_ITEM_OPEN;
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            inputState = INPUT_STATE.TALK_NPC_KEYBORAD;
        }
        else if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            inputState = INPUT_STATE.CHARACTER_MOVE;
        }
        else
        {
            inputState = INPUT_STATE.CHARACTER_IDLE;
        }
    }

    private void GetMouseInput()
    {
        clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    }

    private void MouseInputProcess()
    {
        switch (inputState)
        {
            case INPUT_STATE.CREATE:
                inputState = INPUT_STATE.NONE;
                if(UIPopupManager.isAllpopupClose == true)
                    modifyTerrian.AddBlockCursor(ray, clickPos, BeltItemSelector.singleton.curSelectBlockType);
                break;
            case INPUT_STATE.DELETE:
                inputState = INPUT_STATE.NONE;
                if (UIPopupManager.isAllpopupClose == true)
                    // 0번은 None type의 블록이다. 이 부분에 대해서는 따로 열거형을 쓰거나 해야겠다.
                    modifyTerrian.ReplaceBlockCursor(ray, clickPos, 0);
                break;
            case INPUT_STATE.TALK_NPC_MOUSE:
                inputState = INPUT_STATE.NONE;
                UIPopupManager.OpenShop();
                break;
            default:
                break;
        }
    }

    private void KeyBoardInputProcess()
    {
        switch (inputState)
        {
            case INPUT_STATE.INVEN_OPEN:
                inputState = INPUT_STATE.NONE;
                UIPopupManager.OpenInven();
                break;
            case INPUT_STATE.MENU_OPEN:
                inputState = INPUT_STATE.NONE;
                UIPopupManager.OpenInGameMenu();
                break;
            case INPUT_STATE.CRAFT_ITEM_OPEN:
                inputState = INPUT_STATE.NONE;
                UIPopupManager.OpenCraftItem();
                break;
            case INPUT_STATE.TALK_NPC_KEYBORAD:
                inputState = INPUT_STATE.NONE;
                UIPopupManager.OpenShop();
                break;
            default:
                break;
        }
    }

}
