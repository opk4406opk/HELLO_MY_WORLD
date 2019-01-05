using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPromptServerIP : APopupUI {

    [SerializeField]
    private UIInput input_Prompt;
    private string serverIP;

    private void Start()
    {
        ScaleUpEffect();
    }

    public void OnSubmitIP()
    {
        serverIP = input_Prompt.value;
    }

    public void OnChangeIP()
    {
        
    }

    public void OnClickConnect()
    {
        KojeomLogger.DebugLog("Connect To Host With IP", LOG_TYPE.INFO);
        var netClient = P2PNetworkManager.GetInstance().StartClient();
        P2PNetworkManager.GetInstance().LateInit();
        //
        P2PNetworkManager.GetInstance().isHost = false;
        netClient.Connect(serverIP, 8080);

        ScaleDownEffect("CallBackPopupClose");
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.promptServerIP);
    }
}
