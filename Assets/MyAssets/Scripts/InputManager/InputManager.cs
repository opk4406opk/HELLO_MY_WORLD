using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

    [SerializeField]
    private ModifyTerrain modifyTerrian;

    private Ray screenToWorldRay;
    private RaycastHit rayHit;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

	void Update ()
    {

        
        if (Input.GetMouseButtonDown(0)) // left
        {
            screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenToWorldRay, out rayHit))
            {
                InitModifyProcess();
                Debug.DrawLine(screenToWorldRay.origin,
                    screenToWorldRay.origin + (screenToWorldRay.direction * rayHit.distance),
                    Color.green, 2);
                modifyTerrian.AddBlockCursor(rayHit, 1);
            }
           
        }
        else if (Input.GetMouseButtonDown(1)) // right
        {
            screenToWorldRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(screenToWorldRay, out rayHit))
            {
                InitModifyProcess();
                Debug.DrawLine(screenToWorldRay.origin,
                    screenToWorldRay.origin + (screenToWorldRay.direction * rayHit.distance),
                    Color.green, 2);
                modifyTerrian.ReplaceBlockCursor(rayHit, 255);
            }
           
        }
        
    }

   
    private void InitModifyProcess()
    {
        World world = rayHit.transform.GetComponent<Chunk>().world;
        modifyTerrian.world = world;
    }
    
    

}
