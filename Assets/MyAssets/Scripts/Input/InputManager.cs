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
    CHARACTER_JUMP = 10,
    CHATTING_TOGGLE = 11
}
/// <summary>
/// 게임내 입력관리를 하는 클래스.
/// </summary>
public class InputManager : MonoBehaviour {

    [SerializeField]
    private ModifyTerrain modifyTerrain;

    private AInput curInputDevice;
    private WindowInput windowInput;
    private MobileInput mobileInput;

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
        modifyTerrain.Init();
        windowInput = new WindowInput();
        windowInput.Init(modifyTerrain);
        mobileInput = new MobileInput();
        mobileInput.Init(modifyTerrain);

        var curPlatform = Application.platform;
        if(curPlatform == RuntimePlatform.WindowsEditor || curPlatform == RuntimePlatform.WindowsPlayer)
        {
            curInputDevice = windowInput;
        }
        else if(curPlatform == RuntimePlatform.Android)
        {
            curInputDevice = mobileInput;
        }
    }

    void Update ()
    {
       if(IsBeltItemClicked() == false)
       {
            if(curInputDevice != null)
            {
                curInputDevice.UpdateProcess();
            }
        }
    }

    private bool IsBeltItemClicked()
    {
        if (Input.GetMouseButtonDown(0) && InGameUISupervisor.singleton != null)
        {
            var ingameUICamera = InGameUISupervisor.singleton.GetIngameUICamera();
            var ray = ingameUICamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;
            var isCollide = Physics.Raycast(ray, out hitInfo);
            //
            KojeomLogger.DebugLog(string.Format("isColldie anything : {0}", isCollide), LOG_TYPE.USER_INPUT);
            if (isCollide)
            {
                var isBeltCollide = hitInfo.collider.CompareTag("UserBeltCollider");
                KojeomLogger.DebugLog(hitInfo.collider.tag, LOG_TYPE.USER_INPUT);
                if (isBeltCollide) return true;
            }
        }
        return false;
    }

    public InputData GetInputData()
    {
        return curInputDevice.GetInputData();
    }
    public Queue<InputData> GetOverlappedInputData()
    {
        return curInputDevice.GetOverlappedInputData();
    }

    public AInput GetCurInputDevice()
    {
        return curInputDevice;
    }
}
