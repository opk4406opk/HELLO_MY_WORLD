using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KojeomNet;
using KojeomNet.FrameWork.Soruces;

namespace SimpleTestServer
{
    
    class TestServerMain
    {
        private static List<SimpleUser> UserList = new List<SimpleUser>();
        private static NetworkServiceManager NetworkServiceManagerInstance;
        static void Main(string[] args)
        {
            // Display the number of command line arguments.
            Console.WriteLine(args.Length);

            Init();
            // listen
            NetworkServiceManagerInstance.StartListen("127.0.0.1", 8000);

            Console.WriteLine("Started!");
            while (true)
            {
                //Console.Write(".");
                string input = Console.ReadLine();
                if (input.Equals("users"))
                {
                }
                System.Threading.Thread.Sleep(1000);
            }
        }

        public static void Init()
        {
            NetworkServiceManagerInstance = new NetworkServiceManager();
            NetworkServiceManagerInstance.Initialize();
            NetworkServiceManagerInstance.OnSessionCreated += OnSessionCreated;
        }

        private static void OnSessionCreated(UserToken userToken)
        {
            SimpleUser simpleUser = new SimpleUser(userToken);
            lock(UserList)
            {
                UserList.Add(simpleUser);
            }
        }

        public static void OnSessionRemoved(SimpleUser user)
        {
            lock (UserList)
            {
                UserList.Remove(user);
            }
        }
    }
}
