using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// World 생성에 쓰이는 DataFile.
/// </summary>
public class SubWorldDataFile : MonoBehaviour
{
    private JSONObject subWorldJsonObj;
    private TextAsset jsonFile;
    private List<Dictionary<string, string>> jsonDataSheet;

    private int _maxSubWorld = 0;
    public int maxSubWorld
    {
        get { return _maxSubWorld; }
    }

    public static SubWorldDataFile instance = null;

    public void Init()
    {
        jsonDataSheet = new List<Dictionary<string, string>>();
        jsonFile = Resources.Load(ConstFilePath.TXT_SUB_WORLD_DEFAULT_DATAS) as TextAsset;
        subWorldJsonObj = new JSONObject(jsonFile.text);
        AccessData(subWorldJsonObj);

        if (instance == null) instance = this;
    }

    public int GetPosValue(int idx, string str)
    {
        string value;
        jsonDataSheet[idx].TryGetValue(str, out value);
        return int.Parse(value);
    }
    public string GetWorldName(int idx, string str)
    {
        string value;
        jsonDataSheet[idx].TryGetValue(str, out value);
        return value;
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                //to do
                break;
            case JSONObject.Type.ARRAY:
                _maxSubWorld = jsonObj.Count;
                for (int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    jsonDataSheet.Add(jsonObj.list[idx].ToDictionary());
                }
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
