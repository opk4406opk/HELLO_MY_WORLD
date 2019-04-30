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
        ScaleDownEffect("CallBackPopupClose");
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.promptServerIP);
    }
}
