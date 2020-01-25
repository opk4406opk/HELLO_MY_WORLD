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
        lbl_itemTitle.text = selectedItemData.itemName;
        spr_itemImg.spriteName = selectedItemData.itemName;
        lbl_itemType.text = selectedItemData.type;
        lbl_itemAmount.text = selectedItemData.amount;
        lbl_itemDetailInfo.text = selectedItemData.detailInfo;
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.ItemData);
    }
}
