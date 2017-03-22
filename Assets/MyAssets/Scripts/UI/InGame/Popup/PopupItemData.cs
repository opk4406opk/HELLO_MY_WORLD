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
        UIPopupManager.CloseItemData();
    }

    private void SetData()
    {
        lbl_itemTitle.text = SceneToScene_Data.popupItemInfo.name;
        spr_itemImg.spriteName = SceneToScene_Data.popupItemInfo.name;
        lbl_itemType.text = SceneToScene_Data.popupItemInfo.type;
        lbl_itemAmount.text = SceneToScene_Data.popupItemInfo.amount;
        lbl_itemDetailInfo.text = SceneToScene_Data.popupItemInfo.detailInfo;
    }
}
