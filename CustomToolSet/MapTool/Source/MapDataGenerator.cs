using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace MapTool.Source
{
    public struct WorldMapData
    {
        public int WorldAreaRow;
        public int WorldAreaColumn;
        public int WorldAreaLayer;
        //
        public int SubWorldRow;
        public int SubWorldColumn;
        public int SubWorldLayer;
    }

    public struct WorldMapConfigData
    {
        public int SubWorldSizeX;
        public int SubWorldSizeY;
        public int SubWorldSizeZ;
        //
        public int SubWorld_Count_X_Axis_Per_WorldArea;
        public int SubWorld_Count_Y_Axis_Per_WorldArea;
        public int SubWorld_Count_Z_Axis_Per_WorldArea;
        //
        public int ChunkSize;
        public float ChunkLoadIntervalSeconds;
        public float OneTileUnit;
    }
        
    class MapDataGenerator
    {
        #region defualt values.
        public static readonly int  DefaultSubWorldRowValue = 2;
        public static readonly int  DefaultSubWorldColumnValue = 2;
        public static readonly int  DefaultSubWorldLayerValue = 2;
        //
        public static readonly int DefaultWorldAreaRowValue = 1;
        public static readonly int DefaultWorldAreaColumnValue = 1;
        public static readonly int DefaultWorldAreaLayerValue = 1;
        //
        #endregion

        #region WorldMap Config Information.
        public static readonly int SubWorldSizeX = 32;
        public static readonly int SubWorldSizeY = 32;
        public static readonly int SubWorldSizeZ = 32;
        //
        public static readonly int SubWorld_Count_X_Axis_Per_WorldArea = 32;
        public static readonly int SubWorld_Count_Y_Axis_Per_WorldArea = 32;
        public static readonly int SubWorld_Count_Z_Axis_Per_WorldArea = 32;
        //
        public static readonly int ChunkSize = 8;
        public static readonly float ChunkLoadIntervalSeconds = 0.01f;
        public static readonly float OneTileUnit = 0.0625f;
        #endregion

        private readonly int NumberOfGenerateTimes = 800;

        private struct Properties
        {
            public int WorldAreaRow;
            public int WorldAreaColumn;
            public int WorldAreaLayer;

            public int SubWorldRow;
            public int SubWorldColumn;
            public int SubWorldLayer;
        }

        private class WorldAreaData
        {
            public string UNIQUE_ID;
            public string OFFSET_X;
            public string OFFSET_Y;
            public string OFFSET_Z;
            public string AREA_NAME;
            public List<SubWorldData> SubWorldDatas = new List<SubWorldData>();
        }
       
        private struct SubWorldData
        {
            public string UNIQUE_ID;
            public string OFFSET_X;
            public string OFFSET_Y;
            public string OFFSET_Z;
            public string WORLD_NAME;
            public string IS_SURFACE;
        }

        private WorldMapData WorldMapDataInstance;

        class SubWorldJsonFileData
        {
            public Properties Properties;
            public List<WorldAreaData> WorldAreaDatas = new List<WorldAreaData>();
        }


        public void Init(WorldMapData genData)
        {
            WorldMapDataInstance = genData;
        }

        private bool GenerateWorldMapConfigData()
        {
            WorldMapConfigData configData;
            configData.SubWorldSizeX = MapDataGenerator.SubWorldSizeX;
            configData.SubWorldSizeY = MapDataGenerator.SubWorldSizeY;
            configData.SubWorldSizeZ = MapDataGenerator.SubWorldSizeZ;
            configData.SubWorld_Count_X_Axis_Per_WorldArea = MapDataGenerator.SubWorld_Count_X_Axis_Per_WorldArea;
            configData.SubWorld_Count_Y_Axis_Per_WorldArea = MapDataGenerator.SubWorld_Count_Y_Axis_Per_WorldArea;
            configData.SubWorld_Count_Z_Axis_Per_WorldArea = MapDataGenerator.SubWorld_Count_Z_Axis_Per_WorldArea;
            configData.ChunkLoadIntervalSeconds = MapDataGenerator.ChunkLoadIntervalSeconds;
            configData.ChunkSize = MapDataGenerator.ChunkSize;
            configData.OneTileUnit = MapDataGenerator.OneTileUnit;

            File.WriteAllText(MapToolPath.WorldConfigJsonFilePath, JsonConvert.SerializeObject(configData, Formatting.Indented));
            return true;
        }

        private bool GenerateSubWorldDatas()
        {
            SubWorldJsonFileData jsonFileData = new SubWorldJsonFileData();
            jsonFileData.Properties.SubWorldRow = WorldMapDataInstance.SubWorldRow;
            jsonFileData.Properties.SubWorldColumn = WorldMapDataInstance.SubWorldColumn;
            jsonFileData.Properties.SubWorldLayer = WorldMapDataInstance.SubWorldLayer;
            jsonFileData.Properties.WorldAreaRow = WorldMapDataInstance.WorldAreaRow;
            jsonFileData.Properties.WorldAreaColumn = WorldMapDataInstance.WorldAreaColumn;
            jsonFileData.Properties.WorldAreaLayer = WorldMapDataInstance.WorldAreaLayer;

            int areaIndex = 0;
            for(int x = 0; x < WorldMapDataInstance.WorldAreaRow; x++)
            {
                for (int y = 0; y < WorldMapDataInstance.WorldAreaLayer; y++)
                {
                    for (int z = 0; z < WorldMapDataInstance.WorldAreaColumn; z++)
                    {
                        WorldAreaData worldArea = new WorldAreaData
                        {
                            UNIQUE_ID = string.Format("unique_{0}:{1}:{2}", x.ToString(), y.ToString(), z.ToString()),
                            OFFSET_X = x.ToString(),
                            OFFSET_Y = y.ToString(),
                            OFFSET_Z = z.ToString(),
                            AREA_NAME = string.Format("WORLD_AREA_{0}", areaIndex),
                        };
                        jsonFileData.WorldAreaDatas.Add(worldArea);
                        CreateSubWorlds(jsonFileData, areaIndex);
                        areaIndex++;
                    }
                }
            }
            
            File.WriteAllText(MapToolPath.SubWorldJsonFilePath, JsonConvert.SerializeObject(jsonFileData, Formatting.Indented));
            
            return true;
        }

        private void CreateSubWorlds(SubWorldJsonFileData jsonFileData, int areaIndex)
        {
            //
            // 서브 월드 정보 생성.
            //
            int worldIndex = 0;
            for (int x = 0; x < WorldMapDataInstance.SubWorldRow; x++)
            {
                for (int y = 0; y < WorldMapDataInstance.SubWorldLayer; y++)
                {
                    for (int z = 0; z < WorldMapDataInstance.SubWorldColumn; z++)
                    {
                        SubWorldData subWorldData = new SubWorldData
                        {
                            UNIQUE_ID = string.Format("unique_{0}:{1}:{2}", x.ToString(), y.ToString(), z.ToString()),
                            OFFSET_X = x.ToString(),
                            OFFSET_Y = y.ToString(),
                            OFFSET_Z = z.ToString(),
                            WORLD_NAME = string.Format("AREA_{0}_SUB_WORLD_{1}", areaIndex, worldIndex),
                        };
                        if (y == (WorldMapDataInstance.SubWorldLayer - 1))
                        {
                            subWorldData.IS_SURFACE = true.ToString();
                        }
                        else
                        {
                            subWorldData.IS_SURFACE = false.ToString();
                        }
                        jsonFileData.WorldAreaDatas[areaIndex].SubWorldDatas.Add(subWorldData);
                        worldIndex++;
                    }
                }
            }
        }

        public bool Generate()
        {
            return GenerateSubWorldDatas() && GenerateWorldMapConfigData();
        }
    }
}
