using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;

/// <summary>
/// 게임내 사용자의 인벤토리를 관리하는 클래스.
/// </summary>
public class InventoryUIManager : APopupUI {

    [SerializeField]
    private GameObject itemSlotPrefab;
    [SerializeField]
    private GameObject uiGridObj;

    private readonly int defaultItemSlot = 10;
    private List<UIItemData> itemSlotList = new List<UIItemData>();
    
    // DB에서 추출한 유저 아이템정보를 담아두는 리스트.
    private List<DBUserItem> userItemList = new List<DBUserItem>();

    private static InventoryUIManager _singleton = null;
    public static InventoryUIManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("InventoryUIManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }

    private UIItemData lastestSelectItem;

    void Start ()
    {
        _singleton = this;
        CreateEmptySlot(defaultItemSlot);
        SettingUserItem();
        ScaleUpEffect();
    }

    public UIItemData GetLastestSelectItem()
    {
        return lastestSelectItem;
    }

    public void ClickExit()
    {
        ScaleDownEffect("CallBackPopupClose");
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
    private void SettingUserItem()
    {
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
		if (moreEmptySlot > defaultItemSlot) CreateEmptySlot(moreEmptySlot - defaultItemSlot);
		int itemSlotIdx = 0;
        foreach(DBUserItem uitem in userItemList)
        {
            // set user item info
            itemSlotList[itemSlotIdx].itemName = uitem.name;
            itemSlotList[itemSlotIdx].id = uitem.id;
            itemSlotList[itemSlotIdx].amount = uitem.amount.ToString();
            itemSlotList[itemSlotIdx].type = uitem.type.ToString();
            // set item detail info
            ItemInfo itemInfo = ItemTableReader.GetInstance().GetItemInfo(uitem.id);
            itemSlotList[itemSlotIdx].detailInfo = itemInfo.FlavorText;

            itemSlotList[itemSlotIdx].InitAllData();
            itemSlotList[itemSlotIdx].OnInfo();

            //set event delegate
            Ed_OnClickItem = new EventDelegate(this, "OnClickItem");
            Ed_OnClickItem.parameters[0].value = itemSlotList[itemSlotIdx];
            itemSlotList[itemSlotIdx].GetComponent<UIButton>().onClick.Add(Ed_OnClickItem);
			itemSlotIdx++;
        }
    }

    private EventDelegate Ed_OnClickItem;
    private void OnClickItem(UIItemData itemData)
    {
        lastestSelectItem = itemData;
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.itemData);
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.inven);
    }
}
