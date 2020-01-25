using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class PopupPurchaseItem : APopupUI {
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
    private UILabel lbl_purchaseQuantity;
    [SerializeField]
    private UISlider slider_purchaseQuantity;
    private readonly int maxPurchase = 999;

    private int curQuantity
    {
        get { return Mathf.FloorToInt(slider_purchaseQuantity.value * maxPurchase); }
    }

    void Start () {
        slider_purchaseQuantity.numberOfSteps = maxPurchase;
        SetData();
        ScaleUpEffect();
    }

    public void OnChangeSliderValue()
    {
        lbl_purchaseQuantity.text = curQuantity.ToString();
    }

    public void OnPurchase()
    {
        // to do
        Action<int> UpdateUserItem = (int quantity) =>
        {
            string conn = GameDBManager.GetInstance().GetDBConnectionPath();

            IDbConnection dbconn;
            IDbCommand dbcmd;
            using (dbconn = (IDbConnection)new SqliteConnection(conn))
            {
                var seletedItemData = ShopUIManager.singleton.GetLastestSelectItem();
                using (dbcmd = dbconn.CreateCommand())
                {
                    try
                    {
                        dbconn.Open(); //Open connection to the database.
                        string sqlQuery = "INSERT INTO USER_ITEM (name, type, amount, id) VALUES("
                                           + "'" + seletedItemData.name + "'" + "," + "'" +
                                           seletedItemData.type + "'" + "," + quantity + ","
                                           + seletedItemData.id + ")";
                        dbcmd.CommandText = sqlQuery;
                        dbcmd.ExecuteNonQuery();

                        dbconn.Close();
                    }
                    catch (SqliteException e) // 인벤토리에 중복된 아이템이 있다면, 수량증가를 해야한다.
                    {
                        if (SQLiteErrorCode.Constraint == e.ErrorCode)
                        {
                            string sqlQuery = "SELECT amount FROM USER_ITEM WHERE id = "
                                        + "'" + seletedItemData.id + "'";
                            dbcmd.CommandText = sqlQuery;
                            IDataReader reader = dbcmd.ExecuteReader();
                            reader.Read();
                            int itemAmount = reader.GetInt32(0);
                            itemAmount += quantity;
                            reader.Close();

                            sqlQuery = "UPDATE USER_ITEM SET amount = " + "'" + itemAmount + "'" +
                                        " WHERE id = " + "'" + seletedItemData.id + "'";
                            dbcmd.CommandText = sqlQuery;
                            dbcmd.ExecuteNonQuery();

                            dbconn.Close();
                        }
                    }
                }
                dbconn.Close();
            }
        };
        UpdateUserItem(curQuantity);
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
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.PurchaseItem);
    }
}
