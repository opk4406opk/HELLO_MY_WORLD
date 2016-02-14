using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;

public class InventoryManager : MonoBehaviour {

    [SerializeField]
    private GameObject itemSlotPrefab;
    [SerializeField]
    private GameObject uiGridObj;
    [SerializeField]
    private GameObject popupObj;

    private readonly int defaultItemSlot = 50;
    private List<ItemData> _itemSlotList = new List<ItemData>();
    private List<ItemData> itemSlotList
    {
        get { return _itemSlotList; }
    }
    // DB에서 추출한 유저 아이템정보를 담아두는 리스트.
    private List<USER_ITEM> userItemList = new List<USER_ITEM>();

    private ItemDataFile gameItemDataFile;

    void Start ()
    {
        GameObject obj = GameObject.Find("ItemDataFile");
        gameItemDataFile = obj.GetComponent<ItemDataFile>();

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
    private void ScaleDownEffect()
    {
        popupObj.transform.localScale = new Vector3(1, 1, 1);
        Vector3 scaleUp = new Vector3(0, 0, 0);
        iTween.ScaleTo(popupObj, iTween.Hash("scale", scaleUp,
            "name", "scaleUp",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none));
    }
    public void ClickExit()
    {
        StartCoroutine(PopupExitProcess());
    }
    private IEnumerator PopupExitProcess()
    {
        ScaleDownEffect();
        yield return new WaitForSeconds(0.2f);
        SceneManager.UnloadScene("popup_inventory");
    }

    public void CreateEmptySlot(int _num)
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
           
            _itemSlotList.Add(newItem.GetComponent<ItemData>());
        }
        uiGridObj.GetComponent<UIGrid>().Reposition();

    }

    private delegate void del_GetUserItems();
    private void SettingUserItem()
    {
        del_GetUserItems GetUserItems = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/MyAssets/Resources/GameUserDB/userDB.db";

            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbconn.Open(); //Open connection to the database.

            string sqlQuery = "SELECT name, type, amount FROM USER_ITEM";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                USER_ITEM userItem;
                userItem.name = reader.GetString(0);
                userItem.type = reader.GetInt32(1);
                userItem.amount = reader.GetInt32(2);

                userItemList.Add(userItem);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        };
        GetUserItems();

        int itemSlotIdx = 0;
        foreach(USER_ITEM uitem in userItemList)
        {
            if (itemSlotIdx > defaultItemSlot) CreateEmptySlot(10);

            // set user item info
            _itemSlotList[itemSlotIdx].name = uitem.name;
            _itemSlotList[itemSlotIdx].amount = uitem.amount.ToString();
            _itemSlotList[itemSlotIdx].type = uitem.type.ToString();
            // set item detail info
            ItemInfo itemInfo = gameItemDataFile.GetItemData(uitem.name);
            _itemSlotList[itemSlotIdx].detailInfo = itemInfo.detailInfo;

            _itemSlotList[itemSlotIdx].InitData();
            _itemSlotList[itemSlotIdx].OnInfo();

            itemSlotIdx++;
        }
    }

}
