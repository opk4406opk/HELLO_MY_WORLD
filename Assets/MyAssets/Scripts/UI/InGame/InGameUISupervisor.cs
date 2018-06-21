using System.Collections;
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

    public CHATTING_BOARD_STATE chattingBoardState { get; set; }
    private StringBuilder chatlog;

    private static InGameUISupervisor _singleton = null;
    public static InGameUISupervisor singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("InGameUISupervisor 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }
    public void Init()
    {
        _singleton = this;
        chatlog = new StringBuilder();
        chattingBoardState = CHATTING_BOARD_STATE.CLOSE;
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
            chattingBoardState = CHATTING_BOARD_STATE.OPEN;
            iTween.MoveTo(obj_chattingLog, iTween.Hash(
            "position", target_chattingBoard.position,
            "speed", 1.1f,
            "time", 1.0f));
        }
        else
        {
            chattingBoardState = CHATTING_BOARD_STATE.CLOSE;
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
        chatlog.AppendLine(string.Format("[userID:{0}] :: {1}", userID, msg));
        lbl_chattingLog.text = chatlog.ToString();
        
    }

    public void OnSubmitChatMessage()
    {
        KojeomLogger.DebugLog(string.Format("메세지 : {0} 을(를) 입력하였습니다.", uiInput_chatting.value));
        GameNetworkManager.GetInstance().PushChatMessage(uiInput_chatting.value);
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
}
