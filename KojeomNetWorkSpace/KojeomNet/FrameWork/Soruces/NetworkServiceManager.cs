using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class NetworkServiceManager
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

        public ServerUserManager ServerUserManagerInstance { get; private set; }
        public LogicMessageEntry LogicEntry { get; private set; }

        public NetworkServiceManager(bool bUseLogicEntry = false)
        {
            ListenManagerInstance = new ListenManager();
            ListenManagerInstance.OnNewClientConnected += OnNewClientConnected;
            //
            ServerUserManagerInstance = new ServerUserManager();
            //
            if (bUseLogicEntry == true)
            {
                LogicEntry = new LogicMessageEntry(this);
                LogicEntry.Start();
            }
        }

        public void Initialize()
        {
            //
            BufferManagerInstance = new BufferManager(MaxConnection * BufferSizeBytes * PoolCount, BufferSizeBytes);
            BufferManagerInstance.InitBuffer();
            ReceiveSocketEventArgsPool = new SocketAsyncEventArgsPool(MaxConnection);
            SendSocketEventArgsPool = new SocketAsyncEventArgsPool(MaxConnection);

            for (int idx = 0; idx < MaxConnection; idx++)
            {
                SocketAsyncEventArgs receiveArgs = new SocketAsyncEventArgs();
                receiveArgs.UserToken = null;
                receiveArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
                BufferManagerInstance.SetBuffer(receiveArgs);
                ReceiveSocketEventArgsPool.Push(receiveArgs);
                //
                SocketAsyncEventArgs sendArgs = new SocketAsyncEventArgs();
                sendArgs.UserToken = null;
                sendArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
                sendArgs.SetBuffer(null, 0, 0);
                SendSocketEventArgsPool.Push(sendArgs);
            }
        }

        public void StartListen(string ip, int port)
        {
            ListenManagerInstance.StartListen(ip, port);
            // heartbeat.
            byte checkInterval = 10;
            ServerUserManagerInstance.StartHeartbeatChecking(checkInterval, checkInterval);
        }

     
        private void OnSendCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {
            try
            {
                UserToken token = eventArgs.UserToken as UserToken;
                token.ProcessSend(eventArgs);
            }
            catch (Exception)
            {
            }
        }

        private void OnNewClientConnected(Socket clientSocket, object token)
        {
            SocketAsyncEventArgs receiveArgs = ReceiveSocketEventArgsPool.Pop();
            SocketAsyncEventArgs sendArgs = SendSocketEventArgsPool.Pop();
            // UserToken은 매번 새로 생성하여 깨끗한 인스턴스로 넣어준다.
            UserToken userToken = new UserToken(LogicEntry);
            userToken.OnSessionClosed += OnSessionClosed;
            userToken.SocketInstance = clientSocket;
            //
            receiveArgs.UserToken = userToken;
            sendArgs.UserToken = userToken;

            this.ServerUserManagerInstance.Add(userToken);

            //
            userToken.OnConnected();
            OnSessionCreated?.Invoke(userToken);
            //
            BeginReceive(clientSocket, receiveArgs, sendArgs);
            //
            CPacket msg = CPacket.Create((short)UserToken.SYS_START_HEARTBEAT);
            byte sendInterval = 5;
            msg.Push(sendInterval);
            userToken.Send(msg);
        }

        public void OnConnectCompleted(Socket socket, UserToken token)
        {
            token.OnSessionClosed += OnSessionClosed;

            // SocketAsyncEventArgsPool에서 빼오지 않고 그때 그때 할당해서 사용한다.
            // 풀은 서버에서 클라이언트와의 통신용으로만 쓰려고 만든것이기 때문이다.
            // 클라이언트 입장에서 서버와 통신을 할 때는 접속한 서버당 두개의 EventArgs만 있으면 되기 때문에 그냥 new해서 쓴다.
            // 서버간 연결에서도 마찬가지이다.
            // 풀링처리를 하려면 c->s로 가는 별도의 풀을 만들어서 써야 한다.
            SocketAsyncEventArgs receiveEventArgs = new SocketAsyncEventArgs();
            receiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnReceiveCompleted);
            receiveEventArgs.UserToken = token;
            receiveEventArgs.SetBuffer(new byte[1024], 0, 1024);

            SocketAsyncEventArgs sendEventArgs = new SocketAsyncEventArgs();
            sendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnSendCompleted);
            sendEventArgs.UserToken = token;
            sendEventArgs.SetBuffer(null, 0, 0);

            BeginReceive(socket, receiveEventArgs, sendEventArgs);
        }

        private void OnSessionClosed(UserToken token)
        {
            // 세션 종료된 token 삭제.
            ServerUserManagerInstance.Remove(token);
            // Free the SocketAsyncEventArg so they can be reused by another client
            // 버퍼는 반환할 필요가 없다. SocketAsyncEventArg가 버퍼를 물고 있기 때문에
            // 이것을 재사용 할 때 물고 있는 버퍼를 그대로 사용하면 되기 때문이다.
            if (ReceiveSocketEventArgsPool != null)
            {
                ReceiveSocketEventArgsPool.Push(token.ReceiveArgs);
            }

            if (SendSocketEventArgsPool != null)
            {
                SendSocketEventArgsPool.Push(token.SendArgs);
            }

            token.SetAsyncEventArgs(null, null);
        }
       
        private void BeginReceive(Socket socket, SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            UserToken userToken = receiveArgs.UserToken as UserToken;
            userToken.SocketInstance = socket;
            userToken.SetAsyncEventArgs(receiveArgs, sendArgs);

            bool bPending = socket.ReceiveAsync(receiveArgs);
            if (bPending == false)
            {
                ProcessReceive(receiveArgs);
            }
        }
        private void OnReceiveCompleted(object sender, SocketAsyncEventArgs eventArgs)
        {
            if (eventArgs.LastOperation == SocketAsyncOperation.Receive)
            {
                ProcessReceive(eventArgs);
                return;
            }
            //
            //error occurred..
            throw new ArgumentException("The last operation completed on the socket was not a receive.");
        }
        private void ProcessReceive(SocketAsyncEventArgs receiveArgs)
        {
            UserToken userToken = receiveArgs.UserToken as UserToken;
            if (receiveArgs.BytesTransferred > 0 && receiveArgs.SocketError == SocketError.Success && userToken != null)
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
                try
                {
                    userToken.Close();
                }
                catch (Exception)
                {
                    Console.WriteLine("Already closed this socket.");
                }
            }
        }
    }
}
