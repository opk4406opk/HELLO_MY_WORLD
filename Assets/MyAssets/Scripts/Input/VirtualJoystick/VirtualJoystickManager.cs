using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualJoystickManager : MonoBehaviour {
    [SerializeField]
    private GameObject moveStickFront;
    [SerializeField]
    private GameObject lookStickFront;
    [SerializeField]
    private Transform baseLookStickTrans;
    [SerializeField]
    private Transform baseMoveStickTrans;
    [SerializeField]
    private BoxCollider moveStickCollider;
    [SerializeField]
    private BoxCollider lookStickCollider;

    private Vector3 moveDirection;
    private Vector3 lookDirection;
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
        if (UIPopupSupervisor.isAllpopupClose == false) return;
        if(Input.touchCount > 0)
        {
            var touches = Input.touches;
            foreach(var touch in touches)
            {
                TouchProcess(touch);
            }
        }
        else
        {
            ResetMoveStickPos();
            ResetLookStickPos();
        }
    }

    private void TouchProcess(Touch touch)
    {
        var ingameUISupervisor = InGameUISupervisor.singleton;
        if (ingameUISupervisor != null)
        {
            var uiCam = ingameUISupervisor.GetIngameUICamera();
            var inputMgr = InputManager.singleton;
            Vector3 worldPoint = uiCam.ScreenToWorldPoint(touch.position);
            if (moveStickCollider.bounds.Contains(worldPoint) == true)
            {
                if (inputMgr != null)
                {
                    var mobileInput = (MobileInput)inputMgr.GetCurInputDevice();
                    mobileInput.OnTouchMoveStick();
                }
                MoveStickTouchProcess(touch, worldPoint);
            }
            else if (lookStickCollider.bounds.Contains(worldPoint) == true)
            {
                LookStickTouchProcess(touch, worldPoint);
            }
        }
    }

    public Vector3 GetMoveDirection()
    {
        Vector3 dir = moveDirection.normalized;
        return new Vector3(dir.x, 0.0f, dir.y);
    }

    public float GetMoveAxisX()
    {
        Vector3 center = baseMoveStickTrans.position;
        Vector3 dir = moveDirection.normalized;
        if(moveStickFront.transform.position.x < center.x)
        {
            dir.x *= -1;
        }
        return dir.x;
    }
    public float GetMoveAxisY()
    {
        Vector3 center = baseMoveStickTrans.position;
        Vector3 dir = moveDirection.normalized;
        if (moveStickFront.transform.position.y < center.y)
        {
            dir.y *= -1;
        }
        return dir.y;
    }

    public Vector3 GetLookDirection()
    {
        return lookDirection.normalized;
    }

    private void ResetMoveStickPos()
    {
        moveStickFront.transform.position = baseMoveStickTrans.position;
        moveDirection = Vector3.zero;
    }
    private void ResetLookStickPos()
    {
        lookStickFront.transform.position = baseLookStickTrans.position;
        lookDirection = Vector3.zero;
    }

    private void MoveStickTouchProcess(Touch touch, Vector3 touchPos)
    {
        var origin = moveStickFront.transform.position;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                ResetMoveStickPos();
                break;
            case TouchPhase.Moved:
                moveStickFront.transform.position = Vector3.Lerp(origin, touchPos, Time.deltaTime * stickMoveSpeed);
                moveDirection = moveStickFront.transform.position - baseMoveStickTrans.position;
                break;
            case TouchPhase.Ended:
                ResetMoveStickPos();
                break;
            case TouchPhase.Canceled:
                ResetMoveStickPos();
                break;
            case TouchPhase.Stationary:
                moveStickFront.transform.position = Vector3.Lerp(origin, touchPos, Time.deltaTime * stickMoveSpeed);
                moveDirection = moveStickFront.transform.position - baseMoveStickTrans.position;
                break;
            default:
                ResetMoveStickPos();
                break;
        }
    }
    private void LookStickTouchProcess(Touch touch, Vector3 touchPos)
    {
        var origin = lookStickFront.transform.position;
        switch (touch.phase)
        {
            case TouchPhase.Began:
                ResetLookStickPos();
                break;
            case TouchPhase.Moved:
                lookStickFront.transform.position = Vector3.Lerp(origin, touchPos, Time.deltaTime * stickMoveSpeed);
                lookDirection = lookStickFront.transform.position - baseLookStickTrans.position;
                break;
            case TouchPhase.Ended:
                ResetLookStickPos();
                break;
            case TouchPhase.Canceled:
                ResetLookStickPos();
                break;
            case TouchPhase.Stationary:
                ResetLookStickPos();
                break;
            default:
                ResetLookStickPos();
                break;
        }
    }
}
