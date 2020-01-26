using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPromptServerIP : APopupUI
{
    [SerializeField]
    private UIInput InputPrompt;
    private string ServerIP;

    private void Start()
    {
        ScaleUpEffect();
    }

    public void OnSubmitIP()
    {
        ServerIP = InputPrompt.value;
    }

    public void OnChangeIP()
    {
        
    }

    public void OnClickConnect()
    {
        // effect.
        ScaleDownEffect("CallBackPopupClose");
        // connect.
        if (InputPrompt.value == "") ServerIP = "127.0.0.1";
        else ServerIP = InputPrompt.value;
        GameNetworkManager.GetInstance().ConnectToGameServer(ServerIP, 8000, GameUserNetType.Client);
        GameSceneLoader.LoadGameSceneAsync(GameSceneLoader.SCENE_TYPE.InGame);
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.PromptServerIP);
    }
}
