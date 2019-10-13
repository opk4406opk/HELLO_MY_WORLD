﻿using UnityEngine;
using System.Collections;

/// <summary>
/// 인벤토리, 제작아이템, 상점에 등록되어 있는 아이템 UI의 사용하게 될 정보를 가지고 있는 클래스.
/// ( 아이템 slot 오브젝트에 컴포넌트로 붙게된다. )
/// </summary>
public class UIItemData : MonoBehaviour {
    public string id { set; get; }
    public string amount { set; get; }
    public string type { set; get; }
    public string itemName { set; get; }
    public string detailInfo { set; get; }

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
        gameObject.GetComponent<UIButton>().normalSprite = itemName;
        spr_itemImage.spriteName = itemName;
        lbl_amount.text = amount;
    }
    public void InitAmountData() { lbl_amount.text = amount; }
   
}