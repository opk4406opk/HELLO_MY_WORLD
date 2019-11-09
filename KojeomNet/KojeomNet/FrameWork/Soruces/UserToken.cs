using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    class UserToken
    {
        public Socket SocketInstance { get; set; }
        private SocketAsyncEventArgs ReceiveArgs;
        private SocketAsyncEventArgs SendArgs;

        public void SetAsyncEventArgs(SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            ReceiveArgs = receiveArgs;
            SendArgs = sendArgs;
        }

        public void OnReceive(byte[] buffer, int offset, int bytesTransferred)
        {

        }
    }
}
