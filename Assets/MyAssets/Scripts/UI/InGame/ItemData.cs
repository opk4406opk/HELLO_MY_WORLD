using UnityEngine;
using System.Collections;

/// <summary>
/// 인벤토리에 등록되어 있는 아이템의 세부정보를 가지고 있는 클래스.
/// </summary>
public class ItemData : MonoBehaviour {

    private string _amount;
    public string amount
    {
        set { _amount = value; }
        get { return _amount; }
    }

    private string _type;
    public string type
    {
        set { _type = value; }
        get { return _type; }
    }

    private string _itemName;
    public string itemName
    {
        set { _itemName = value; }
        get { return _itemName; }
    }

    private string _detailInfo;
    public string detailInfo
    {
        set { _detailInfo = value; }
        get { return _detailInfo; }
    }

    [SerializeField]
    private UILabel lbl_amount;
    [SerializeField]
    private UISprite spr_itemImage;

    public void OffInfo()
    {
        lbl_amount.gameObject.SetActive(false);
        spr_itemImage.gameObject.SetActive(false);
    }

    public void OnInfo()
    {
        lbl_amount.gameObject.SetActive(true);
        spr_itemImage.gameObject.SetActive(true);
    }

    public void InitAllData()
    {
        gameObject.GetComponent<UIButton>().normalSprite = _itemName;
        spr_itemImage.spriteName = _itemName;
        lbl_amount.text = _amount;
    }
    public void InitAmountData() { lbl_amount.text = _amount; }
   
}
