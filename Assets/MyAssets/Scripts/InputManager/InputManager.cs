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
        INVEN_OPEN = 4
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
            Debug.Log("hit : " + rayHit.transform.name);
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
                Debug.DrawLine(screenToWorldRay.origin,
                    screenToWorldRay.origin + (screenToWorldRay.direction * rayHit.distance),
                    Color.green, 2);
                modifyTerrian.AddBlockCursor(rayHit, blockSelector.curSelectBlockType);
                break;
            case INPUT_STATE.DELETE:
                InitModifyProcess();
                inputState = INPUT_STATE.NONE;
                Debug.DrawLine(screenToWorldRay.origin,
                    screenToWorldRay.origin + (screenToWorldRay.direction * rayHit.distance),
                    Color.green, 2);
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
                if (SceneManager.GetSceneByName("popup_inventory").isLoaded == false)
                    SceneManager.LoadSceneAsync("popup_inventory", LoadSceneMode.Additive);
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
