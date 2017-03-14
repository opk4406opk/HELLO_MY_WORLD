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

    // DB에서 추출한 유저 아이템정보를 담아두는 리스트.
    private List<USER_ITEM> userItemList = new List<USER_ITEM>();

    private ItemDataFile gameItemDataFile;

    void Start()
    {
        if(gameItemDataFile == null)
        {
            GameObject obj = GameObject.Find("ItemDataFile");
            gameItemDataFile = obj.GetComponent<ItemDataFile>();
        }
        CreateInvenEmptySlot(defaultItemSlot);
        SettingUserItem();
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

    private void SettingShopItem()
    {

    }

    private void SettingUserItem()
    {
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
            invenItemSlotList[itemSlotIdx].amount = "x" + uitem.amount.ToString();
            invenItemSlotList[itemSlotIdx].type = uitem.type.ToString();
            // set item detail info
            ItemInfo itemInfo = gameItemDataFile.GetItemData(uitem.id);
            invenItemSlotList[itemSlotIdx].detailInfo = itemInfo.detailInfo;

            invenItemSlotList[itemSlotIdx].InitAllData();
            invenItemSlotList[itemSlotIdx].OnInfo();

            //set event delegate
            Ed_OnClickItem = new EventDelegate(this, "OnClickItem");
            Ed_OnClickItem.parameters[0].value = invenItemSlotList[itemSlotIdx];
            invenItemSlotList[itemSlotIdx].GetComponent<UIButton>().onClick.Add(Ed_OnClickItem);
            itemSlotIdx++;
        }
    }

    private EventDelegate Ed_OnClickItem;
    private void OnClickItem(ItemData itemData)
    {
        GameObject sceneToSceneData = GameObject.Find("SceneToScene_datas");
        SceneToScene_Data.gameInvenItemDatas.Clear();
        SceneToScene_Data.gameInvenItemDatas.Add("id", itemData.id);
        SceneToScene_Data.gameInvenItemDatas.Add("itemName", itemData.itemName);
        SceneToScene_Data.gameInvenItemDatas.Add("type", itemData.type);
        SceneToScene_Data.gameInvenItemDatas.Add("amount", itemData.amount);
        SceneToScene_Data.gameInvenItemDatas.Add("detailInfo", itemData.detailInfo);
        UIPopupManager.OpenItemData();
    }
}
