using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using UnityEngine;

public class ShopUIManager : APopupUI {

    [SerializeField]
    private GameObject invenItemSlotPrefab;
    [SerializeField]
    private GameObject uiInvenGridObj;
    [SerializeField]
    private GameObject shopItemSlotPrefab;
    [SerializeField]
    private GameObject uiShopItemGridObj; 

    private readonly int defaultItemSlot = 10;
    private List<ItemData> invenItemSlotList = new List<ItemData>();
    private List<ItemData> shopItemSlotList = new List<ItemData>();

    private static ShopUIManager _singleton = null;
    public static ShopUIManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("ShopUIManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }

    private ItemData lastestSelectItem;

    void Start()
    {
        _singleton = this;
        CreateInvenEmptySlot(defaultItemSlot);
        CreateShopEmptySlot(defaultItemSlot);
        SettingUserItem();
        SettingShopItem();
        ScaleUpEffect();
    }

    public ItemData GetLastestSelectItem()
    {
        return lastestSelectItem;
    }

    public void ClickExit()
    {
        // to do
        ScaleDownEffect("CallBackPopupClose");
    }
  
    private void CreateInvenEmptySlot(int _num)
    {
        for (int idx = 0; idx < _num; ++idx)
        {
            GameObject newItem = Instantiate(invenItemSlotPrefab,
                new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;

            newItem.SetActive(true);
            newItem.GetComponent<ItemData>().OffInfo();
            //item parenting
            newItem.transform.parent = uiInvenGridObj.transform;
            newItem.transform.localScale = new Vector3(1, 1, 1);
            newItem.transform.localPosition = new Vector3(0, 0, 0);

            invenItemSlotList.Add(newItem.GetComponent<ItemData>());
        }
        uiInvenGridObj.GetComponent<UIGrid>().Reposition();
    }

    private void CreateShopEmptySlot(int _num)
    {
        for (int idx = 0; idx < _num; ++idx)
        {
            GameObject newItem = Instantiate(shopItemSlotPrefab,
                new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;

            newItem.SetActive(true);
            newItem.GetComponent<ItemData>().OffInfo();
            //item parenting
            newItem.transform.parent = uiShopItemGridObj.transform;
            newItem.transform.localScale = new Vector3(1, 1, 1);
            newItem.transform.localPosition = new Vector3(0, 0, 0);

            shopItemSlotList.Add(newItem.GetComponent<ItemData>());
        }
        uiShopItemGridObj.GetComponent<UIGrid>().Reposition();
    }

    private void SettingShopItem()
    {
        var shopSellingItemIds = ((RoamingMerchant)NPCManager.Singleton.GetLastestClickedActor()).GetSellingItemIDList();
        List <ItemInfo> shopItems = new List<ItemInfo>();
        foreach(var id in shopSellingItemIds)
        {
            shopItems.Add(ItemDataFile.instance.GetItemData(id.ToString()));
        }

        int moreEmptySlot = shopItems.Count;
        if (moreEmptySlot > defaultItemSlot) CreateShopEmptySlot(moreEmptySlot - defaultItemSlot);
        int itemSlotIdx = 0;
        foreach (ItemInfo item in shopItems)
        {
            // set user item info
            shopItemSlotList[itemSlotIdx].itemName = item.name;
            shopItemSlotList[itemSlotIdx].id = item.id;
            shopItemSlotList[itemSlotIdx].amount = "∞";
            shopItemSlotList[itemSlotIdx].type = item.type.ToString();
            // set item detail info
            shopItemSlotList[itemSlotIdx].detailInfo = item.detailInfo;

            shopItemSlotList[itemSlotIdx].InitAllData();
            shopItemSlotList[itemSlotIdx].OnInfo();
            //set event delegate
            Ed_OnClickShopItem = new EventDelegate(this, "OnClickShopItem");
            Ed_OnClickShopItem.parameters[0].value = shopItemSlotList[itemSlotIdx];
            shopItemSlotList[itemSlotIdx].GetComponent<UIButton>().onClick.Add(Ed_OnClickShopItem);
            itemSlotIdx++;
        }
    }

    private EventDelegate Ed_OnClickShopItem;
    private void OnClickShopItem(ItemData itemData)
    {
        lastestSelectItem = itemData;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.purchaseItem);
    }

    private void SettingUserItem()
    {
        // DB에서 추출한 유저 아이템정보를 담아두는 리스트.
        List<USER_ITEM> userItemList = new List<USER_ITEM>();
        Action GetUserItems = () =>
        {
            StringBuilder conn = new StringBuilder();
            conn.AppendFormat(GameDBHelper.GetInstance().GetDBConnectionPath(), Application.dataPath);

            using (IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn.ToString()))
            {
                using (IDbCommand dbcmd = dbconn.CreateCommand())
                {
                    dbconn.Open(); //Open connection to the database.
                    string sqlQuery = "SELECT name, type, amount, id FROM USER_ITEM";
                    dbcmd.CommandText = sqlQuery;
                    using (IDataReader reader = dbcmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            USER_ITEM userItem;
                            userItem.name = reader.GetString(0);
                            userItem.type = reader.GetInt32(1);
                            userItem.amount = reader.GetInt32(2);
                            userItem.id = reader.GetString(3);
                            userItemList.Add(userItem);
                        }
                        reader.Close();
                    }
                }
                dbconn.Close();
            }
        };
        GetUserItems();

        int moreEmptySlot = userItemList.Count;
        if (moreEmptySlot > defaultItemSlot) CreateInvenEmptySlot(moreEmptySlot - defaultItemSlot);
        int itemSlotIdx = 0;
        foreach (USER_ITEM uitem in userItemList)
        {
            // set user item info
            invenItemSlotList[itemSlotIdx].itemName = uitem.name;
            invenItemSlotList[itemSlotIdx].id = uitem.id;
            invenItemSlotList[itemSlotIdx].amount = uitem.amount.ToString();
            invenItemSlotList[itemSlotIdx].type = uitem.type.ToString();
            // set item detail info
            ItemInfo itemInfo = ItemDataFile.instance.GetItemData(uitem.id);
            invenItemSlotList[itemSlotIdx].detailInfo = itemInfo.detailInfo;

            invenItemSlotList[itemSlotIdx].InitAllData();
            invenItemSlotList[itemSlotIdx].OnInfo();

            //set event delegate
            Ed_OnClickUserItem = new EventDelegate(this, "OnClickUserItem");
            Ed_OnClickUserItem.parameters[0].value = invenItemSlotList[itemSlotIdx];
            invenItemSlotList[itemSlotIdx].GetComponent<UIButton>().onClick.Add(Ed_OnClickUserItem);
            itemSlotIdx++;
        }
    }

    private EventDelegate Ed_OnClickUserItem;
    private void OnClickUserItem(ItemData itemData)
    {
        lastestSelectItem = itemData;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.sellItem);
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.shop);
    }
}
