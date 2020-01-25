using System;
using System.Collections.Generic;
using System.Linq;
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
        // 서버에서 기록하는 타임스탬프.
        public long TimeStampTicks;


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
        private static GameWorldMapManager Instance;
        public static GameWorldMapManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new GameWorldMapManager();
            }
            return Instance;
        }

        public Dictionary<string, WorldArea> WorldAreaMap = new Dictionary<string, WorldArea>();
        public WorldMapPropertiesPacketData WorldMapProperties { get; set; }

        private GameWorldMapManager()
        {
        }

        public bool MakeWorldMap()
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
                        List<SubWorld> subWorldList = MakeSubWorld();
                        foreach(var subWorld in subWorldList)
                        {
                            worldArea.SubWorlds.Add(subWorld.UniqueID, subWorld);
                        }
                        WorldAreaMap.Add(worldArea.UniqueID, worldArea);
                    }
                }
            }
            return true;
        }

        private List<SubWorld> MakeSubWorld()
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
                        subworldList.Add(subWorld);
                    }
                }
            }
            return subworldList;
        }

        public void AddSubWorldData(SubWorldBlockPacketData packetData)
        {
            WorldAreaMap.TryGetValue(packetData.AreaID, out WorldArea worldArea);
            worldArea.SubWorlds.TryGetValue(packetData.SubWorldID, out SubWorld subWorld);
            subWorld.Blocks[packetData.BlockIndex_X, packetData.BlockIndex_Y, packetData.BlockIndex_Z] = packetData.BlockTypeValue;
        }
    }
}
