using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    class GameServerManager
    {
        private List<GameUser> GameUserList;
        private NetworkServiceManager NetworkServiceManagerInstance;

        private static GameServerManager Instance = null;

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
            NetworkServiceManagerInstance = new NetworkServiceManager();
            NetworkServiceManagerInstance.Initialize();
            NetworkServiceManagerInstance.OnSessionCreated += OnSessionCreated;
        }

        public void StartListen()
        {
            // listen
            NetworkServiceManagerInstance.StartListen("127.0.0.1", 8000);
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
                    Console.WriteLine("users count : " + NetworkServiceManagerInstance.ServerUserManagerInstance.GetTotalCount());
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        private void OnSessionCreated(UserToken userToken)
        {
            GameUser simpleUser = new GameUser(userToken);
            lock (GameUserList)
            {
                GameUserList.Add(simpleUser);
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
