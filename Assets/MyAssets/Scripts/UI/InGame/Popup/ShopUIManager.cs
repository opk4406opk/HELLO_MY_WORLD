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
    private List<UIItemData> invenItemSlotList = new List<UIItemData>();
    private List<UIItemData> shopItemSlotList = new List<UIItemData>();

    private static ShopUIManager _singleton = null;
    public static ShopUIManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("ShopUIManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }

    private UIItemData lastestSelectItem;

    void Start()
    {
        _singleton = this;
        CreateInvenEmptySlot(defaultItemSlot);
        CreateShopEmptySlot(defaultItemSlot);
        SettingUserItem();
        SettingShopItem();
        ScaleUpEffect();
    }

    public UIItemData GetLastestSelectItem()
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
            newItem.GetComponent<UIItemData>().OffInfo();
            //item parenting
            newItem.transform.parent = uiInvenGridObj.transform;
            newItem.transform.localScale = new Vector3(1, 1, 1);
            newItem.transform.localPosition = new Vector3(0, 0, 0);

            invenItemSlotList.Add(newItem.GetComponent<UIItemData>());
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
            newItem.GetComponent<UIItemData>().OffInfo();
            //item parenting
            newItem.transform.parent = uiShopItemGridObj.transform;
            newItem.transform.localScale = new Vector3(1, 1, 1);
            newItem.transform.localPosition = new Vector3(0, 0, 0);

            shopItemSlotList.Add(newItem.GetComponent<UIItemData>());
        }
        uiShopItemGridObj.GetComponent<UIGrid>().Reposition();
    }

    private void SettingShopItem()
    {
        var shopSellingItemIds = ((MerchantNPC)ActorSuperviosr.Instance.NPCManagerInstance.GetLastestClickedActor()).GetSellingItemIDList();
        List <ItemInfo> shopItems = new List<ItemInfo>();
        foreach(var id in shopSellingItemIds)
        {
            shopItems.Add(ItemTableReader.GetInstance().GetItemInfo(id.ToString()));
        }

        int moreEmptySlot = shopItems.Count;
        if (moreEmptySlot > defaultItemSlot) CreateShopEmptySlot(moreEmptySlot - defaultItemSlot);
        int itemSlotIdx = 0;
        foreach (ItemInfo item in shopItems)
        {
            // set user item info
            shopItemSlotList[itemSlotIdx].itemName = item.Name;
            shopItemSlotList[itemSlotIdx].id = item.UniqueID;
            shopItemSlotList[itemSlotIdx].amount = "∞";
            shopItemSlotList[itemSlotIdx].type = item.Type.ToString();
            // set item detail info
            shopItemSlotList[itemSlotIdx].detailInfo = item.FlavorText;

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
    private void OnClickShopItem(UIItemData itemData)
    {
        lastestSelectItem = itemData;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.purchaseItem);
    }

    private void SettingUserItem()
    {
        // DB에서 추출한 유저 아이템정보를 담아두는 리스트.
        List<DBUserItem> userItemList = new List<DBUserItem>();
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
                            DBUserItem userItem;
                            userItem.name = reader.GetString(0);
                            userItem.type = reader.GetString(1);
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
        foreach (DBUserItem uitem in userItemList)
        {
            // set user item info
            invenItemSlotList[itemSlotIdx].itemName = uitem.name;
            invenItemSlotList[itemSlotIdx].id = uitem.id;
            invenItemSlotList[itemSlotIdx].amount = uitem.amount.ToString();
            invenItemSlotList[itemSlotIdx].type = uitem.type.ToString();
            // set item detail info
            ItemInfo itemInfo = ItemTableReader.GetInstance().GetItemInfo(uitem.id);
            invenItemSlotList[itemSlotIdx].detailInfo = itemInfo.FlavorText;

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
    private void OnClickUserItem(UIItemData itemData)
    {
        lastestSelectItem = itemData;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.sellItem);
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.shop);
    }
}
