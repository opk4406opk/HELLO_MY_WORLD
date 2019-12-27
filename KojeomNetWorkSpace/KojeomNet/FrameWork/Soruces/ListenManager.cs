using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class ListenManager
    {
        private Socket ListenSocket;
        private readonly int BackLog = 100;
        //
        private SocketAsyncEventArgs AcceptEventArgsInstance;
        private AutoResetEvent FlowControlEvent;

        public delegate void DelegateNewClientConnected(Socket clientSocket, object userToken);
        public event DelegateNewClientConnected OnNewClientConnected;

        public void StartListen(string ip, int port)
        {
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            ListenSocket = new Socket(localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.Bind(localEndpoint);
            ListenSocket.Listen(BackLog);
            //
            AcceptEventArgsInstance = new SocketAsyncEventArgs();
            AcceptEventArgsInstance.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);

            Thread listenThread = new Thread(TaskListening);
            listenThread.Start();
          
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {
            if (eventArgs.SocketError == SocketError.Success)
            {
                Socket clientSocket = eventArgs.AcceptSocket;
                clientSocket.NoDelay = true;
                // 이 클래스에서는 accept까지의 역할만 수행하고 클라이언트의 접속 이후의 처리는
                // 외부로 넘기기 위해서 콜백 매소드를 호출해 주도록 합니다.
                // 이유는 소켓 처리부와 컨텐츠 구현부를 분리하기 위함입니다.
                // 컨텐츠 구현부분은 자주 바뀔 가능성이 있지만, 소켓 Accept부분은 상대적으로 변경이 적은 부분이기 때문에
                // 양쪽을 분리시켜주는것이 좋습니다.
                // 또한 클래스 설계 방침에 따라 Listen에 관련된 코드만 존재하도록 하기 위한 이유도 있습니다.
                OnNewClientConnected?.Invoke(clientSocket, eventArgs.UserToken);
                //
                FlowControlEvent.Set();
                return;
            }
            else
            {
                Console.WriteLine("Failed to accept client. " + eventArgs.SocketError);
            }
            FlowControlEvent.Set();
        }

        private void TaskListening()
        {
            FlowControlEvent = new AutoResetEvent(false);
            while (true)
            {
                AcceptEventArgsInstance.AcceptSocket = null;
                bool pending = ListenSocket.AcceptAsync(AcceptEventArgsInstance);
                if (pending == false)
                {
                    OnAcceptCompleted(null, AcceptEventArgsInstance);
                }
                FlowControlEvent.WaitOne();
            }
           
        }
    }
}
