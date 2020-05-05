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
        private readonly byte USING_NET_ID = 0;
        private readonly byte FREE_NET_ID = 1;
        private static int CurrentMaxIDTable = 32167;
        /// <summary>
        /// 
        /// </summary>
        private List<byte> NetIDTable = new List<byte>();
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
            // Net 식별자 테이블 초기화.
            for (int idx = 0; idx < CurrentMaxIDTable; idx++) NetIDTable.Add(FREE_NET_ID);
        }

        public void StartListen()
        {
            // listen
            string localIP = GetLocalIP();
            NetworkServiceMgr.StartListen(localIP, 8000);
            GameLogger.ConsoleLogNoFileInfo(string.Format("Listen with : {0}", localIP));
        }

        public void StartServer()
        {
            GameLogger.ConsoleLogNoFileInfo("Hello my World GameServer Started!");
            GameWorldMapManager.GetInstance().AsyncMakeMap();
            while (true)
            {
                if (GameWorldMapManager.GetInstance().bInitMakeWorldMap == false) continue;
                //Console.Write(".");
                string input = Console.ReadLine();
                if (input.Equals("users"))
                {
                    GameLogger.ConsoleLogNoFileInfo("users count : " + GameUserList.Count);
                    foreach(GameUser gameUser in GameUserList)
                    {
                        GameLogger.ConsoleLogNoFileInfo(string.Format("ID : {0}, IP Adress {1}", gameUser.NetIdentityNumber, gameUser.Token.SocketInstance.RemoteEndPoint));
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
                user.NetIdentityNumber = AssignNetID();
                GameUserList.Add(user);
                GameLogger.ConsoleLogNoFileInfo(string.Format("New Session Created. ( IP : {0}, NetID : {1} )",
                    userToken.SocketInstance.RemoteEndPoint.ToString(), user.NetIdentityNumber));

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
                        GameLogger.ConsoleLogNoFileInfo(string.Format("BroadCasting to user(ID : {0}, IPAdreess: {1})", 
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
                GameLogger.ConsoleLogNoFileInfo(string.Format("Session( NetID : {0} ) is Removed.", user.NetIdentityNumber));
                RecallNetID(user.NetIdentityNumber);
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

        private int AssignNetID()
        {
            for(int idx = 0; idx < NetIDTable.Count; idx++)
            {
                if (NetIDTable[idx] == FREE_NET_ID)
                {
                    NetIDTable[idx] = USING_NET_ID;
                    return idx;
                }
            }
            // 현재 테이블에서 비어있는 식별자가 없다면 1개 증가시킨다.
            CurrentMaxIDTable++;
            NetIDTable.Add(USING_NET_ID);
            GameLogger.ConsoleLogNoFileInfo(string.Format("Maximum NetID Table is Increased.  [current max : {0}]",
                CurrentMaxIDTable));
            //
            return CurrentMaxIDTable - 1;
        }

        private void RecallNetID(int netIdentifier)
        {
            NetIDTable[netIdentifier] = FREE_NET_ID;
        }
    }
}
