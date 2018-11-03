using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SubWorldData
{
    public int worldIdx;
    public string worldName;
    public int x;
    public int z;
}
/// <summary>
/// World 생성에 쓰이는 DataFile.
/// </summary>
public class SubWorldDataFile : MonoBehaviour
{
    private JSONObject subWorldJsonObj;
    private TextAsset jsonFile;
    private List<Dictionary<string, string>> jsonDataSheet;
    public List<SubWorldData> subWorldDataList { get; } = new List<SubWorldData>();
    public int rowOffset { get; private set; }


    public static SubWorldDataFile instance = null;

    public void Init()
    {
        jsonDataSheet = new List<Dictionary<string, string>>();
        jsonFile = Resources.Load(ConstFilePath.TXT_SUB_WORLD_DEFAULT_DATAS) as TextAsset;
        subWorldJsonObj = new JSONObject(jsonFile.text);
        AccessData(subWorldJsonObj);

        if (instance == null) instance = this;
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                //to do
                break;
            case JSONObject.Type.ARRAY:
                // row offset 값 가져오기.
                string offset;
                jsonObj.list[0].ToDictionary().TryGetValue("ROW_OFFSET", out offset);
                rowOffset = int.Parse(offset);
                // 서브월드 정보 오브젝트들을 각각의 딕셔너리로 변환.
                for (int idx = 1; idx < jsonObj.Count; ++idx)
                {
                    var extractedData = jsonObj.list[idx].ToDictionary();

                    SubWorldData subWorldData;
                    string getValue;
                    extractedData.TryGetValue("WORLD_IDX", out getValue);
                    subWorldData.worldIdx = int.Parse(getValue);
                    extractedData.TryGetValue("WORLD_NAME", out getValue);
                    subWorldData.worldName = getValue;
                    extractedData.TryGetValue("X", out getValue);
                    subWorldData.x = int.Parse(getValue);
                    extractedData.TryGetValue("Z", out getValue);
                    subWorldData.z = int.Parse(getValue);

                    subWorldDataList.Add(subWorldData);
                }
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}
