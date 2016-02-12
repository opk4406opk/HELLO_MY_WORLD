using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    [SerializeField]
    private ModifyTerrain modifyTerrian;

    private Ray screenToWorldRay;
    private RaycastHit rayHit;
    private enum INPUT_STATE
    {
        NONE = 0,
        CREATE = 1,
        DELETE = 2,
        ATTACK = 3
    }
    private INPUT_STATE inputState = INPUT_STATE.NONE;

    private bool isInput = false;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

	void Update ()
    {
        ChkInputState();
        if (ChkRayHit()) InputProcess();
    }

    private void ChkInputState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isInput = true;
            inputState = INPUT_STATE.CREATE;
        }
        else if (Input.GetMouseButtonDown(1))
        {
            isInput = true;
            inputState = INPUT_STATE.DELETE;
        }
        else
        {
            isInput = false;
            inputState = INPUT_STATE.NONE;
        }
    }

    private bool ChkRayHit()
    {
        if (isInput == false) return false;

        screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(screenToWorldRay, out rayHit);
    }

    private void InputProcess()
    {
        InitModifyProcess();
        switch (inputState)
        {
            case INPUT_STATE.CREATE:
                Debug.DrawLine(screenToWorldRay.origin,
                    screenToWorldRay.origin + (screenToWorldRay.direction * rayHit.distance),
                    Color.green, 2);
                modifyTerrian.AddBlockCursor(rayHit, 4);
                break;
            case INPUT_STATE.DELETE:
                Debug.DrawLine(screenToWorldRay.origin,
                    screenToWorldRay.origin + (screenToWorldRay.direction * rayHit.distance),
                    Color.green, 2);
                modifyTerrian.ReplaceBlockCursor(rayHit, 0);
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
