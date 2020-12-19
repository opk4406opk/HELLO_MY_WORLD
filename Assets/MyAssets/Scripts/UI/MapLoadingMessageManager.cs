using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLoadingMessageManager : APopupUI
{
    [SerializeField]
    private UILabel Lbl_Message;

    public override bool Equals(object other)
    {
        return base.Equals(other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    void Start()
    {
        Lbl_Message.text = " World Map Loading... ";
        ScaleUpEffect();
    }

    public void Close()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.GameMessage);
    }
}
