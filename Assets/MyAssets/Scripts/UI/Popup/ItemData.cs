using UnityEngine;
using System.Collections;

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

    public void InitData()
    {
        spr_itemImage.spriteName = _itemName;
        lbl_amount.text = _amount;
    }
}
