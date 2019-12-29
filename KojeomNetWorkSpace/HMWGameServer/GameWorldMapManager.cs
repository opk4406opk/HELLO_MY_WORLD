using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    struct SubWorldBlockPacketData
    {
        public string AreaID;
        public string SubWorldID;
        public int BlockIndex_X;
        public int BlockIndex_Y;
        public int BlockIndex_Z;
        public byte ToChangedTileValue;
    }
    class GameWorldMapManager
    {
        public Dictionary<string, SubWorldBlockPacketData> ChangedWorldBlockHistory { get; private set; }
        private static GameWorldMapManager Instance;
        public static GameWorldMapManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new GameWorldMapManager();
            }
            return Instance;
        }
        private GameWorldMapManager()
        {
            ChangedWorldBlockHistory = new Dictionary<string, SubWorldBlockPacketData>();
        }
    }
}
