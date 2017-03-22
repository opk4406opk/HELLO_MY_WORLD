using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class ShopManager : MonoBehaviour {

    [SerializeField]
    private GameObject invenItemSlotPrefab;
    [SerializeField]
    private GameObject uiInvenGridObj;
    [SerializeField]
    private GameObject shopItemSlotPrefab;
    [SerializeField]
    private GameObject uiShopItemGridObj; 

    [SerializeField]
    private GameObject popupShopObj;

    private readonly int defaultItemSlot = 10;
    private List<ItemData> invenItemSlotList = new List<ItemData>();
    private List<ItemData> shopItemSlotList = new List<ItemData>();
    // 아이템 정보가 기록되어 있는 데이터파일.
    private ItemDataFile gameItemDataFile;

    void Start()
    {
        if(gameItemDataFile == null)
        {
            GameObject obj = GameObject.Find("ItemDataFile");
            gameItemDataFile = obj.GetComponent<ItemDataFile>();
        }
        CreateInvenEmptySlot(defaultItemSlot);
        CreateShopEmptySlot(defaultItemSlot);
        SettingUserItem();
        SettingShopItem();
        ScaleUpEffect();
    }

    private void ScaleUpEffect()
    {
        popupShopObj.transform.localScale = new Vector3(0, 0, 0);
        Vector3 scaleUp = new Vector3(1, 1, 1);
        iTween.ScaleTo(popupShopObj, iTween.Hash("scale", scaleUp,
            "name", "scaleUp",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none));
    }
    private void ScaleDownEffect(string _callBack)
    {
        popupShopObj.transform.localScale = new Vector3(1, 1, 1);
        Vector3 scaleDown = new Vector3(0, 0, 0);
        iTween.ScaleTo(popupShopObj, iTween.Hash("scale", scaleDown,
            "name", "scaleDown",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none,
            "oncomplete", _callBack,
            "oncompletetarget", gameObject));
    }
    public void ClickExit()
    {
        // to do
        ScaleDownEffect("CallBackPopupClose");
    }
    /// <summary>
    /// ScaleDown 애니메이션이 종료된 후, 호출되어지는 팝업창 종료 메소드.
    /// </summary>
    private void CallBackPopupClose()
    {
        UIPopupManager.CloseShop();
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
        List<int> shopItemIDs = SceneToScene_Data.shopItemIDs;
        List<ItemInfo> shopItems = new List<ItemInfo>();
        foreach(var id in shopItemIDs)
        {
            shopItems.Add(gameItemDataFile.GetItemData(id.ToString()));
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
        SceneToScene_Data.popupItemInfo.Clear();
        SceneToScene_Data.popupItemInfo.id = itemData.id;
        SceneToScene_Data.popupItemInfo.name = itemData.itemName;
        SceneToScene_Data.popupItemInfo.type = itemData.type;
        SceneToScene_Data.popupItemInfo.amount = itemData.amount;
        SceneToScene_Data.popupItemInfo.detailInfo = itemData.detailInfo;

        UIPopupManager.OpenPurchaseItemData();
    }

    private void SettingUserItem()
    {
        // DB에서 추출한 유저 아이템정보를 담아두는 리스트.
        List<USER_ITEM> userItemList = new List<USER_ITEM>();
        Action GetUserItems = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/StreamingAssets/GameUserDB/userDB.db";
            using (IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn))
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
            ItemInfo itemInfo = gameItemDataFile.GetItemData(uitem.id);
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
        SceneToScene_Data.popupItemInfo.Clear();
        SceneToScene_Data.popupItemInfo.id = itemData.id;
        SceneToScene_Data.popupItemInfo.name = itemData.itemName;
        SceneToScene_Data.popupItemInfo.type = itemData.type;
        SceneToScene_Data.popupItemInfo.amount = itemData.amount;
        SceneToScene_Data.popupItemInfo.detailInfo = itemData.detailInfo;

        UIPopupManager.OpenSellItemData();
    }
}
