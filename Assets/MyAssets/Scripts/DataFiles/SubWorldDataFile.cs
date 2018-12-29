using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct SubWorldData
{
    public int worldIdx;
    public string worldName;
    public int x;
    public int y;
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

    // subworld 들은 아래와 같은 배열형식으로 구성. 
    // [0][1][2]   -- 1행
    // [3][4][5]   -- 2행
    // [6][7][N]...-- 3행
    //  각 행(row)을 구분지어서 계산하기 위해 offset값을 따로 설정해서 사용.
    // 여기서는 1행에 3개씩의 서브월드들이 존재하므로 offset값은 3이다.
    // 그리고 위와 같은 배열들이 N개의 Layer로 구성되어 겹겹이 쌓아져서 구성된다. ( 2차원 배열형식)
    //         
    //             ______________________
    //            /_/_/_/_/_/_/_/_/_/_/_/| 
    //           /_/_/_/_/_/_/_/_/_/_/_/ |
    //          /_/_/_/_/_/_/_/_/_/_/_/ /|
    //          |_____________________|///
    //          |_____________________|//
    //          |_____________________|/
    //            
    //       Y Offset값은 1개의 Layer에 존재하는 원소갯수만큼을 곱해야 한다. 
    //      만약 1번 Layer의 있는 원소가 9개라면 YOffset * 9 

    /// <summary>
    /// 각 서브월드들이 1개의 행에 몇개씩 있는지 판별하는 offset 값.
    /// </summary>
    public int rowOffset { get; private set; }
    /// <summary>
    /// 1개의 레이어에 존재하는 원소의 개수.
    /// </summary>
    public int elements_per_layer { get; private set; }

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
                string elementNum;
                jsonObj.list[1].ToDictionary().TryGetValue("NUM_PER_LAYER", out elementNum);
                elements_per_layer = int.Parse(elementNum);

                // 서브월드 정보 오브젝트들을 각각의 딕셔너리로 변환.
                for (int idx = 2; idx < jsonObj.Count; ++idx)
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
                    extractedData.TryGetValue("Y", out getValue);
                    subWorldData.y = int.Parse(getValue);
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
