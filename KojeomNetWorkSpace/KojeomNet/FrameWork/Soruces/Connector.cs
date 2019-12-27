using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class Connector
    {
        public delegate void ConnectedHandler(UserToken token);
        public ConnectedHandler OnConnectedHandler { get; set; }

        // 원격지 서버와의 연결을 위한 소켓.
        private Socket ClientSocket;

        private NetworkServiceManager NetworkServiceManagerInstance;

        public Connector(NetworkServiceManager networkService)
        {
            NetworkServiceManagerInstance = networkService;
            OnConnectedHandler = null;
        }

        public void Connect(IPEndPoint remoteEndpoint)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.NoDelay = true;

            // 비동기 접속을 위한 event args.
            SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += OnConnectCompleted;
            eventArgs.RemoteEndPoint = remoteEndpoint;
            bool pending = this.ClientSocket.ConnectAsync(eventArgs);
            if (pending == false)
            {
                OnConnectCompleted(null, eventArgs);
            }
        }

        void OnConnectCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Console.WriteLine("Connect completd!");
                UserToken token = new UserToken(NetworkServiceManagerInstance.LogicEntry);

                // 데이터 수신 준비.
                this.NetworkServiceManagerInstance.OnConnectCompleted(ClientSocket, token);

                this.OnConnectedHandler?.Invoke(token);
            }
            else
            {
                // failed.
                Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
            }
        }
    }
}
