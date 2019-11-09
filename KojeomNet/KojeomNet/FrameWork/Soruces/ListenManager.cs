using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    class ListenManager
    {
        private Socket ListenSocket;
        private readonly int ListenPort = 8000;
        private readonly int BackLog = 100;
        //
        private SocketAsyncEventArgs SocketAsyncEventArgsInstance;

        public void StartListen()
        {
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, ListenPort);
            ListenSocket = new Socket(localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.Bind(localEndpoint);
            ListenSocket.Listen(BackLog);
            //
            SocketAsyncEventArgsInstance = new SocketAsyncEventArgs();
            SocketAsyncEventArgsInstance.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            bool pending = ListenSocket.AcceptAsync(SocketAsyncEventArgsInstance);
            if(pending == false)
            {
                ProcessAcceptCompleted(SocketAsyncEventArgsInstance);
            }
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {
            ProcessAcceptCompleted(eventArgs);
        }

        private void ProcessAcceptCompleted(SocketAsyncEventArgs eventArgs)
        {

        }
    }
}
