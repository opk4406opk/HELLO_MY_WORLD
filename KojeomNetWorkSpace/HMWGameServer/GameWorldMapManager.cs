using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
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
        private GameWorldMapManager()
        {
        }

        public void AddSubWorldData(SubWorldBlockPacketData packetData)
        {

        }
    }
}
