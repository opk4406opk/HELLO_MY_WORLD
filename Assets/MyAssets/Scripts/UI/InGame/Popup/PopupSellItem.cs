﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupSellItem : APopupUI {

    [SerializeField]
    private UILabel lbl_itemTitle;
    [SerializeField]
    private UILabel lbl_itemAmount;
    [SerializeField]
    private UILabel lbl_itemType;
    [SerializeField]
    private UILabel lbl_itemDetailInfo;
    [SerializeField]
    private UISprite spr_itemImg;

    [SerializeField]
    private UILabel lbl_sellQuantity;
    [SerializeField]
    private UISlider slider_sellQuantity;

    private int maxSell;
    private int curQuantity
    {
        get { return Mathf.FloorToInt(slider_sellQuantity.value * maxSell); }
    }

    void Start()
    {
        maxSell = int.Parse(ShopUIManager.singleton.GetLastestSelectItem().amount);
        slider_sellQuantity.numberOfSteps = maxSell;
        SetData();
        ScaleUpEffect();
    }

    public void OnChangeSliderValue()
    {
        lbl_sellQuantity.text = curQuantity.ToString();
    }

    public void OnSell()
    {

    }

    public void OnClickClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    private void SetData()
    {
        lbl_itemTitle.text = ShopUIManager.singleton.GetLastestSelectItem().itemName;
        spr_itemImg.spriteName = ShopUIManager.singleton.GetLastestSelectItem().itemName;
        lbl_itemType.text = ShopUIManager.singleton.GetLastestSelectItem().type;
        lbl_itemAmount.text = ShopUIManager.singleton.GetLastestSelectItem().amount;
        lbl_itemDetailInfo.text = ShopUIManager.singleton.GetLastestSelectItem().detailInfo;
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.SellItem);
    }
}
