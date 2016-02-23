using UnityEngine;
using System.Collections;

public class PopupElementItemData : MonoBehaviour
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
        UIPopupManager.CloseElementItemData();
    }

    private void SetData()
    {
        GameObject sceneToSceneData = GameObject.Find("SceneToScene_datas");
        string output;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameInvenItemDatas.TryGetValue("itemName", out output);
        lbl_itemTitle.text = output;
        spr_itemImg.spriteName = output;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameInvenItemDatas.TryGetValue("type", out output);
        lbl_itemType.text = output;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameInvenItemDatas.TryGetValue("amount", out output);
        lbl_itemAmount.text = "x" + output;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameInvenItemDatas.TryGetValue("detailInfo", out output);
        lbl_itemDetailInfo.text = output;
    }
}
