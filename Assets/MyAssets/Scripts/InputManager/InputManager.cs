using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임내 입력관리를 하는 클래스.
/// </summary>
public class InputManager : MonoBehaviour {

    [SerializeField]
    private ModifyTerrain modifyTerrian;
    [SerializeField]
    private BlockSelector blockSelector;

    private enum INPUT_STATE
    {
        NONE = 0,
        CREATE = 1,
        DELETE = 2,
        ATTACK = 3,
        INVEN_OPEN = 4,
        MENU_OPEN = 5,
        CRAFT_ITEM_OPEN = 6
    }
    private INPUT_STATE inputState = INPUT_STATE.NONE;

    void Update ()
    {
        ChkInputState();
        MouseInputProcess();
        KeyBoardInputProcess();
    }

    private void ChkInputState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            inputState = INPUT_STATE.CREATE;
        }
        else if (Input.GetMouseButtonDown(1))
        {
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
        else
        {
            inputState = INPUT_STATE.NONE;
        }
    }

    private void MouseInputProcess()
    {
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.magenta);
        switch (inputState)
        {
            case INPUT_STATE.CREATE:
                inputState = INPUT_STATE.NONE;
                if(UIPopupManager.isAllpopupClose == true)
                    modifyTerrian.AddBlockCursor(ray, blockSelector.curSelectBlockType);
                break;
            case INPUT_STATE.DELETE:
                inputState = INPUT_STATE.NONE;
                if (UIPopupManager.isAllpopupClose == true)
                    modifyTerrian.ReplaceBlockCursor(clickPos, 0);
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
            default:
                break;
        }
    }

}
