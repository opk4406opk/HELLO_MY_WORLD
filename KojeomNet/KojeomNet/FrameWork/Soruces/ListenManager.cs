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
           if(eventArgs.SocketError == SocketError.Success)
            {
                Socket clientSocket = eventArgs.AcceptSocket;
                OnNewClientConnected?.Invoke(clientSocket, eventArgs.UserToken);
                //
                FlowControlEvent.Set();
            }
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
