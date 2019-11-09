using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    class NetworkServiceManager
    {
        // 송수신.
        private SocketAsyncEventArgsPool ReceiveSocketEventArgsPool;
        private SocketAsyncEventArgsPool SendSocketEventArgsPool;

        private readonly int MaxConnection = 10;
        private readonly int BufferSizeBytes = 1024;
        private readonly int PoolCount = 2;

        private BufferManager BufferManagerInstance;
        private ListenManager ListenManagerInstance;

        public delegate void DelegateSessionCreated(UserToken userToken);
        public DelegateSessionCreated OnSessionCreated;

        public void Initialize()
        {
            ListenManagerInstance = new ListenManager();
            ListenManagerInstance.OnNewClientConnected += OnNewClientConnected;
            //
            BufferManagerInstance = new BufferManager(MaxConnection * BufferSizeBytes * PoolCount, BufferSizeBytes);
            ReceiveSocketEventArgsPool = new SocketAsyncEventArgsPool(MaxConnection);
            SendSocketEventArgsPool = new SocketAsyncEventArgsPool(MaxConnection);

            for(int idx = 0; idx < MaxConnection; idx++)
            {
                UserToken userToken = new UserToken();
                //
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.UserToken = userToken;
                receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                BufferManagerInstance.SetBuffer(receiveArgs);
                ReceiveSocketEventArgsPool.Push(receiveArgs);
                //
                SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
                sendArgs.UserToken = userToken;
                sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                BufferManagerInstance.SetBuffer(sendArgs);
                SendSocketEventArgsPool.Push(sendArgs);
            }
        }

        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {

        }
        private void OnSendCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {

        }

        private void OnNewClientConnected(Socket clientSocket, object userToken)
        {
            SocketAsyncEventArgs receiveArgs = ReceiveSocketEventArgsPool.Pop();
            SocketAsyncEventArgs sendArgs = SendSocketEventArgsPool.Pop();
            //
            OnSessionCreated?.Invoke(receiveArgs.UserToken as UserToken);
            //
            BeginReceive(clientSocket, receiveArgs, sendArgs);
        }

        private void BeginReceive(Socket socket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            UserToken userToken = receiveArgs.UserToken as UserToken;
            userToken.SocketInstance = socket;
            userToken.SetAsyncEventArgs(receiveArgs, sendArgs);

            //
            bool bPending = socket.ReceiveAsync(receiveArgs);
            if(bPending == false)
            {
                ProcessReceive(receiveArgs);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs receiveArgs)
        {
            UserToken userToken = receiveArgs.UserToken as UserToken;
            if (receiveArgs.BytesTransferred > 0 && receiveArgs.SocketError == SocketError.Success)
            {
                userToken.OnReceive(receiveArgs.Buffer, receiveArgs.Offset, receiveArgs.BytesTransferred);
                bool bPending = userToken.SocketInstance.ReceiveAsync(receiveArgs);
                if(bPending == false)
                {
                    ProcessReceive(receiveArgs);
                }
            }
            else
            {
                // error occurred.
            }
        }
    }
}
