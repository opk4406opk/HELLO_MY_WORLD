﻿using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum CHATTING_BOARD_STATE
{
    OPEN = 0,
    CLOSE = 1
}
public class InGameUISupervisor : MonoBehaviour {
    [SerializeField]
    private Camera ingameUICamera;
    [SerializeField]
    private Transform origin_chattingBoard;
    [SerializeField]
    private Transform target_chattingBoard;
    [SerializeField]
    private GameObject obj_chattingLog;
    [SerializeField]
    private UIInput uiInput_chatting;
    [SerializeField]
    private UILabel lbl_chattingLog;
    [SerializeField]
    private GameObject obj_mobileUI;
    [SerializeField]
    private VirtualJoystickManager virtualJoystickManager;

    public CHATTING_BOARD_STATE ChattingBoardState { get; set; }
    private StringBuilder Chatlog;

    private static InGameUISupervisor _Singleton = null;
    public static InGameUISupervisor Singleton
    {
        get
        {
            if (_Singleton == null) KojeomLogger.DebugLog("InGameUISupervisor 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _Singleton;
        }
    }
    public void Init()
    {
        _Singleton = this;
        Chatlog = new StringBuilder();
        ChattingBoardState = CHATTING_BOARD_STATE.CLOSE;
        if (Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            obj_mobileUI.SetActive(false);
        }
        else if(Application.platform == RuntimePlatform.Android || 
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            virtualJoystickManager.Init();
        }
        
        KojeomLogger.DebugLog("InGameUISupervisor 초기화.");
    }

    public Camera GetIngameUICamera()
    {
        return ingameUICamera;
    }

    public void ToggleChattingLog()
    {
        if(obj_chattingLog.transform.position == origin_chattingBoard.position)
        {
            OpenChttingLog();
            ChattingBoardState = CHATTING_BOARD_STATE.OPEN;
            iTween.MoveTo(obj_chattingLog, iTween.Hash(
            "position", target_chattingBoard.position,
            "speed", 1.1f,
            "time", 1.0f));
        }
        else
        {
            ChattingBoardState = CHATTING_BOARD_STATE.CLOSE;
            iTween.MoveTo(obj_chattingLog, iTween.Hash(
           "position", origin_chattingBoard.position,
           "speed", 1.1f,
           "time", 1.0f,
           "oncomplete", "CloseChattingLog",
           "oncompletetarget", gameObject));
        }
    }

    public void SetMsgToChattingLog(string msg, string userID)
    {
        Chatlog.AppendLine(string.Format("[userID:{0}] :: {1}", userID, msg));
        lbl_chattingLog.text = Chatlog.ToString();
        
    }

    public void OnSubmitChatMessage()
    {
        KojeomLogger.DebugLog(string.Format("메세지 : {0} 을(를) 입력하였습니다.", uiInput_chatting.value));
        uiInput_chatting.value = "";
    }

    private void OpenChttingLog()
    {
        KojeomLogger.DebugLog("OpenChattingLog");
        obj_chattingLog.SetActive(true);
    }
    private void CloseChattingLog()
    {
        KojeomLogger.DebugLog("CloseChattingLog");
        uiInput_chatting.value = "";
        obj_chattingLog.SetActive(false);
    }
    
    public void OnClickJump()
    {
        KojeomLogger.DebugLog("Clicked Jump Button");
        var inputMgr = InputManager.Instance;
        if(inputMgr != null)
        {
            var device = inputMgr.GetCurInputDevice();
            ((MobileInput)device).OnTouchJump();
        }
    }
    public void OnClickCreateBlock()
    {
        KojeomLogger.DebugLog("Clicked Create Block");
        var inputMgr = InputManager.Instance;
        if (inputMgr != null)
        {
            var device = inputMgr.GetCurInputDevice();
            ((MobileInput)device).OnTouchCreateBlock();
        }
    }
    public void OnClickDeleteBlock()
    {
        KojeomLogger.DebugLog("Clicked Delete Block");
        var inputMgr = InputManager.Instance;
        if (inputMgr != null)
        {
            var device = inputMgr.GetCurInputDevice();
            ((MobileInput)device).OnTouchDeleteBlock();
        }
    }

    public void OnClickInventory()
    {
        KojeomLogger.DebugLog("Clicked OnClickInventory");
        var inputMgr = InputManager.Instance;
        if (inputMgr != null)
        {
            var device = inputMgr.GetCurInputDevice();
            ((MobileInput)device).OnTouchInventory();
        }
    }
    public void OnClickCraftItem()
    {
        KojeomLogger.DebugLog("Clicked CraftItem");
        var inputMgr = InputManager.Instance;
        if (inputMgr != null)
        {
            var device = inputMgr.GetCurInputDevice();
            ((MobileInput)device).OnTouchCraftItem();
        }
    }
    public void OnClickMenu()
    {
        KojeomLogger.DebugLog("Clicked MainMenu");
        var inputMgr = InputManager.Instance;
        if (inputMgr != null)
        {
            var device = inputMgr.GetCurInputDevice();
            ((MobileInput)device).OnTouchMenu();
        }
    }
}
