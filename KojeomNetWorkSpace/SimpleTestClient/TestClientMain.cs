using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using KojeomNet.FrameWork.Soruces;

namespace SimpleTestClient
{
    class TestClientMain
    {
        static List<IPeer> GameServers = new List<IPeer>();
        static void Main(string[] args)
        {
            // CNetworkService객체는 메시지의 비동기 송,수신 처리를 수행한다.
            // 메시지 송,수신은 서버, 클라이언트 모두 동일한 로직으로 처리될 수 있으므로
            // CNetworkService객체를 생성하여 Connector객체에 넘겨준다.
            NetworkServiceManager service = new NetworkServiceManager();

            // endpoint정보를 갖고있는 Connector생성. 만들어둔 NetworkService객체를 넣어준다.
            Connector connector = new Connector(service);
            // 접속 성공시 호출될 콜백 매소드 지정.
            connector.OnConnectedHandler += OnConnectedGameServer;
            IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8000);
            connector.Connect(endpoint);
            //System.Threading.Thread.Sleep(10);

            while (true)
            {
                Console.Write("> ");
                string line = Console.ReadLine();
                if (line == "q")
                {
                    break;
                }

                CPacket msg = CPacket.Create((short)NetProtocol.PROTOCOL.CHAT_MSG_REQ);
                msg.Push(line);
                msg.Push(123);
                GameServers[0].Send(msg);
            }

            ((RemoteServerPeer)GameServers[0]).UserTokenInstance.Disconnect();

            //System.Threading.Thread.Sleep(1000 * 20);
            Console.ReadKey();
        }

        /// <summary>
		/// 접속 성공시 호출될 콜백 매소드.
		/// </summary>
		/// <param name="serverToken"></param>
		static public void OnConnectedGameServer(UserToken serverToken)
        {
            lock (GameServers)
            {
                IPeer server = new RemoteServerPeer(serverToken);
                serverToken.OnConnected();
                GameServers.Add(server);
                Console.WriteLine("Connected!");
            }
        }
    }
}
