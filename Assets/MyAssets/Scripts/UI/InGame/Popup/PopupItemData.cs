using UnityEngine;
using System.Collections;

public class PopupItemData : APopupUI
{
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

    void Start()
    {
        SetData();
        ScaleUpEffect();
    }

    public void OnClickClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    private void SetData()
    {
        UIItemData selectedItemData = new UIItemData();
        if (UIPopupSupervisor.bInvenOpen)
        {
            selectedItemData = InventoryUIManager.singleton.GetLastestSelectItem();
        }
        else if (UIPopupSupervisor.bCraftItemOpen)
        {
            selectedItemData = InventoryUIManager.singleton.GetLastestSelectItem();
        }
        lbl_itemTitle.text = selectedItemData.ItemName;
        spr_itemImg.spriteName = selectedItemData.ItemName;
        lbl_itemType.text = selectedItemData.Type;
        lbl_itemAmount.text = selectedItemData.Amount;
        lbl_itemDetailInfo.text = selectedItemData.DetailInfo;
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.ItemData);
    }
}
