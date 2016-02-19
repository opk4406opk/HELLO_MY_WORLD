﻿using UnityEngine;
using System.Collections;

public class GameMessage
{
    public enum MESSAGE_TYPE
    {
        NONE = 0,
        WORLD_LOAD_FAIL = 1,
        WORLD_SAVE_FAIL = 2,
        WORLD_LOAD_SUCCESS = 3,
        WORLD_SAVE_SUCCESS = 4
    }

    private static MESSAGE_TYPE _curGameMsgType = MESSAGE_TYPE.NONE;
    public static MESSAGE_TYPE SetGameMsgType
    {
        set { _curGameMsgType = value; }
    }

    private static string curKeyString;

    public static void SetMessage(string _message)
    {
        switch(_curGameMsgType)
        {
            case MESSAGE_TYPE.WORLD_LOAD_FAIL:
                PlayerPrefs.SetString("WORLD_LOAD_FAIL", _message);
                curKeyString = "WORLD_LOAD_FAIL";
                break;
            case MESSAGE_TYPE.WORLD_LOAD_SUCCESS:
                PlayerPrefs.SetString("WORLD_LOAD_SUCCESS", _message);
                curKeyString = "WORLD_LOAD_SUCCESS";
                break;
            case MESSAGE_TYPE.WORLD_SAVE_FAIL:
                PlayerPrefs.SetString("WORLD_SAVE_FAIL", _message);
                curKeyString = "WORLD_SAVE_FAIL";
                break;
            case MESSAGE_TYPE.WORLD_SAVE_SUCCESS:
                PlayerPrefs.SetString("WORLD_SAVE_SUCCESS", _message);
                curKeyString = "WORLD_SAVE_SUCCESS";
                break;
            default:
                break;
        }
    }

    public static string GetMessage()
    {
        return PlayerPrefs.GetString(curKeyString);
    }
}

public class MessageManager : MonoBehaviour {

    [SerializeField]
    private GameObject popupObj;
    [SerializeField]
    private UILabel lbl_message;

    void Start ()
    {
        lbl_message.text = GameMessage.GetMessage();
        ScaleUpEffect();
    }

    public void OnClose()
    {
        ScaleDownEffect("CallBackClose");
    }

    private void ScaleUpEffect()
    {
        popupObj.transform.localScale = new Vector3(0, 0, 0);
        Vector3 scaleUp = new Vector3(1, 1, 1);
        iTween.ScaleTo(popupObj, iTween.Hash("scale", scaleUp,
            "name", "scaleUp",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none));
    }

    private void ScaleDownEffect(string _callBack)
    {
        popupObj.transform.localScale = new Vector3(1, 1, 1);
        Vector3 scaleDown = new Vector3(0, 0, 0);
        iTween.ScaleTo(popupObj, iTween.Hash("scale", scaleDown,
            "name", "scaleDown",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none,
            "oncomplete", _callBack,
            "oncompletetarget", gameObject));
    }

    private void CallBackClose()
    {
        UIPopupManager.CloseGameMessage();
    }

}