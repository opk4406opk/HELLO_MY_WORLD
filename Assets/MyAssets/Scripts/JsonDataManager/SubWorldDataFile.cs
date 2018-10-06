using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SubWorldData
{
    public string worldName;
    public int x;
    public int z;
    public int idx;
}
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

    private int _rowOffset;
    public int rowOffset
    {
        get { return _rowOffset; }
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

    public int GetPosValue(int idx, string axis)
    {
        string value;
        jsonDataSheet[idx].TryGetValue(axis, out value);
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
                // ROW_OFFSET 정보를 뺀 나머지 오브젝트들의 갯수.
                _maxSubWorld = jsonObj.Count - 1;
                // row offset 값 가져오기.
                string offset;
                jsonObj.list[0].ToDictionary().TryGetValue("ROW_OFFSET", out offset);
                _rowOffset = int.Parse(offset);
                // 서브월드 정보 오브젝트들을 각각의 딕셔너리로 변환.
                for (int idx = 1; idx < jsonObj.Count; ++idx)
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
