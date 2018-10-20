using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class NPCData
{
    public float hp;
    public float mp;
    public float ap;
    public float dp;
    public string name;
    public string type;
    public Vector3 spawnPos;
    public int spawnWorld;
}
public class RoamingMerchantData : NPCData
{
    public List<int> sellingItemsID;
}
public class ShopMerchantData : NPCData
{
    public List<int> sellingItemsID;
}
public class NPC_TYPE
{
    public static readonly string TYPE_SHOP_MERCHANT = "shop_merchant";
    public static readonly string TYPE_ROAMING_MERCHANT = "roaming_merchant";
}
public class NPCDataFile : MonoBehaviour {
    
    private JSONObject jsonObject;
    private TextAsset jsonFile;
    public List<RoamingMerchantData> roamingMerchantDatas { get; } = new List<RoamingMerchantData>();
    public List<ShopMerchantData> shopMerchantDatas { get; } = new List<ShopMerchantData>();

    public static NPCDataFile instance = null;

	public void Init()
    {
        jsonFile =Resources.Load(ConstFilePath.TXT_NPC_DATAS) as TextAsset;
        jsonObject = new JSONObject(jsonFile.text);
        AccessData(jsonObject);

        if (instance == null) instance = this;
    }

    private List<int> GetItemsID(string ids)
    {
        List<int> itemIDs = new List<int>();
        string[] s = ids.Split(',');
        foreach(var id in s)
        {
            itemIDs.Add(int.Parse(id));
        }
        return itemIDs;
    }
    private Vector3 GetSpwanPos(string pos)
    {
        Vector3 p;
        string[] s = pos.Split(',');
        p.x = float.Parse(s[0]);
        p.y = float.Parse(s[1]);
        p.z = float.Parse(s[2]);
        return p;
    }
    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                //to do
                break;
            case JSONObject.Type.ARRAY:
                foreach(var data in jsonObj.list)
                {
                    string type;
                    data.ToDictionary().TryGetValue("type", out type);
                    if (type.Equals(NPC_TYPE.TYPE_ROAMING_MERCHANT))
                    {
                        RoamingMerchantData npcData = new RoamingMerchantData();
                        string value;
                        data.ToDictionary().TryGetValue("name", out value);
                        npcData.name = value;
                        data.ToDictionary().TryGetValue("type", out value);
                        npcData.type = value;
                        data.ToDictionary().TryGetValue("ap", out value);
                        npcData.ap = float.Parse(value);
                        data.ToDictionary().TryGetValue("dp", out value);
                        npcData.dp = float.Parse(value);
                        data.ToDictionary().TryGetValue("mp", out value);
                        npcData.mp = float.Parse(value);
                        data.ToDictionary().TryGetValue("hp", out value);
                        npcData.hp = float.Parse(value);
                        data.ToDictionary().TryGetValue("spawn_world", out value);
                        npcData.spawnWorld = int.Parse(value);
                        data.ToDictionary().TryGetValue("spawn_position", out value);
                        npcData.spawnPos = GetSpwanPos(value);
                        data.ToDictionary().TryGetValue("selling_items_id", out value);
                        npcData.sellingItemsID = GetItemsID(value);

                        roamingMerchantDatas.Add(npcData);
                    }
                    else if (type.Equals(NPC_TYPE.TYPE_SHOP_MERCHANT))
                    {
                        ShopMerchantData npcData = new ShopMerchantData();
                        string value;
                        data.ToDictionary().TryGetValue("name", out value);
                        npcData.name = value;
                        data.ToDictionary().TryGetValue("type", out value);
                        npcData.type = value;
                        data.ToDictionary().TryGetValue("ap", out value);
                        npcData.ap = float.Parse(value);
                        data.ToDictionary().TryGetValue("dp", out value);
                        npcData.dp = float.Parse(value);
                        data.ToDictionary().TryGetValue("mp", out value);
                        npcData.mp = float.Parse(value);
                        data.ToDictionary().TryGetValue("hp", out value);
                        npcData.hp = float.Parse(value);
                        data.ToDictionary().TryGetValue("spawn_world", out value);
                        npcData.spawnWorld = int.Parse(value);
                        data.ToDictionary().TryGetValue("spawn_position", out value);
                        npcData.spawnPos = GetSpwanPos(value);
                        data.ToDictionary().TryGetValue("selling_items_id", out value);
                        npcData.sellingItemsID = GetItemsID(value);

                        shopMerchantDatas.Add(npcData);
                    }
                }
               // to do
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }
    }
}
