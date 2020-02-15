using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HMWTest.TestCode
{
    class TestMain
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
                if (line == "quit")
                {
                    break;
                }

                CPacket msg = CPacket.Create((short)NetProtocol.CHANGED_SUBWORLD_BLOCK_REQ);
                SubWorldBlockPacketData sendData;
                sendData.AreaID = "unique_0:0:0";
                sendData.SubWorldID = "unique_0:0:0";
                sendData.BlockIndex_X = TestUtils.RandomInteger(0, 32);
                sendData.BlockIndex_Y = TestUtils.RandomInteger(0, 32);
                sendData.BlockIndex_Z = TestUtils.RandomInteger(0, 32);
                sendData.BlockTypeValue = (byte)TestUtils.RandomInteger(2, 6);
                sendData.OwnerChunkType = (byte)TestUtils.RandomInteger(0, 3);
                //
                msg.Push(sendData.AreaID);
                msg.Push(sendData.SubWorldID);
                msg.Push(sendData.BlockIndex_X);
                msg.Push(sendData.BlockIndex_Y);
                msg.Push(sendData.BlockIndex_Z);
                msg.Push(sendData.BlockTypeValue);
                msg.Push(sendData.OwnerChunkType);
                GameServers[0].Send(msg);
            }

            ((DummyClient)GameServers[0]).UserTokenInstance.Disconnect();

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
                IPeer server = new DummyClient(serverToken);
                serverToken.OnConnected();
                GameServers.Add(server);
                Console.WriteLine("Connected! To HelloMyWorld Game Server.");
                // 서버에 세션을 접속 완료후에, 초기화 요청 패킷을 보낸다.
                CPacket initPacket = CPacket.Create((short)NetProtocol.AFTER_SESSION_INIT_REQ);
                initPacket.Push((byte)GameUserNetType.Client); // 유저의 NetType을 보낸다.
                if (GameServers[0] != null) GameServers[0].Send(initPacket);
            }
        }
    }
}
