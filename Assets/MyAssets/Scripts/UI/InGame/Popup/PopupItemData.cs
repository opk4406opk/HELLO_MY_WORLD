using UnityEngine;
using System.Collections;

public class PopupItemData : MonoBehaviour
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

    [SerializeField]
    private GameObject popupObj;

    void Start()
    {
        SetData();
        ScaleUpEffect();
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

    public void OnClickClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    private void CallBackPopupClose()
    {
        UIPopupManager.ClosePopupUI(POPUP_TYPE.itemData);
    }

    private void SetData()
    {
        ItemData selectedItemData = new ItemData();
        if (UIPopupManager.isInvenOpen)
        {
            selectedItemData = InventoryUIManager.singleton.GetLastestSelectItem();
        }
        else if (UIPopupManager.isCraftItemOpen)
        {
            selectedItemData = InventoryUIManager.singleton.GetLastestSelectItem();
        }
        lbl_itemTitle.text = selectedItemData.itemName;
        spr_itemImg.spriteName = selectedItemData.itemName;
        lbl_itemType.text = selectedItemData.type;
        lbl_itemAmount.text = selectedItemData.amount;
        lbl_itemDetailInfo.text = selectedItemData.detailInfo;
    }
}
