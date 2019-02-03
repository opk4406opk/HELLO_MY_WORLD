using UnityEngine;
using System.Collections.Generic;

public class WorldMapData
{
    public List<SubWorldData> SubWorldDatas = new List<SubWorldData>();
    public int Row;
    public int Column;
    public int Layer;
}

public struct SubWorldData
{
    public int WorldIdx;
    public string WorldName;
    public int X;
    public int Y;
    public int Z;
    public bool IsSurface;
}


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
/// World 생성에 쓰이는 DataFile.
/// </summary>
public class WorldMapDataFile
{
    private JSONObject SubWorldJsonObj;
    private TextAsset JsonFile;
    private List<Dictionary<string, string>> JsonDataSheet;
    public WorldMapData WorldMapData { get; private set; } = new WorldMapData();
    //
    public static WorldMapDataFile instance = null;

    public void Init()
    {
        JsonDataSheet = new List<Dictionary<string, string>>();
        JsonFile = Resources.Load(ConstFilePath.TXT_WORLD_MAP_DATAS) as TextAsset;
        SubWorldJsonObj = new JSONObject(JsonFile.text);
        AccessData(SubWorldJsonObj);

        if (instance == null) instance = this;
    }

    private void AccessData(JSONObject jsonObj)
    {
        var properties = jsonObj.list[0].ToDictionary();
        var subWorldDatas = jsonObj.list[1].list;
        //
        string extractedData;
        properties.TryGetValue("ROW", out extractedData);
        WorldMapData.Row = int.Parse(extractedData);
        properties.TryGetValue("COLUMN", out extractedData);
        WorldMapData.Column = int.Parse(extractedData);
        properties.TryGetValue("LAYER", out extractedData);
        WorldMapData.Layer = int.Parse(extractedData);
        //
        foreach(var subworld in subWorldDatas)
        {
            SubWorldData subWorldData = new SubWorldData();
            //
            string val;
            var data = subworld.ToDictionary();
            data.TryGetValue("WORLD_IDX", out val);
            subWorldData.WorldIdx = int.Parse(val);
            //
            data.TryGetValue("WORLD_NAME", out val);
            subWorldData.WorldName = val;
            //
            data.TryGetValue("X", out val);
            subWorldData.X = int.Parse(val);
            //
            data.TryGetValue("Y", out val);
            subWorldData.Y = int.Parse(val);
            //
            data.TryGetValue("Z", out val);
            subWorldData.Z = int.Parse(val);
            //
            data.TryGetValue("IS_SURFACE", out val);
            if(val == "False")
            {
                subWorldData.IsSurface = false;
            }
            else
            {
                subWorldData.IsSurface = true;
            }
            WorldMapData.SubWorldDatas.Add(subWorldData);
        }
    }
}
