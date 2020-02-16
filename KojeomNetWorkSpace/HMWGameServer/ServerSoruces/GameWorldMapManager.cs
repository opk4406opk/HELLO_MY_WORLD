using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    [Serializable]
    class WorldArea
    {
        public string UniqueID;
        public Dictionary<string, SubWorld> SubWorlds;
    }
    [Serializable]
    class SubWorld
    {
        public string UniqueID;
        public byte[,,] Blocks;
        public string SavePath;
    }
    [Serializable]
    class SubWorldDataFileFormat
    {
        public string AreaID;
        public string SubWorldID;
        public byte[,,] BlockTypes;
    }
    struct SubWorldPacketData
    {
        public int Size;
        public byte[] SubWorldDataFileBytes;
    }
    struct WorldMapPropertiesPacketData
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
        public WorldMapPropertiesPacketData WorldMapProperties { get; set; }

        private GameWorldMapManager()
        {
            LockObject = new object();
        }

        public async void AsyncMakeMap()
        {
            bInitMakeWorldMap = await MakeWorldMap();
        }

        private async Task<bool> MakeWorldMap()
        {
            return await Task.Run(() =>
            {
                // init
                WorldAreaMap = new Dictionary<string, WorldArea>();
                // make.
                for (int areaX = 0; areaX < WorldMapProperties.WorldAreaRow; ++areaX)
                {
                    for (int areaY = 0; areaY < WorldMapProperties.WorldAreaLayer; ++areaY)
                    {
                        for (int areaZ = 0; areaZ < WorldMapProperties.WorldAreaColumn; ++areaZ)
                        {
                            WorldArea worldArea = new WorldArea();
                            worldArea.UniqueID = Utils.MakeUniqueID(areaX, areaY, areaZ);
                            worldArea.SubWorlds = new Dictionary<string, SubWorld>();
                            List<SubWorld> subWorldList = MakeSubWorld(worldArea.UniqueID);
                            foreach (var subWorld in subWorldList)
                            {
                                worldArea.SubWorlds.Add(subWorld.UniqueID, subWorld);
                            }
                            WorldAreaMap.Add(worldArea.UniqueID, worldArea);
                        }
                    }
                }
                return true;
            });
        }

        private List<SubWorld> MakeSubWorld(string areaID)
        {
            lock(LockObject)
            {
                List<SubWorld> subworldList = new List<SubWorld>();
                for (int x = 0; x < WorldMapProperties.SubWorldRow; ++x)
                {
                    for (int y = 0; y < WorldMapProperties.SubWorldLayer; ++y)
                    {
                        for (int z = 0; z < WorldMapProperties.SubWorldColumn; ++z)
                        {
                            SubWorld subWorld = new SubWorld();
                            subWorld.UniqueID = Utils.MakeUniqueID(x, y, z);
                            subWorld.Blocks = new byte[WorldMapProperties.SubWorldSizeX, WorldMapProperties.SubWorldSizeY, WorldMapProperties.SubWorldSizeZ];
                            subWorld.SavePath = SaveSubWorldFile(subWorld, areaID); // make data file.
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

        private byte[] LoadSubWorldFileBytes(string filePath)
        {
            FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            var bytes = Utils.ReadFully(fileStream);
            fileStream.Close();
            return bytes;
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
                BlockTypes = subWorld.Blocks,
                AreaID = AreaID,
                SubWorldID = subWorld.UniqueID
            };
            // 시리얼라이징.
            bf.Serialize(fileStream, dataFile);
            fileStream.Close();
            // return path.
            return savePath;
        }

        public void AddSubWorldData(SubWorldBlockPacketData packetData)
        {
            lock(LockObject)
            {
                WorldAreaMap.TryGetValue(packetData.AreaID, out WorldArea worldArea);
                if(worldArea != null)
                {
                    worldArea.SubWorlds.TryGetValue(packetData.SubWorldID, out SubWorld subWorld);
                    subWorld.Blocks[packetData.BlockIndex_X, packetData.BlockIndex_Y, packetData.BlockIndex_Z] = packetData.BlockTypeValue;
                    // save subworld.
                    SaveSubWorldFile(subWorld, packetData.AreaID);
                }
                else
                {
                    Console.WriteLine("[ERROR] AddSubWorldData - WorldArea is Null");
                }
               
            }
           
        }

        public List<SubWorldPacketData> GetWorldMapData()
        {
            lock(LockObject)
            {
                List<SubWorldPacketData> packetDatas = new List<SubWorldPacketData>();
                foreach (var area in WorldAreaMap)
                {
                    WorldArea worldArea = area.Value;
                    foreach (var sub in worldArea.SubWorlds)
                    {
                        SubWorld subWorld = sub.Value;
                        SubWorldPacketData packetData;
                        packetData.SubWorldDataFileBytes = LoadSubWorldFileBytes(subWorld.SavePath);
                        packetData.Size = packetData.SubWorldDataFileBytes.Length;
                        packetDatas.Add(packetData);
                    }
                }
                return packetDatas;
            }
        }
    }
}
