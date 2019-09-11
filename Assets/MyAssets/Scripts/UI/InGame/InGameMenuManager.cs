using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임내 메뉴 팝업창을 관리하는 클래스.
/// </summary>
public class InGameMenuManager : APopupUI
{

    void Start()
    {
        ScaleUpEffect();
    }

    public void OnClickSave()
    {
        //if(saveAndLoadManager.Save())
        //{
        //    GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_SAVE_SUCCESS;
        //    GameMessage.SetMessage("게임 세이브에 성공했습니다.");
        //    UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMessage);
        //}
        //else
        //{
        //    GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_SAVE_FAIL;
        //    GameMessage.SetMessage("게임 세이브에 실패했습니다.");
        //    UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMessage);
        //}
    }

    public void OnClickLoad()
    {
        //if (saveAndLoadManager.Load())
        //{
        //    GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_LOAD_SUCCESS;
        //    GameMessage.SetMessage("게임 로드에 성공했습니다.");
        //    UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMessage);
        //}
        //else
        //{
        //    GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_LOAD_FAIL;
        //    GameMessage.SetMessage("게임 로드에 실패했습니다.");
        //    UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.gameMessage);
        //}
    }

    public void OnClickClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.gameMenu);
    }
}
