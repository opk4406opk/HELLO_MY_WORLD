﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System.Text;

public class CraftItemUIManager : APopupUI {

    [SerializeField]
    private GameObject itemSlotPrefab;
    [SerializeField]
    private GameObject uiGridObj;
    [SerializeField]
    private UIPopupList selectCraftItemList;
    [SerializeField]
    private UIPopupList selectQuantityList;
    [SerializeField]
    private UISprite spr_afterItemImg;

    private readonly int DefaultItemSlot = 9;
    private List<UIItemData> itemSlotList = new List<UIItemData>();

    private UIItemData LastestSelectItem;

    private static CraftItemUIManager _Instance = null;
    public static CraftItemUIManager Instance
    {
        get
        {
            if (_Instance == null) KojeomLogger.DebugLog("CraftItemUIManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _Instance;
        }
    }

    void Start ()
    {
        _Instance = this;
        spr_afterItemImg.spriteName = string.Empty;

        Ed_OnClickQuantityList = new EventDelegate(this, "OnClickQuantTityList");
        selectQuantityList.onChange.Add(Ed_OnClickQuantityList);

        CreateEmptySlot(DefaultItemSlot);
        SetDropDownList();
        ScaleUpEffect();
    }

    public UIItemData GetLastestSelectItem()
    {
        return LastestSelectItem;
    }
    
    public void OnClickMakeItem()
    {
        UpdateUserItems();
    }

    private void UpdateUserItems()
    {
        if(CheckPossibleMakeItem() == true)
        {
            // consume user materials, and user item info update
            CraftItem item;
            CraftItemListDataFile.Instance.CraftItems.TryGetValue(selectCraftItemList.value, out item);
            foreach (CraftRawMaterial raw in item.RawMaterials)
            {
                int userMatAmount = GetUserMaterialAmount(raw.UniqueID);
                int amount = userMatAmount - raw.ConsumeAmount;

                if (amount == 0) DeleteUserMaterial(raw.UniqueID);
                else SetUserMaterialAmount(raw.UniqueID, amount);
            }

            // Set CraftItem to user
            ItemInfo itemInfo = ItemTableReader.GetInstance().GetItemInfo(item.CraftItemID);
            SetCraftItemToUser(item.CraftItemID, item.CraftItemName, int.Parse(selectQuantityList.value), itemInfo.Type.ToString());

            GameMessage.SetMessage("아이템 제작에 성공했습니다.", GameMessage.MESSAGE_TYPE.CRAFT_ITEM_SUCCESS);
            UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.GameMessage);
        }
        else
        {
            GameMessage.SetMessage("수량부족으로 아이템 제작이 불가능합니다.", GameMessage.MESSAGE_TYPE.CRAFT_ITEM_FAIL);
            UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.GameMessage);
        }
    }

    private int GetUserMaterialAmount(string itemID)
    {
        StringBuilder conn = new StringBuilder();
        conn.AppendFormat(GameDBManager.GetInstance().GetDBConnectionPath(), Application.dataPath);

        int amount;
        using (IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn.ToString()))
        {
            dbconn.Open(); //Open connection to the database.
            using (IDbCommand dbcmd = dbconn.CreateCommand())
            {
                StringBuilder sqlQuery = new StringBuilder();
                sqlQuery.AppendFormat("SELECT amount FROM USER_ITEM WHERE id = '{0}'", itemID);
                dbcmd.CommandText = sqlQuery.ToString();
                IDataReader reader = dbcmd.ExecuteReader();

                amount = 0;
                reader.Read();
                amount = reader.GetInt32(0);

                reader.Close();
                reader = null;
            }
            dbconn.Close();
        }
        return amount;
    }

    private void SetCraftItemToUser(string itemID, string itemName, int itemAmount, string itemType)
    {
        StringBuilder conn = new StringBuilder();
        conn.AppendFormat(GameDBManager.GetInstance().GetDBConnectionPath(), Application.dataPath);

        StringBuilder sqlQuery = new StringBuilder();
        sqlQuery.AppendFormat("INSERT INTO USER_ITEM (name, type, amount, id) VALUES ('{0}',{1},{2},'{3}')",
            itemName, itemType, itemAmount, itemID);
        using (IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn.ToString()))
        {
            dbconn.Open(); //Open connection to the database.
            using (IDbCommand dbcmd = dbconn.CreateCommand())
            {
                try
                {
                    dbcmd.CommandText = sqlQuery.ToString();
                    dbcmd.ExecuteNonQuery();
                    dbconn.Close();
                }
                catch // 인벤토리에 중복된 아이템이 있다면, 수량증가를 해야한다.
                {
                    sqlQuery.Remove(0, sqlQuery.Length);
                    sqlQuery.AppendFormat("SELECT amount FROM USER_ITEM WHERE id = '{0}'", itemID);
                    dbcmd.CommandText = sqlQuery.ToString();
                    IDataReader reader = dbcmd.ExecuteReader();
                    reader.Read();
                    int userInvenAmount = reader.GetInt32(0);
                    userInvenAmount += itemAmount;
                    reader.Close();

                    sqlQuery.Remove(0, sqlQuery.Length);
                    sqlQuery.AppendFormat("UPDATE USER_ITEM SET amount = '{0}' WHERE id = '{1}'", userInvenAmount, itemID);
                    dbcmd.CommandText = sqlQuery.ToString();
                    dbcmd.ExecuteNonQuery();
                    dbconn.Close();
                }
            }
            dbconn.Close();
        }
    }

    private void SetUserMaterialAmount(string itemID, int itemAmount)
    {
        string conn = GameDBManager.GetInstance().GetDBConnectionPath();

        using (IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            using (IDbCommand dbcmd = dbconn.CreateCommand())
            {
                string sqlQuery = string.Format("UPDATE USER_ITEM SET amount = '{0}' WHERE id = '{1}'", itemAmount, itemID);
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
            }
            dbconn.Close();
        }
    }

    private void DeleteUserMaterial(string itemID)
    {
        string conn = GameDBManager.GetInstance().GetDBConnectionPath();

        using (IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            using (IDbCommand dbcmd = dbconn.CreateCommand())
            {
                string sqlQuery = string.Format("DELETE FROM USER_ITEM WHERE id = '{0}'", itemID);
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
            }
            dbconn.Close();
        }
    }

    private bool CheckPossibleMakeItem()
    {
        bool bPossibleMakeItem = false;

        CraftItem item;
        CraftItemListDataFile.Instance.CraftItems.TryGetValue(selectCraftItemList.value, out item);
        foreach (CraftRawMaterial raw in item.RawMaterials)
        {
            bPossibleMakeItem = CheckMaterialAmount(raw.UniqueID, raw.ConsumeAmount * int.Parse(selectQuantityList.value));
            if (bPossibleMakeItem == false) return false;
        }
        return true;
    }

    private bool CheckMaterialAmount(string itemID, int needAmount)
    {
        string conn = GameDBManager.GetInstance().GetDBConnectionPath();

        string sqlQuery = string.Format("SELECT amount FROM USER_ITEM WHERE id = '{0}'", itemID);
        using (IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn))
        {
            dbconn.Open(); //Open connection to the database.
            using (IDbCommand dbcmd = dbconn.CreateCommand())
            {
                dbcmd.CommandText = sqlQuery;
                using (IDataReader reader = dbcmd.ExecuteReader())
                {
                    try
                    {
                        int amount = 0;
                        reader.Read();
                        amount = reader.GetInt32(0);
                        reader.Close();
                        dbconn.Close();
                        if (amount >= needAmount) return true;
                        else return false;
                    }
                    catch // 사용자에게 해당 재료 아이템이 없다.
                    {
                        reader.Close();
                        dbconn.Close();
                        return false;
                    }
                }
            }
        }
    }

    private void SetDropDownList()
    {
        foreach(var craftItem in CraftItemListDataFile.Instance.CraftItems)
        {
            selectCraftItemList.AddItem(craftItem.Value.CraftItemName);
        }
        // set event delegate
        Ed_OnClickCraftItemList = new EventDelegate(this, "OnClickCraftItemList");
        selectCraftItemList.onChange.Add(Ed_OnClickCraftItemList);
    }

    private EventDelegate Ed_OnClickQuantityList;
    private void OnClickQuantityList()
    {
        UpdateConsumeAmount();
    }

    private void UpdateConsumeAmount()
    {
        CraftItem item;
        CraftItemListDataFile.Instance.CraftItems.TryGetValue(selectCraftItemList.value, out item);
        int slotIdx = 0;
        foreach (CraftRawMaterial raw in item.RawMaterials)
        {
            string calcedAmount = (raw.ConsumeAmount * int.Parse(selectQuantityList.value)).ToString();
            itemSlotList[slotIdx].Amount = calcedAmount;
            itemSlotList[slotIdx].InitAmountData();
            slotIdx++;
        }
    }

    private EventDelegate Ed_OnClickCraftItemList;
    private void OnClickCraftItemList()
    {
        ShowRawMaterials();
        UpdateConsumeAmount();
    }

    private void ShowRawMaterials()
    {
        ClearItemSlot();

        string selectItemName = selectCraftItemList.value;
        spr_afterItemImg.spriteName = selectItemName;

        CraftItem item;
        CraftItemListDataFile.Instance.CraftItems.TryGetValue(selectItemName, out item);
        var itemTable = ItemTableReader.GetInstance(); ;
        int slotIdx = 0;
        foreach (CraftRawMaterial raw in item.RawMaterials)
        {
            if (slotIdx > DefaultItemSlot) CreateEmptySlot(5);
          
            itemSlotList[slotIdx].ItemName = raw.RawMaterialName;
            itemSlotList[slotIdx].Type = itemTable.GetItemInfo(raw.UniqueID).Type.ToString();
            itemSlotList[slotIdx].DetailInfo = itemTable.GetItemInfo(raw.UniqueID).FlavorText;
            itemSlotList[slotIdx].Amount = raw.ConsumeAmount.ToString();
            itemSlotList[slotIdx].ResourceName = RawElementTableReader.GetInstance().GetTableRow(raw.UniqueID).ResourceName;
            itemSlotList[slotIdx].InitAllData();
            itemSlotList[slotIdx].OnInfo();

            //set event delegate
            Ed_OnClickItem = new EventDelegate(this, "OnClickItem");
            Ed_OnClickItem.parameters[0].value = itemSlotList[slotIdx];
            itemSlotList[slotIdx].GetComponent<UIButton>().onClick.Add(Ed_OnClickItem);

            slotIdx++;
        }
    }

    private EventDelegate Ed_OnClickItem;
    private void OnClickItem(UIItemData itemData)
    {
        LastestSelectItem = itemData;
        UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.ItemData);
    }

    private void ClearItemSlot()
    {
        foreach(UIItemData itemData in itemSlotList)
        {
            itemData.ItemName = string.Empty;
            itemData.Amount = string.Empty;
            itemData.InitAllData();
            itemData.OffInfo();
        }
    }

    private void CreateEmptySlot(int _num)
    {
        for (int idx = 0; idx < _num; ++idx)
        {
            GameObject newItem = Instantiate(itemSlotPrefab,
                new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;

            newItem.SetActive(true);
            newItem.GetComponent<UIItemData>().OffInfo();
            //item parenting
            newItem.transform.parent = uiGridObj.transform;
            newItem.transform.localScale = new Vector3(1, 1, 1);
            newItem.transform.localPosition = new Vector3(0, 0, 0);

            itemSlotList.Add(newItem.GetComponent<UIItemData>());
        }
        uiGridObj.GetComponent<UIGrid>().Reposition();
    }

    public void OnClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(UI_POPUP_TYPE.CraftItem);
    }
}
