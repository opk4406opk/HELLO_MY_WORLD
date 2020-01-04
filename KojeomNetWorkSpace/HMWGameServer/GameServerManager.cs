using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace HMWGameServer
{
    class GameServerManager
    {
        private List<GameUser> GameUserList;
        private NetworkServiceManager NetworkServiceMgr;

        private static GameServerManager Instance = null;

        /// <summary>
        /// 호스트에서 최초 맵 생성시에 사용된 Random Seed 값.
        /// </summary>
        public int WorldMapRandomSeed { get; set; }

        public static GameServerManager GetInstance()
        {
            if(Instance == null)
            {
                Instance = new GameServerManager();
            }
            return Instance;
        }


        private GameServerManager()
        {
            GameUserList = new List<GameUser>();
            NetworkServiceMgr = new NetworkServiceManager();
            NetworkServiceMgr.Initialize();
            NetworkServiceMgr.OnSessionCreated += OnSessionCreated;
        }

        public void StartListen()
        {
            // listen
            NetworkServiceMgr.StartListen("127.0.0.1", 8000);
        }

        public void StartServer()
        {
            Console.WriteLine("Hello my World GameServer Started!");
            while (true)
            {
                //Console.Write(".");
                string input = Console.ReadLine();
                if (input.Equals("users"))
                {
                    Console.WriteLine("users count : " + NetworkServiceMgr.ServerUserManagerInstance.GetTotalCount());
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void OnSessionCreated(UserToken userToken)
        {
            GameUser user = new GameUser(userToken);
            lock (GameUserList)
            {
                Console.WriteLine(string.Format("New Session Created. {0}", userToken.SocketInstance.RemoteEndPoint.ToString()));
                GameUserList.Add(user);
                // 새로 접속한 유저에게는 게임 월드 변경 히스토리를 전부 패킷으로 전달.
                foreach(SubWorldBlockChangedData data in GameWorldMapManager.GetInstance().ChangedWorldBlockHistory)
                {
                    CPacket historyPacket = CPacket.Create((short)NetProtocol.CHANGED_WORLD_HISTORY_ACK);
                    historyPacket.Push(data.AreaID);
                    historyPacket.Push(data.SubWorldID);
                    historyPacket.Push(data.BlockIndex_X);
                    historyPacket.Push(data.BlockIndex_Y);
                    historyPacket.Push(data.BlockIndex_Z);
                    historyPacket.Push(data.ToChangedTileValue);
                    user.Send(historyPacket);
                }
            }
        }

        public void OnSessionRemoved(GameUser user)
        {
            lock (GameUserList)
            {
                GameUserList.Remove(user);
            }
        }
    }
}
