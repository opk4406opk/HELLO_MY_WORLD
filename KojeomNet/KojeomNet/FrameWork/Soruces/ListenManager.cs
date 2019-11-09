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

        public void StartListen()
        {
            IPEndPoint localEndpoint = new IPEndPoint(IPAddress.Any, ListenPort);
            ListenSocket = new Socket(localEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ListenSocket.Bind(localEndpoint);
            ListenSocket.Listen(BackLog);
        }
    }
}
