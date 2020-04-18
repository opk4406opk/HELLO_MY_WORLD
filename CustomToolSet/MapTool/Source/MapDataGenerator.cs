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

    public struct ServerWorldMapConfigData
    {
        public int WorldAreaRow;
        public int WorldAreaColumn;
        public int WorldAreaLayer;
        public int SubWorldRow;
        public int SubWorldColumn;
        public int SubWorldLayer;
        public int SubWorldSizeX;
        public int SubWorldSizeY;
        public int SubWorldSizeZ;
        public int ChunkSize;
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
            public string UniqueID;
            public string OffsetX;
            public string OffsetY;
            public string OffsetZ;
            public string AreaName;
            public List<SubWorldData> SubWorldDatas = new List<SubWorldData>();
        }
       
        private struct SubWorldData
        {
            public string UniqueID;
            public string OffsetX;
            public string OffsetY;
            public string OffsetZ;
            public string WorldName;
            public string bSurface;
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

            File.WriteAllText(MapToolPath.ClientWorldConfigFilePath, JsonConvert.SerializeObject(configData, Formatting.Indented));
            return true;
        }

        private bool GenerateServerWorldMapConfigData()
        {
            ServerWorldMapConfigData serverConfigData;
            serverConfigData.WorldAreaColumn = WorldMapDataInstance.WorldAreaColumn;
            serverConfigData.WorldAreaLayer = WorldMapDataInstance.WorldAreaLayer;
            serverConfigData.WorldAreaRow = WorldMapDataInstance.WorldAreaRow;
            serverConfigData.SubWorldColumn = WorldMapDataInstance.SubWorldColumn;
            serverConfigData.SubWorldLayer = WorldMapDataInstance.SubWorldLayer;
            serverConfigData.SubWorldRow = WorldMapDataInstance.SubWorldRow;
            serverConfigData.SubWorldSizeX = MapDataGenerator.SubWorldSizeX;
            serverConfigData.SubWorldSizeY = MapDataGenerator.SubWorldSizeY;
            serverConfigData.SubWorldSizeZ = MapDataGenerator.SubWorldSizeZ;
            serverConfigData.ChunkSize = MapDataGenerator.ChunkSize;

            File.WriteAllText(MapToolPath.ServerWorldConfigFilePath, JsonConvert.SerializeObject(serverConfigData, Formatting.Indented));
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
                            UniqueID = string.Format("unique_{0}:{1}:{2}", x.ToString(), y.ToString(), z.ToString()),
                            OffsetX = x.ToString(),
                            OffsetY = y.ToString(),
                            OffsetZ = z.ToString(),
                            AreaName = string.Format("WORLD_AREA_{0}", areaIndex),
                        };
                        jsonFileData.WorldAreaDatas.Add(worldArea);
                        CreateSubWorlds(jsonFileData, areaIndex);
                        areaIndex++;
                    }
                }
            }
            
            File.WriteAllText(MapToolPath.SubWorldFilePath, JsonConvert.SerializeObject(jsonFileData, Formatting.Indented));
            File.WriteAllText(MapToolPath.ServerSubWorldFilePath, JsonConvert.SerializeObject(jsonFileData, Formatting.Indented));
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
                            UniqueID = string.Format("unique_{0}:{1}:{2}", x.ToString(), y.ToString(), z.ToString()),
                            OffsetX = x.ToString(),
                            OffsetY = y.ToString(),
                            OffsetZ = z.ToString(),
                            WorldName = string.Format("AREA_{0}_SUB_WORLD_{1}", areaIndex, worldIndex),
                        };
                        if (y == (WorldMapDataInstance.SubWorldLayer - 1))
                        {
                            subWorldData.bSurface = true.ToString();
                        }
                        else
                        {
                            subWorldData.bSurface = false.ToString();
                        }
                        jsonFileData.WorldAreaDatas[areaIndex].SubWorldDatas.Add(subWorldData);
                        worldIndex++;
                    }
                }
            }
        }

        public bool Generate()
        {
            return GenerateSubWorldDatas() && GenerateWorldMapConfigData() && GenerateServerWorldMapConfigData();
        }
    }
}
