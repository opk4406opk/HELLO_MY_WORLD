using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Data;
using System;

/// <summary>
/// 게임내 사용자의 인벤토리를 관리하는 클래스.
/// </summary>
public class InventoryManager : MonoBehaviour {

    [SerializeField]
    private GameObject itemSlotPrefab;
    [SerializeField]
    private GameObject uiGridObj;
    [SerializeField]
    private GameObject popupObj;

    private readonly int defaultItemSlot = 10;
    private List<ItemData> itemSlotList = new List<ItemData>();
    
    // DB에서 추출한 유저 아이템정보를 담아두는 리스트.
    private List<USER_ITEM> userItemList = new List<USER_ITEM>();

    private ItemDataFile gameItemDataFile;

    void Start ()
    {
        if(gameItemDataFile == null)
        {
            GameObject obj = GameObject.Find("ItemDataFile");
            gameItemDataFile = obj.GetComponent<ItemDataFile>();

        }
        CreateEmptySlot(defaultItemSlot);
        SettingUserItem();
        ScaleUpEffect();
    }

    private void ScaleUpEffect()
    {
        popupObj.transform.localScale = new Vector3(0, 0, 0);
        Vector3 scaleUp = new Vector3(1, 1, 1);
        iTween.ScaleTo(popupObj, iTween.Hash("scale", scaleUp,
            "name", "scaleUp",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none));
    }
    private void ScaleDownEffect(string _callBack)
    {
        popupObj.transform.localScale = new Vector3(1, 1, 1);
        Vector3 scaleDown = new Vector3(0, 0, 0);
        iTween.ScaleTo(popupObj, iTween.Hash("scale", scaleDown,
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
        ScaleDownEffect("CallBackPopupClose");
    }
    /// <summary>
    /// ScaleDown 애니메이션이 종료된 후, 호출되어지는 팝업창 종료 메소드.
    /// </summary>
    private void CallBackPopupClose()
    {
        UIPopupManager.CloseInven();
    }

    private void CreateEmptySlot(int _num)
    {
        for (int idx = 0; idx < _num; ++idx)
        {
            GameObject newItem = Instantiate(itemSlotPrefab,
                new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;

            newItem.SetActive(true);
            newItem.GetComponent<ItemData>().OffInfo();
            //item parenting
            newItem.transform.parent = uiGridObj.transform;
            newItem.transform.localScale = new Vector3(1, 1, 1);
            newItem.transform.localPosition = new Vector3(0, 0, 0);
           
            itemSlotList.Add(newItem.GetComponent<ItemData>());
        }
        uiGridObj.GetComponent<UIGrid>().Reposition();
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
		if (moreEmptySlot > defaultItemSlot) CreateEmptySlot(moreEmptySlot - defaultItemSlot);
		int itemSlotIdx = 0;
        foreach(USER_ITEM uitem in userItemList)
        {
            // set user item info
            itemSlotList[itemSlotIdx].itemName = uitem.name;
            itemSlotList[itemSlotIdx].id = uitem.id;
            itemSlotList[itemSlotIdx].amount = "x" + uitem.amount.ToString();
            itemSlotList[itemSlotIdx].type = uitem.type.ToString();
            // set item detail info
            ItemInfo itemInfo = gameItemDataFile.GetItemData(uitem.id);
            itemSlotList[itemSlotIdx].detailInfo = itemInfo.detailInfo;

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
    private void OnClickItem(ItemData itemData)
    {
        SceneToScene_Data.popupItemInfo.Clear();
        SceneToScene_Data.popupItemInfo.id = itemData.id;
        SceneToScene_Data.popupItemInfo.name = itemData.itemName;
        SceneToScene_Data.popupItemInfo.type = itemData.type;
        SceneToScene_Data.popupItemInfo.amount = itemData.amount;
        SceneToScene_Data.popupItemInfo.detailInfo = itemData.detailInfo;
        
        UIPopupManager.OpenItemData();
    }

}
