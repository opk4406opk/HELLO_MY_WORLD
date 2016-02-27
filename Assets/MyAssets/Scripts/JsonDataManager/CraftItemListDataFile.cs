using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 제작아이템 정보를 가지고 있는 클래스.
/// </summary>
public class CraftItem
{
    private string _craftItemName;
    public string craftItemName
    {
        get { return _craftItemName; }
        set { _craftItemName = value; }
    }
    private List<CraftRawMaterial> _rawMaterials = new List<CraftRawMaterial>();
    public List<CraftRawMaterial> rawMaterials
    {
        get { return _rawMaterials; }
        set { _rawMaterials = value; }
    }
}

/// <summary>
/// 아이템제작에 쓰이는 재료의 정보를 가지고있는 클래스.
/// </summary>
public struct CraftRawMaterial
{
    public string rawMaterialName;
    public int consumeAmount;
}

/// <summary>
/// 제작아이템 리스트 Json데이터 파일을 관리하는 클래스.
/// </summary>
public class CraftItemListDataFile : MonoBehaviour
{

    private JSONObject craftItemListJsonObj;
    private TextAsset jsonFile;
    // 일반적인 용도에서 쓰이는 제작 아이템 리스트.
    private List<CraftItem> _craftItemList;
    public List<CraftItem> craftItemList
    {
        get { return _craftItemList; }
    }
    // UIPopupList 에서 쓰이는 용도의 제작 아이템 리스트.
    private Dictionary<string, List<CraftRawMaterial>> _craftItemDictionary;
    public Dictionary<string, List<CraftRawMaterial>> craftItemDictionary
    {
        get { return _craftItemDictionary; }
    }

    public void Init()
    {
        _craftItemDictionary = new Dictionary<string, List<CraftRawMaterial>>();
        _craftItemList = new List<CraftItem>();
        jsonFile = Resources.Load("TextAsset/ItemDatas/craftItemListDatas") as TextAsset;
        craftItemListJsonObj = new JSONObject(jsonFile.text);
        AccessData(craftItemListJsonObj);
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                for(int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    CraftItem craftItem = new CraftItem();
                    craftItem.craftItemName = jsonObj.keys[idx];

                    JSONObject rawMaterials = jsonObj[idx];
                    int materialNum = rawMaterials.Count;
                    for(int i = 0; i < materialNum; ++i)
                    {
                        CraftRawMaterial craftRawMaterial;
                        craftRawMaterial.rawMaterialName = rawMaterials.keys[i];
                        craftRawMaterial.consumeAmount = int.Parse(rawMaterials.list[i].str);
                        craftItem.rawMaterials.Add(craftRawMaterial);
                    }
                    _craftItemList.Add(craftItem);
                    _craftItemDictionary.Add(craftItem.craftItemName, craftItem.rawMaterials);
                }
                break;
            case JSONObject.Type.ARRAY:
                // to do
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }
    }
}
