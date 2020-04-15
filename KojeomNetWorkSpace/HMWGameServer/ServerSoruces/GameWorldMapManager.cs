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
        public int AreaSizeX;
        public int AreaSizeZ;
        public Dictionary<string, SubWorld> SubWorlds;
    }
    [Serializable]
    class SubWorld
    {
        public string UniqueID;
        public Block[,,] Blocks;
    }
    [Serializable]
    class SubWorldDataFileFormat
    {
        public string AreaID;
        public string SubWorldID;
        public Block[,,] Blocks;
    }
    struct SubWorldPacketData
    {
        public int Size;
        public byte[] SubWorldDataFileBytes;
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
                // make.
                for (int areaX = 0; areaX < GameConfigDataFile.GetInstance().GetConfig().WorldAreaRow; ++areaX)
                {
                    for (int areaY = 0; areaY < GameConfigDataFile.GetInstance().GetConfig().WorldAreaLayer; ++areaY)
                    {
                        for (int areaZ = 0; areaZ < GameConfigDataFile.GetInstance().GetConfig().WorldAreaColumn; ++areaZ)
                        {
                            WorldArea worldArea = new WorldArea();
                            worldArea.UniqueID = Utils.MakeUniqueID(areaX, areaY, areaZ);
                            worldArea.AreaSizeX = config.SubWorldRow * config.SubWorldSizeX;
                            worldArea.AreaSizeZ = config.SubWorldColumn * config.SubWorldSizeZ;
                            worldArea.SubWorlds = new Dictionary<string, SubWorld>();
                            List<SubWorld> subWorldList = MakeDefaultSubWorld(worldArea.UniqueID);
                            foreach (var subWorld in subWorldList)
                            {
                                worldArea.SubWorlds.Add(subWorld.UniqueID, subWorld);
                            }
                            WorldAreaMap.Add(worldArea.UniqueID, worldArea);
                        }
                    }
                }
                // setting data.
                foreach(var pair in WorldAreaMap)
                {
                    string areaUniqueID = pair.Key;
                    WorldArea worldAreaInst = pair.Value;
                    if(worldAreaInst != null)
                    {
                        var normalTerrainData = WorldGenAlgorithms.GenerateNormalTerrain(worldAreaInst.AreaSizeX, worldAreaInst.AreaSizeZ,
                                                                                         config.SubWorldLayer, config.SubWorldSizeY);
                        foreach(var subWorldPair in worldAreaInst.SubWorlds)
                        {
                            string subWorldUniqueID = subWorldPair.Key;
                            SubWorld subWorldInst = subWorldPair.Value;
                            if(subWorldInst != null)
                            {
                                //int rangeY = normalTerrainData[x,z].Layers[(int)OffsetCoordinate.y];
                                for (int y = 0; y < subWorldInst.Blocks.GetLength(1); y++)
                                {
                                    for (int x = 0; x < subWorldInst.Blocks.GetLength(0); x++)
                                    {
                                        for (int z = 0; z < subWorldInst.Blocks.GetLength(2); z++)
                                        {
                                            //subWorldInst.Blocks[x,y,z].Type = normalTerrainData[x, z].
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            });
        }

        private List<SubWorld> MakeDefaultSubWorld(string areaID)
        {
            lock(LockObject)
            {
                List<SubWorld> subworldList = new List<SubWorld>();
                for (int x = 0; x < GameConfigDataFile.GetInstance().GetConfig().SubWorldRow; ++x)
                {
                    for (int y = 0; y < GameConfigDataFile.GetInstance().GetConfig().SubWorldLayer; ++y)
                    {
                        for (int z = 0; z < GameConfigDataFile.GetInstance().GetConfig().SubWorldColumn; ++z)
                        {
                            SubWorld subWorld = new SubWorld();
                            subWorld.UniqueID = Utils.MakeUniqueID(x, y, z);
                            subWorld.Blocks = new Block[GameConfigDataFile.GetInstance().GetConfig().SubWorldSizeX,
                                                       GameConfigDataFile.GetInstance().GetConfig().SubWorldSizeY,
                                                       GameConfigDataFile.GetInstance().GetConfig().SubWorldSizeZ];
                            subworldList.Add(subWorld);
                        }
                    }
                }
                return subworldList;
            }
        }

        private SubWorldDataFileFormat LoadSubWorldFile(string filePath)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            var worldData = bf.Deserialize(fileStream) as SubWorldDataFileFormat;
            fileStream.Close();
            return worldData;
        }

        private string SaveSubWorldFile(SubWorld subWorld, string AreaID)
        {
            Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
            string savePath = Path.Combine(ConstFilePath.RAW_SUB_WORLD_DATA_PATH, string.Format("Area({0})_SubWorld({1}).MapFile",
                Utils.ConvertUniqueIDToFileName(AreaID), Utils.ConvertUniqueIDToFileName(subWorld.UniqueID)));
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);

            SubWorldDataFileFormat dataFile = new SubWorldDataFileFormat
            {
                Blocks = subWorld.Blocks,
                AreaID = AreaID,
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
                    subWorld.Blocks[packetData.BlockIndex_X, packetData.BlockIndex_Y, packetData.BlockIndex_Z].Type = packetData.BlockTypeValue;
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
