using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임 Item 데이터 내부통신을 위한 구조체.
/// </summary>
public struct ItemInfo
{
    public string id;
    public string name;
    public string type;
    public string detailInfo;
}

/// <summary>
/// 게임 Item 데이터를 가지고 있는 클래스. (json)
/// </summary>
public class ItemDataFile : MonoBehaviour
{

    private JSONObject itemDataJsonObj;
    private TextAsset jsonFile;
    private Dictionary<string, Dictionary<string, string>> jsonDataSheet;

    public void Init()
    {
        jsonDataSheet = new Dictionary<string, Dictionary<string, string>>();
        jsonFile = Resources.Load("TextAsset/ItemDatas/itemDatas") as TextAsset;
        itemDataJsonObj = new JSONObject(jsonFile.text);
        AccessData(itemDataJsonObj);
    }

    public ItemInfo GetItemData(string id)
    {
        Dictionary<string, string> dic;
        jsonDataSheet.TryGetValue(id, out dic);
        string type, detail, name;
        dic.TryGetValue("name", out name);
        dic.TryGetValue("type", out type);
        dic.TryGetValue("detail", out detail);

        ItemInfo itemInfo;
        itemInfo.id = id;
        itemInfo.name = name;
        itemInfo.type = type;
        itemInfo.detailInfo = detail;

        return itemInfo;
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    jsonDataSheet.Add(jsonObj.keys[idx],
                        jsonObj.list[idx].ToDictionary());
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
