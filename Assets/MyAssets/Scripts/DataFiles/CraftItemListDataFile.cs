﻿using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 제작아이템 정보를 가지고 있는 클래스.
/// </summary>
public class CraftItem
{
    public string craftItemID { get; set; }
    public string craftItemName { get; set; }
    public List<CraftRawMaterial> rawMaterials { get; set; } = new List<CraftRawMaterial>();
}

/// <summary>
/// 아이템제작에 쓰이는 재료의 정보를 가지고있는 클래스.
/// </summary>
public struct CraftRawMaterial
{
    public string id;
    public string rawMaterialName;
    public int consumeAmount;
}

/// <summary>
/// 제작아이템 리스트 Json데이터 파일을 관리하는 클래스.
/// </summary>
public class CraftItemListDataFile
{
    private JSONObject craftItemListJsonObj;
    private TextAsset jsonFile;
    public Dictionary<string, CraftItem> craftItems { get; private set; } = new Dictionary<string, CraftItem>();

    public static CraftItemListDataFile instance = null;

    public void Init()
    {
        jsonFile = Resources.Load(ConstFilePath.TXT_CRAFT_ITEM_LIST_DATAS) as TextAsset;
        craftItemListJsonObj = new JSONObject(jsonFile.text);
        AccessData(craftItemListJsonObj);

        if (instance == null) instance = this;
    }

    private void AccessData(JSONObject jsonObj)
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
                    craftItem.craftItemID = id;
                    craftItem.craftItemName = ItemDataFile.instance.GetItemData(id).name;
                    List<CraftRawMaterial> rawMats = new List<CraftRawMaterial>();
                    foreach (var raw in e.list[0].list)
                    {
                        CraftRawMaterial rawMat;
                        string value;
                        raw.ToDictionary().TryGetValue("name", out value);
                        rawMat.rawMaterialName = value;
                        raw.ToDictionary().TryGetValue("amount", out value);
                        rawMat.consumeAmount = int.Parse(value);
                        raw.ToDictionary().TryGetValue("id", out value);
                        rawMat.id = value;
                        rawMats.Add(rawMat);
                    }
                    craftItem.rawMaterials = rawMats;
                    craftItems.Add(craftItem.craftItemName, craftItem);
                }
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }
    }
}
