using HMWGameServer.ServerSoruces.DataFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MapGenLib;

namespace HMWGameServer
{
    [Serializable]
    class WorldArea
    {
        public string UniqueID;
        public int OffsetX, OffsetY, OffsetZ;
        public string AreaName;
        public int AreaSizeX;
        public int AreaSizeZ;
        public Dictionary<string, SubWorld> SubWorlds;
    }
    [Serializable]
    class SubWorld
    {
        public string UniqueID;
        public int OffsetX, OffsetY, OffsetZ;
        public string WorldName;
        public bool bSurface;
        public Block[,,] Blocks;
    }
    [Serializable]
    class SubWorldDataFileFormat
    {
        public string AreaID;
        public string SubWorldID;
        public Block[,,] Blocks;
    }
   
    struct SubWorldBlockPacketData
    {
        // 실제 패킷 데이터.
        public string AreaID;
        public string SubWorldID;
        public int BlockIndex_X;
        public int BlockIndex_Y;
        public int BlockIndex_Z;
        public byte BlockTypeValue;
        public byte OwnerChunkType;
        // 서버에서 기록하는 타임스탬프.
        //public long TimeStampTicks;


        public static bool operator ==(SubWorldBlockPacketData a, SubWorldBlockPacketData b)
        {
            bool bAreaID = a.AreaID == b.AreaID;
            bool bSubWorldID = a.SubWorldID == b.SubWorldID;
            bool bIndexX = a.BlockIndex_X == b.BlockIndex_X;
            bool bIndexY = a.BlockIndex_Y == b.BlockIndex_Y;
            bool bIndexZ = a.BlockIndex_Z == b.BlockIndex_Z;

            return bAreaID ^ bSubWorldID ^ bIndexX ^ bIndexY ^ bIndexZ;
        }
        public static bool operator !=(SubWorldBlockPacketData a, SubWorldBlockPacketData b)
        {
            bool bAreaID = a.AreaID == b.AreaID;
            bool bSubWorldID = a.SubWorldID == b.SubWorldID;
            bool bIndexX = a.BlockIndex_X == b.BlockIndex_X;
            bool bIndexY = a.BlockIndex_Y == b.BlockIndex_Y;
            bool bIndexZ = a.BlockIndex_Z == b.BlockIndex_Z;

            return !(bAreaID ^ bSubWorldID ^ bIndexX ^ bIndexY ^ bIndexZ);
        }

    }
    class GameWorldMapManager
    {
        private object LockObject = null;
        private static GameWorldMapManager Instance;
        public static GameWorldMapManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new GameWorldMapManager();
            }
            return Instance;
        }

        public int RandomSeedValue { get; private set; } = 1;
        public bool bInitMakeWorldMap = false;

        public Dictionary<string, WorldArea> WorldAreaMap = new Dictionary<string, WorldArea>();

        private GameWorldMapManager()
        {
            LockObject = new object();
        }

        public async void AsyncMakeMap()
        {
            GameLogger.SimpleConsoleWriteLineNoFileInfo("Start Async MakeMap.");
            var watch = System.Diagnostics.Stopwatch.StartNew();
            bInitMakeWorldMap = await MakeWorldMap();
            watch.Stop();
            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("Finish Async MakeMap. (takes time : {0} ms)", watch.ElapsedMilliseconds));
        }

        private async Task<bool> MakeWorldMap()
        {
            return await Task.Run(() =>
            {
                ConfigFileStruct config = GameConfigDataFile.GetInstance().GetConfig();
                // init
                WorldAreaMap = new Dictionary<string, WorldArea>();
                // allocate map.
                WorldMapData mapData = GameWorldMapDataFile.GetInstance().MapData;
                foreach(WorldAreaTerrainData terrainData in mapData.WorldAreaDatas)
                {
                    WorldArea areaInstance = new WorldArea();
                    areaInstance.UniqueID = Utils.MakeUniqueID(terrainData.OffsetX, terrainData.OffsetY, terrainData.OffsetZ);
                    areaInstance.AreaName = terrainData.AreaName;
                    areaInstance.OffsetX = terrainData.OffsetX;
                    areaInstance.OffsetY = terrainData.OffsetY;
                    areaInstance.OffsetZ = terrainData.OffsetZ;
                    areaInstance.AreaSizeX = config.SubWorldRow * config.SubWorldSizeX; ;
                    areaInstance.AreaSizeZ = config.SubWorldColumn * config.SubWorldSizeZ;
                    areaInstance.SubWorlds = new Dictionary<string, SubWorld>();
                    foreach(var subWorldData in terrainData.SubWorldDatas)
                    {
                        SubWorld subWorldInst = new SubWorld();
                        subWorldInst.UniqueID = subWorldData.UniqueID;
                        subWorldInst.WorldName = subWorldData.WorldName;
                        subWorldInst.OffsetX = subWorldData.OffsetX;
                        subWorldInst.OffsetY = subWorldData.OffsetY;
                        subWorldInst.OffsetZ = subWorldData.OffsetZ;
                        subWorldInst.bSurface = subWorldData.bSurface;
                        subWorldInst.Blocks = new Block[config.SubWorldSizeX, config.SubWorldSizeY, config.SubWorldSizeZ];
                        //
                        areaInstance.SubWorlds.Add(subWorldInst.UniqueID, subWorldInst);
                    }
                    WorldAreaMap.Add(areaInstance.UniqueID, areaInstance);
                }

                // setting data.
                foreach(var pair in WorldAreaMap)
                {
                    string areaUniqueID = pair.Key;
                    WorldArea worldAreaInst = pair.Value;
                    if(worldAreaInst != null)
                    {
                        var normalTerrainData = WorldGenAlgorithms.GenerateNormalTerrain(worldAreaInst.AreaSizeX, worldAreaInst.AreaSizeZ,
                                                                                         config.SubWorldLayer, config.SubWorldSizeY, Utils.GetSeed());
                        foreach(var subWorldPair in worldAreaInst.SubWorlds)
                        {
                            string subWorldUniqueID = subWorldPair.Key;
                            SubWorld subWorldInst = subWorldPair.Value;
                            if(subWorldInst != null)
                            {
                                for (int x = 0; x < subWorldInst.Blocks.GetLength(0); x++)
                                {
                                    for (int z = 0; z < subWorldInst.Blocks.GetLength(2); z++)
                                    {
                                        int mapX = (subWorldInst.OffsetX * config.SubWorldSizeX) + x;
                                        int mapZ = (subWorldInst.OffsetZ * config.SubWorldSizeZ) + z;
                                        WorldGenAlgorithms.TerrainValue terrainValue = normalTerrainData[mapX, mapZ];
                                        int rangeY = terrainValue.Layers[subWorldInst.OffsetY];
                                        byte blockType = (byte)terrainValue.BlockType;
                                        for (int y = 0; y < rangeY; y++)
                                        {
                                            // 블록 타입 세팅.
                                            subWorldInst.Blocks[x, y, z].CurrentType = blockType;
                                            subWorldInst.Blocks[x, y, z].OriginalType = blockType;
                                            // 블록 내구도 세팅.
                                            //BlockTileInfo blockTypeInfo = BlockTileDataFile.Instance.GetBlockTileInfo((BlockTileType)blockType);
                                            //subWorldInst.Blocks[x, y, z].Durability = blockTypeInfo.Durability;
                                        }
                                    }
                                }
                                // save.
                                SaveSubWorldFile(subWorldInst, worldAreaInst.UniqueID);
                            }
                        }
                    }
                }
                return true;
            });
        }

        private SubWorldDataFileFormat LoadSubWorldFile(string filePath)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            var worldData = bf.Deserialize(fileStream) as SubWorldDataFileFormat;
            fileStream.Close();
            return worldData;
        }

        public List<byte[]> GetSubWorldBytesList()
        {
            List<byte[]> list = new List<byte[]>();
            foreach(var data in WorldAreaMap)
            {
                WorldArea worldAreaInst = data.Value;
                foreach(var subWorldData in worldAreaInst.SubWorlds)
                {
                    SubWorld subWorldInst = subWorldData.Value;
                    list.Add(GetSubWorldFileBytes(worldAreaInst.UniqueID, subWorldInst.UniqueID));
                }
            }
            return list;
        }

        private byte[] GetSubWorldFileBytes(string areaUniqueID, string subWorldUniqueID)
        {
            string loadPath = Path.Combine(ConstFilePath.RAW_SUB_WORLD_DATA_PATH, string.Format("Area({0})_SubWorld({1}).MapFile",
                                            Utils.ConvertUniqueIDToFileName(areaUniqueID), Utils.ConvertUniqueIDToFileName(subWorldUniqueID)));
            FileStream fileStream = File.Open(loadPath, FileMode.OpenOrCreate, FileAccess.Read);
            using (var memoryStream = new MemoryStream())
            {
                fileStream.CopyTo(memoryStream);
                fileStream.Close();
                return memoryStream.ToArray();
            }
        }

        private string SaveSubWorldFile(SubWorld subWorld, string areaID)
        {
            Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
            string savePath = Path.Combine(ConstFilePath.RAW_SUB_WORLD_DATA_PATH, string.Format("Area({0})_SubWorld({1}).MapFile",
                Utils.ConvertUniqueIDToFileName(areaID), Utils.ConvertUniqueIDToFileName(subWorld.UniqueID)));
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);

            SubWorldDataFileFormat dataFile = new SubWorldDataFileFormat
            {
                Blocks = subWorld.Blocks,
                AreaID = areaID,
                SubWorldID = subWorld.UniqueID
            };
            // 시리얼라이징.
            bf.Serialize(fileStream, dataFile);
            fileStream.Close();
            // return path.
            return savePath;
        }

        public bool AddSubWorldData(SubWorldBlockPacketData packetData)
        {
            lock(LockObject)
            {
                WorldAreaMap.TryGetValue(packetData.AreaID, out WorldArea worldArea);
                if(worldArea != null)
                {
                    worldArea.SubWorlds.TryGetValue(packetData.SubWorldID, out SubWorld subWorld);
                    subWorld.Blocks[packetData.BlockIndex_X, packetData.BlockIndex_Y, packetData.BlockIndex_Z].CurrentType = packetData.BlockTypeValue;
                    // save subworld.
                    SaveSubWorldFile(subWorld, packetData.AreaID);
                    //
                    return true;
                }
                else
                {
                    Console.WriteLine("[ERROR] AddSubWorldData - WorldArea is Null");
                    return false;
                }
            }
        }

    }
}
