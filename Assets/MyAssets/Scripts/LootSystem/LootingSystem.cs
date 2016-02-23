using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임내 사용자가 블록 해제시 얻는 아이템 및 행동을 관리하는 클래스.
/// </summary>
public class LootingSystem : MonoBehaviour
{
    private JSONObject typeToItemNameJsonObj;
    private TextAsset jsonFile;
    private Dictionary<string, string> jsonDataSheet;

    public void Init()
    {
        jsonDataSheet = new Dictionary<string, string>();
        jsonFile = Resources.Load("TextAsset/ItemDatas/typeToItemName") as TextAsset;
        typeToItemNameJsonObj = new JSONObject(jsonFile.text);
        AccessData(typeToItemNameJsonObj);
    }

    public string GetTypeToItemName(string _type)
    {
        string itemName;
        jsonDataSheet.TryGetValue(_type, out itemName);
        return itemName;
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                for (int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    jsonDataSheet.Add(jsonObj.keys[idx], jsonObj.list[idx].str);
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
