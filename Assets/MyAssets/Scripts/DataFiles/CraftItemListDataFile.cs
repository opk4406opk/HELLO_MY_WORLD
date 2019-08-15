using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 제작아이템 정보를 가지고 있는 클래스.
/// </summary>
public class CraftItem
{
    public string CraftItemID { get; set; }
    public string CraftItemName { get; set; }
    public List<CraftRawMaterial> RawMaterials { get; set; } = new List<CraftRawMaterial>();
}

/// <summary>
/// 아이템제작에 쓰이는 재료의 정보를 가지고있는 클래스.
/// </summary>
public struct CraftRawMaterial
{
    public string UniqueID;
    public string RawMaterialName;
    public int ConsumeAmount;
}

/// <summary>
/// 제작아이템 리스트 Json데이터 파일을 관리하는 클래스.
/// </summary>
public class CraftItemListDataFile : BaseDataFile
{
    public Dictionary<string, CraftItem> CraftItems { get; private set; } = new Dictionary<string, CraftItem>();

    public static CraftItemListDataFile Instance = null;

    public override void Init()
    {
        JsonFile = Resources.Load(ConstFilePath.TXT_CRAFT_ITEM_LIST_DATAS) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);

        if (Instance == null) Instance = this;
    }

    protected override void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                break;
            case JSONObject.Type.ARRAY:
                foreach (var e in jsonObj.list)
                {
                    CraftItem craftItem = new CraftItem();
                    string id = e.keys[0];
                    craftItem.CraftItemID = id;
                    craftItem.CraftItemName = ItemTableReader.GetInstance().GetItemInfo(id).Name;
                    List<CraftRawMaterial> rawMats = new List<CraftRawMaterial>();
                    foreach (var raw in e.list[0].list)
                    {
                        CraftRawMaterial rawMat;
                        string value;
                        raw.ToDictionary().TryGetValue("name", out value);
                        rawMat.RawMaterialName = value;
                        raw.ToDictionary().TryGetValue("amount", out value);
                        rawMat.ConsumeAmount = int.Parse(value);
                        raw.ToDictionary().TryGetValue("id", out value);
                        rawMat.UniqueID = value;
                        rawMats.Add(rawMat);
                    }
                    craftItem.RawMaterials = rawMats;
                    CraftItems.Add(craftItem.CraftItemName, craftItem);
                }
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }
    }
}
