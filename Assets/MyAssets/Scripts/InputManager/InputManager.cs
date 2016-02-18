using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour {

    [SerializeField]
    private ModifyTerrain modifyTerrian;
    [SerializeField]
    private BlockSelector blockSelector;

    private Ray screenToWorldRay;
    private RaycastHit rayHit;
    private enum INPUT_STATE
    {
        NONE = 0,
        CREATE = 1,
        DELETE = 2,
        ATTACK = 3,
        INVEN_OPEN = 4,
        MENU_OPEN = 5
    }
    private INPUT_STATE inputState = INPUT_STATE.NONE;
    
    
    public void Init()
    {
        // to do
    }

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
            if (ChkRayHit() == false) return;
            inputState = INPUT_STATE.CREATE;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (ChkRayHit() == false) return;
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
        else
        {
            inputState = INPUT_STATE.NONE;
        }
    }

    private bool ChkRayHit()
    {
        screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(screenToWorldRay, out rayHit) &&
            (rayHit.transform.CompareTag("Chunk")))
        {
            return true;
        }
        else
            return false;
       
    }

    private void MouseInputProcess()
    {
        switch (inputState)
        {
            case INPUT_STATE.CREATE:
                InitModifyProcess();
                inputState = INPUT_STATE.NONE;
                if(UIPopupManager.isAllpopupClose == true)
                    modifyTerrian.AddBlockCursor(rayHit, blockSelector.curSelectBlockType);
                break;
            case INPUT_STATE.DELETE:
                InitModifyProcess();
                inputState = INPUT_STATE.NONE;
                if (UIPopupManager.isAllpopupClose == true)
                    modifyTerrian.ReplaceBlockCursor(rayHit, 0);
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
            default:
                break;
        }
    }

    private void InitModifyProcess()
    {
        World world = rayHit.transform.GetComponent<Chunk>().world;
        modifyTerrian.world = world;
    }
}
