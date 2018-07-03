using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualJoystickManager : MonoBehaviour {
    [SerializeField]
    private GameObject moveStickFront;
    [SerializeField]
    private GameObject lookStickFront;
    [SerializeField]
    private Transform baseLookStickPosition;
    [SerializeField]
    private Transform baseMoveStickPostion;
    [SerializeField]
    private BoxCollider moveStickCollider;
    [SerializeField]
    private BoxCollider lookStickCollider;

    private float stickMoveSpeed = 2.5f;

    private static VirtualJoystickManager _singleton = null;
    public static VirtualJoystickManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("VirtualJoystickManager 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }
    public void Init()
    {
        _singleton = this;
    }

    private void Update()
    {
        if(Input.touchCount > 0)
        {
            var touches = Input.touches;
            foreach(var touch in touches)
            {
                var ingameUISupervisor = InGameUISupervisor.singleton;
                if(ingameUISupervisor != null)
                {
                    var uiCam = ingameUISupervisor.GetIngameUICamera();
                    Vector3 worldPoint = uiCam.ScreenToWorldPoint(touch.position);
                    if(moveStickCollider.bounds.Contains(worldPoint) == true)
                    {
                        MoveStickTouchProcess(touch, worldPoint);
                    }
                    else if(lookStickCollider.bounds.Contains(worldPoint) == true)
                    {
                        LookStickTouchProcess(touch, worldPoint);
                    }
                }
            }
        }
    }

    private void MoveStickTouchProcess(Touch touch, Vector3 touchPos)
    {
        var origin = moveStickFront.transform.position;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                moveStickFront.transform.position = baseMoveStickPostion.position;
                break;
            case TouchPhase.Moved:
                moveStickFront.transform.position = Vector3.Lerp(origin, touchPos, Time.deltaTime * stickMoveSpeed);
                break;
            case TouchPhase.Ended:
                moveStickFront.transform.position = baseMoveStickPostion.position;
                break;
            case TouchPhase.Canceled:
                moveStickFront.transform.position = baseMoveStickPostion.position;
                break;
            case TouchPhase.Stationary:
                moveStickFront.transform.position = Vector3.Lerp(origin, touchPos, Time.deltaTime * stickMoveSpeed);
                break;
        }
    }
    private void LookStickTouchProcess(Touch touch, Vector3 touchPos)
    {
        var origin = lookStickFront.transform.position;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                lookStickFront.transform.position = baseLookStickPosition.position;
                break;
            case TouchPhase.Moved:
                lookStickFront.transform.position = Vector3.Lerp(origin, touchPos, Time.deltaTime * stickMoveSpeed);
                break;
            case TouchPhase.Ended:
                lookStickFront.transform.position = baseLookStickPosition.position;
                break;
            case TouchPhase.Canceled:
                lookStickFront.transform.position = baseLookStickPosition.position;
                break;
            case TouchPhase.Stationary:
                lookStickFront.transform.position = Vector3.Lerp(origin, touchPos, Time.deltaTime * stickMoveSpeed);
                break;
        }
    }
}
