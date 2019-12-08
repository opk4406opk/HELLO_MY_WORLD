using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    class Connector
    {
        public delegate void ConnectedHandler(UserToken token);
        public ConnectedHandler OnConnectedHandler { get; set; }

        // 원격지 서버와의 연결을 위한 소켓.
        Socket ClientSocket;

        NetworkServiceManager NetworkServiceManagerInstance;

        public Connector(NetworkServiceManager networkService)
        {
            NetworkServiceManagerInstance = networkService;
            OnConnectedHandler = null;
        }

        public void connect(IPEndPoint remote_endpoint)
        {
            ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ClientSocket.NoDelay = true;

            // 비동기 접속을 위한 event args.
            SocketAsyncEventArgs eventArgs = new SocketAsyncEventArgs();
            eventArgs.Completed += on_connect_completed;
            eventArgs.RemoteEndPoint = remote_endpoint;
            bool pending = this.ClientSocket.ConnectAsync(eventArgs);
            if (!pending)
            {
                on_connect_completed(null, eventArgs);
            }
        }

        void on_connect_completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                //Console.WriteLine("Connect completd!");
                UserToken token = new UserToken(this.NetworkServiceManagerInstance.logic_entry);

                // 데이터 수신 준비.
                this.NetworkServiceManagerInstance.on_connect_completed(this.ClientSocket, token);

                if (this.OnConnectedHandler != null)
                {
                    this.OnConnectedHandler(token);
                }
            }
            else
            {
                // failed.
                Console.WriteLine(string.Format("Failed to connect. {0}", e.SocketError));
            }
        }
    }
}
