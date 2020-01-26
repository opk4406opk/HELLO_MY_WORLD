using UnityEngine;
using System.Collections.Generic;

public class WorldMapData
{
    public List<WorldAreaTerrainData> WorldAreaDatas = new List<WorldAreaTerrainData>();
   
    public int WorldAreaRow;
    public int WorldAreaColumn;
    public int WorldAreaLayer;

    public int SubWorldRow;
    public int SubWorldColumn;
    public int SubWorldLayer;
}

public class WorldAreaTerrainData
{
    public string UniqueID;
    public int OffsetX, OffsetY, OffsetZ;
    public string AreaName;
    public List<SubWorldData> SubWorldDataList = new List<SubWorldData>();
}

public struct SubWorldData
{
    public string UniqueID;
    public int OffsetX, OffsetY, OffsetZ;
    public string WorldName;
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
public class WorldMapDataFile : BaseDataFile
{
    private List<Dictionary<string, string>> JsonDataSheet;
    public WorldMapData MapData { get; private set; } = new WorldMapData();
    //
    public static WorldMapDataFile Instance = null;

    public override void Init()
    {
        JsonDataSheet = new List<Dictionary<string, string>>();
        JsonFile = Resources.Load(ConstFilePath.TXT_RESOURCE_WORLD_MAP_DATAS) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);

        if (Instance == null) Instance = this;
    }

    protected override void AccessData(JSONObject jsonObj)
    {
        var properties = jsonObj.list[0].ToDictionary();
        var worldAreaList = jsonObj.list[1].list;
        //
        string extractedData;
        properties.TryGetValue("WorldAreaRow", out extractedData);
        MapData.WorldAreaRow = int.Parse(extractedData);
        properties.TryGetValue("WorldAreaColumn", out extractedData);
        MapData.WorldAreaColumn = int.Parse(extractedData);
        properties.TryGetValue("WorldAreaLayer", out extractedData);
        MapData.WorldAreaLayer = int.Parse(extractedData);
        //
        properties.TryGetValue("SubWorldRow", out extractedData);
        MapData.SubWorldRow = int.Parse(extractedData);
        properties.TryGetValue("SubWorldColumn", out extractedData);
        MapData.SubWorldColumn = int.Parse(extractedData);
        properties.TryGetValue("SubWorldLayer", out extractedData);
        MapData.SubWorldLayer = int.Parse(extractedData);
        //
        foreach(var worldArea in worldAreaList)
        {
            WorldAreaTerrainData worldAreaData = new WorldAreaTerrainData();
            var data = worldArea.ToDictionary();
            string val;
            data.TryGetValue("AREA_NAME", out val);
            worldAreaData.AreaName = val;
            data.TryGetValue("OFFSET_X", out val);
            worldAreaData.OffsetX = int.Parse(val);
            data.TryGetValue("OFFSET_Y", out val);
            worldAreaData.OffsetY = int.Parse(val);
            data.TryGetValue("OFFSET_Z", out val);
            worldAreaData.OffsetZ = int.Parse(val);
            data.TryGetValue("UNIQUE_ID", out val);
            worldAreaData.UniqueID = val;
            // 데이터 리스트에 마지막 원소가 SubWorld 데이터 리스트임.
            var subWorldDataList = worldArea.list[worldArea.list.Count - 1].list;
            foreach (var subWorld in subWorldDataList)
            {
                SubWorldData subWorldData = new SubWorldData();
                //
                var map = subWorld.ToDictionary();
                string outValue;
                map.TryGetValue("WORLD_NAME", out outValue);
                subWorldData.WorldName = outValue;
                //
                map.TryGetValue("OFFSET_X", out outValue);
                subWorldData.OffsetX = int.Parse(outValue);
                map.TryGetValue("OFFSET_Y", out outValue);
                subWorldData.OffsetY = int.Parse(outValue);
                map.TryGetValue("OFFSET_Z", out outValue);
                subWorldData.OffsetZ = int.Parse(outValue);
                //
                map.TryGetValue("UNIQUE_ID", out outValue);
                subWorldData.UniqueID = outValue;
                //
                map.TryGetValue("IS_SURFACE", out val);
                if (val == "False")
                {
                    subWorldData.IsSurface = false;
                }
                else
                {
                    subWorldData.IsSurface = true;
                }
                worldAreaData.SubWorldDataList.Add(subWorldData);
            }
            MapData.WorldAreaDatas.Add(worldAreaData);
        }
    }
}
