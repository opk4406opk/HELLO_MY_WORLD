using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

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
            NetworkServiceMgr = new NetworkServiceManager(true);
            NetworkServiceMgr.Initialize();
            NetworkServiceMgr.OnSessionCreated += OnSessionCreated;
        }

        public void StartListen()
        {
            // listen
            string localIP = GetLocalIP();
            NetworkServiceMgr.StartListen(localIP, 8000);
            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("Listen with : {0}", localIP));
        }

        public void StartServer()
        {
            GameLogger.SimpleConsoleWriteLineNoFileInfo("Hello my World GameServer Started!");
            while (true)
            {
                //Console.Write(".");
                string input = Console.ReadLine();
                if (input.Equals("users"))
                {
                    GameLogger.SimpleConsoleWriteLineNoFileInfo("users count : " + GameUserList.Count);
                    foreach(GameUser gameUser in GameUserList)
                    {
                        GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("ID : {0}, IP Adress {1}", gameUser.NetIdentityNumber, gameUser.Token.SocketInstance.RemoteEndPoint));
                    }
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void OnSessionCreated(UserToken userToken)
        {
            GameUser user = new GameUser(userToken);
            lock (GameUserList)
            {
                GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("New Session Created. {0}", userToken.SocketInstance.RemoteEndPoint.ToString()));
                user.NetIdentityNumber = AssignNetID++;
                GameUserList.Add(user);
            }
        }

        public void BroadCasting(CPacket packet, int exceptUserID)
        {
            lock(GameUserList)
            {
                foreach (var user in GameUserList)
                {
                    if (user.NetIdentityNumber != exceptUserID)
                    {
                        GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("BroadCasting to user(ID : {0}, IPAdreess: {1})", 
                            user.NetIdentityNumber,
                            user.Token.SocketInstance.RemoteEndPoint.ToString()));
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
                    bool bValid = user != null && user.Token != null;
                    if (bValid == true) user.Send(packet);
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
        public static string GetLocalIP()
        {
            string localIP = "Not available, please check your network settings!";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }
            return localIP;
        }
    }
}
