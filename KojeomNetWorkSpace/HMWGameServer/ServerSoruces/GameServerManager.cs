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
        /// 게임 유저에게 할당하는 NetID.
        /// <br></br>유저가 접속할 때마다 1씩 증가한 값으로 설정.
        /// </summary>
        public int AssignNetID = 0;
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
                user.NetIdentity = AssignNetID++;
                GameUserList.Add(user);
            }
        }

        public void BroadCasting(CPacket packet, int exceptUserID)
        {
            lock(GameUserList)
            {
                foreach (var user in GameUserList)
                {
                    if (user.NetIdentity != exceptUserID)
                    {
                        user.Send(packet);
                    }
                }
            }
        }

        public void BroadCasting(CPacket packet)
        {
            lock (GameUserList)
            {
                foreach (var user in GameUserList)
                {
                    user.Send(packet);
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
