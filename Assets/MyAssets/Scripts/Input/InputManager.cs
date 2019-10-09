using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public struct InputData
{
    public INPUT_STATE InputState;
    public List<KeyCode> KeyCodeValues;
    public MOBILE_INPUT_TYPE MobileInputType;
}

public enum INPUT_DEVICE_TYPE
{
    WINDOW,
    MOBILE,
    NONE
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
public class InputManager : MonoBehaviour
{

    [SerializeField]
    private ModifyTerrain modifyTerrain;

    private AInput CurrentInputDevice;
    private WindowInput WindowInput;
    private MobileInput MobileInput;

    private static InputManager _Singleton = null;
    public static InputManager Singleton
    {
        get
        {
            if (_Singleton == null) KojeomLogger.DebugLog("InputManager 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _Singleton;
        }
    }

    public void Init()
    {
        _Singleton = this;
        modifyTerrain.Init();
        WindowInput = new WindowInput();
        WindowInput.Init(modifyTerrain);
        MobileInput = new MobileInput();
        MobileInput.Init(modifyTerrain);

        var currentPlatform = Application.platform;
        if(currentPlatform == RuntimePlatform.WindowsEditor ||
            currentPlatform == RuntimePlatform.WindowsPlayer)
        {
            CurrentInputDevice = WindowInput;
        }
        else if(currentPlatform == RuntimePlatform.Android ||
            currentPlatform == RuntimePlatform.IPhonePlayer)
        {
            CurrentInputDevice = MobileInput;
        }
    }

    void Update()
    {
        if (IsBeltItemClicked() == false)
        {
            if (CurrentInputDevice != null)
            {
                CurrentInputDevice.UpdateProcess();
            }
        }
    }

    private bool IsBeltItemClicked()
    {
        if (Input.GetMouseButtonDown(0) && InGameUISupervisor.Singleton != null)
        {
            var ingameUICamera = InGameUISupervisor.Singleton.GetIngameUICamera();
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
        return CurrentInputDevice.GetInputData();
    }

    public InputData PeekInputData()
    {
        return CurrentInputDevice.PeekInputData();
    }

    public AInput GetCurInputDevice()
    {
        return CurrentInputDevice;
    }
}
