using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    [Serializable]
    struct WorldArea
    {
        public string UniqueID;
        public Dictionary<string, SubWorld> SubWorlds;
    }
    [Serializable]
    struct SubWorld
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

        public WorldMapPropertiesPacketData WorldMapProperties { get; set; }

        private GameWorldMapManager()
        {
        }

        public bool MakeWorldMap()
        {
            return true;
        }

        public void AddSubWorldData(SubWorldBlockPacketData packetData)
        {

        }
    }
}
