using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KojeomNet.FrameWork.Soruces
{
    public class UserToken
    {
        enum State
        {
            // 대기중.
            Idle,

            // 연결됨.
            Connected,

            // 종료가 예약됨.
            // sending_list에 대기중인 상태에서 disconnect를 호출한 경우,
            // 남아있는 패킷을 모두 보낸 뒤 끊도록 하기 위한 상태값.
            ReserveClosing,

            // 소켓이 완전히 종료됨.
            Closed,
        }
        // 종료 요청. S -> C
        const short SYS_CLOSE_REQ = 0;
        // 종료 응답. C -> S
        const short SYS_CLOSE_ACK = -1;
        // 하트비트 시작. S -> C
        public const short SYS_START_HEARTBEAT = -2;
        // 하트비트 갱신. C -> S
        public const short SYS_UPDATE_HEARTBEAT = -3;

        // close중복 처리 방지를 위한 플래그.
        // 0 = 연결된 상태.
        // 1 = 종료된 상태.
        int bIsClosed;
        State CurrentState;
        // heartbeat.
        public long LatestHeartbeatTime { get; private set; }
        HeartbeatSender HeartbeatSenderInstance;
        bool bAutoHeartbeat;

        // session객체. 어플리케이션 딴에서 구현하여 사용.
        IPeer PeerInstance;

        public Socket SocketInstance { get; set; }
        public SocketAsyncEventArgs ReceiveArgs { get; private set; }
        public SocketAsyncEventArgs SendArgs { get; private set; }
        private MessageResolver MessageResolverInstance;
        private object CSSendingListLock;

        // BufferList적용을 위해 queue에서 list로 변경.
        List<ArraySegment<byte>> SendingList;

        public delegate void SessionClosedDelegate(UserToken token);
        public SessionClosedDelegate OnSessionClosed;

        private IMessageDispatcher Dispatcher;

        public UserToken(IMessageDispatcher dispatcher)
        {
            MessageResolverInstance = new MessageResolver();
            CSSendingListLock = new object();
            //
            Dispatcher = dispatcher;
            PeerInstance = null;
            LatestHeartbeatTime = DateTime.Now.Ticks;
            CurrentState = State.Idle;
            //
            SendingList = new List<ArraySegment<byte>>();
        }

        public void SetAsyncEventArgs(SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            ReceiveArgs = receiveArgs;
            SendArgs = sendArgs;
        }

        public void OnReceive(byte[] buffer, int offset, int bytesTransferred)
        {
            MessageResolverInstance.OnReceive(buffer, offset, bytesTransferred, OnMessageCompleted);
        }


        /// <summary>
        /// 패킷을 전송한다.
        /// 큐가 비어 있을 경우에는 큐에 추가한 뒤 바로 SendAsync매소드를 호출하고,
        /// 데이터가 들어있을 경우에는 새로 추가만 한다.
        /// 
        /// 큐잉된 패킷의 전송 시점 :
        ///		현재 진행중인 SendAsync가 완료되었을 때 큐를 검사하여 나머지 패킷을 전송한다.
        /// </summary>
        /// <param name="msg"></param>
        public void Send(ArraySegment<byte> data)
        {
            lock (CSSendingListLock)
            {
                SendingList.Add(data);

                if (SendingList.Count > 1)
                {
                    // 큐에 무언가가 들어 있다면 아직 이전 전송이 완료되지 않은 상태이므로 큐에 추가만 하고 리턴한다.
                    // 현재 수행중인 SendAsync가 완료된 이후에 큐를 검사하여 데이터가 있으면 SendAsync를 호출하여 전송해줄 것이다.
                    return;
                }
            }

            StartSend();
        }

        public void Send(CPacket msg)
        {
            msg.RecordSize();
            Send(new ArraySegment<byte>(msg.Buffer, 0, msg.Position));
        }

        private void StartSend()
        {
            try
            {
                SendArgs.BufferList = SendingList;
                bool bPending = SocketInstance.SendAsync(SendArgs);
                if(bPending == false)
                {
                    ProcessSend(SendArgs);
                }
            }
            catch (Exception e)
            {
                if (this.SocketInstance == null)
                {
                    Close();
                    return;
                }

                Console.WriteLine("send error!! close socket. " + e.Message);
                throw new Exception(e.Message, e);
            }
        }

        public void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                // 연결이 끊겨서 이미 소켓이 종료된 경우일 것이다.
                //Console.WriteLine(string.Format("Failed to send. error {0}, transferred {1}", e.SocketError, e.BytesTransferred));
                return;
            }

            lock (CSSendingListLock)
            {
                // 리스트에 들어있는 데이터의 총 바이트 수.
                var size = SendingList.Sum(obj => obj.Count);

                // 전송이 완료되기 전에 추가 전송 요청을 했다면 sending_list에 무언가 더 들어있을 것이다.
                if (e.BytesTransferred != size)
                {
                    //todo:세그먼트 하나를 다 못보낸 경우에 대한 처리도 해줘야 함.
                    // 일단 close시킴.
                    if (e.BytesTransferred < SendingList[0].Count)
                    {
                        string error = string.Format("Need to send more! transferred {0},  packet size {1}", e.BytesTransferred, size);
                        Console.WriteLine(error);

                        Close();
                        return;
                    }

                    // 보낸 만큼 빼고 나머지 대기중인 데이터들을 한방에 보내버린다.
                    int sent_index = 0;
                    int sum = 0;
                    for (int i = 0; i < SendingList.Count; ++i)
                    {
                        sum += SendingList[i].Count;
                        if (sum <= e.BytesTransferred)
                        {
                            // 여기 까지는 전송 완료된 데이터 인덱스.
                            sent_index = i;
                            continue;
                        }

                        break;
                    }
                    // 전송 완료된것은 리스트에서 삭제한다.
                    SendingList.RemoveRange(0, sent_index + 1);

                    // 나머지 데이터들을 한방에 보낸다.
                    StartSend();
                    return;
                }

                // 다 보냈고 더이상 보낼것도 없다.
                SendingList.Clear();

                // 종료가 예약된 경우, 보낼건 다 보냈으니 진짜 종료 처리를 진행한다.
                if (CurrentState == State.ReserveClosing)
                {
                    SocketInstance.Shutdown(SocketShutdown.Send);
                }
            }
        }

        public void SetPeer(IPeer peer)
        {
            PeerInstance = peer;
        }

        public void OnConnected()
        {
            CurrentState = State.Connected;
            bIsClosed = 0;
            bAutoHeartbeat = true;
        }

        public void Close()
        {
            // 중복 수행을 막는다.
            if (Interlocked.CompareExchange(ref bIsClosed, 1, 0) == 1)
            {
                return;
            }

            if (CurrentState == State.Closed)
            {
                // already closed.
                return;
            }

            CurrentState = State.Closed;
            SocketInstance.Close();
            SocketInstance = null;

            SendArgs.UserToken = null;
            ReceiveArgs.UserToken = null;

            SendingList.Clear();
            MessageResolverInstance.ClearBuffer();

            if (PeerInstance != null)
            {
                CPacket msg = CPacket.Create((short)-1);
                if (Dispatcher != null)
                {
                    Dispatcher.OnMessage(this, new ArraySegment<byte>(msg.Buffer, 0, msg.Position));
                }
                else
                {
                    OnMessage(msg);
                }
            }
        }

        public void OnMessageCompleted(ArraySegment<byte> buffer)
        {
            if (PeerInstance == null)
            {
                return;
            }

            if (Dispatcher != null)
            {
                // 로직 스레드의 큐를 타고 호출되도록 함.
                Dispatcher.OnMessage(this, buffer);
            }
            else
            {
                // IO스레드에서 직접 호출.
                CPacket msg = new CPacket(buffer, this);
                OnMessage(msg);
            }
        }

        public void OnMessage(CPacket msg)
        {
            // active close를 위한 코딩.
            //   서버에서 종료하라고 연락이 왔는지 체크한다.
            //   만약 종료신호가 맞다면 disconnect를 호출하여 받은쪽에서 먼저 종료 요청을 보낸다.
            switch (msg.ProtocolID)
            {
                case SYS_CLOSE_REQ:
                    Disconnect();
                    return;

                case SYS_START_HEARTBEAT:
                    {
                        // 순서대로 파싱해야 하므로 프로토콜 아이디는 버린다.
                        msg.PopProtocolID();
                        // 전송 인터벌.
                        byte interval = msg.Popbyte();
                        HeartbeatSenderInstance = new HeartbeatSender(this, interval);

                        if (bAutoHeartbeat == true)
                        {
                            StartHearBeat();
                        }
                    }
                    return;

                case SYS_UPDATE_HEARTBEAT:
                    //Console.WriteLine("heartbeat : " + DateTime.Now);
                    LatestHeartbeatTime = DateTime.Now.Ticks;
                    return;
            }


            if (PeerInstance != null)
            {
                try
                {
                    switch (msg.ProtocolID)
                    {
                        case SYS_CLOSE_ACK:
                            PeerInstance.OnRemoved();
                            break;

                        default:
                            PeerInstance.OnMessage(msg);
                            break;
                    }
                }
                catch (Exception)
                {
                    Close();
                }
            }

            if (msg.ProtocolID == SYS_CLOSE_ACK)
            {
                OnSessionClosed?.Invoke(this);
            }
        }

        public void StartHearBeat()
        {
            if (HeartbeatSenderInstance != null)
            {
                HeartbeatSenderInstance.Play();
            }
        }

        /// <summary>
        /// 연결을 종료한다.
        /// 주로 클라이언트에서 종료할 때 호출한다.
        /// </summary>
        public void Disconnect()
        {
            // close the socket associated with the client
            try
            {
                if (SendingList.Count <= 0)
                {
                    SocketInstance.Shutdown(SocketShutdown.Send);
                    return;
                }

                CurrentState = State.ReserveClosing;
            }
            // throws if client process has already closed
            catch (Exception)
            {
                Close();
            }
        }

        /// <summary>
        /// 연결을 종료한다. 단, 종료코드를 전송한 뒤 상대방이 먼저 연결을 끊게 한다.
        /// 주로 서버에서 클라이언트의 연결을 끊을 때 사용한다.
        /// 
        /// TIME_WAIT상태를 서버에 남기지 않으려면 disconnect대신 이 매소드를 사용해서
        /// 클라이언트를 종료시켜야 한다.
        /// </summary>
        public void Ban()
        {
            try
            {
                Byebye();
            }
            catch (Exception)
            {
                Close();
            }
        }


        /// <summary>
        /// 종료코드를 전송하여 상대방이 먼저 끊도록 한다.
        /// </summary>
        private void Byebye()
        {
            CPacket bye = CPacket.Create(SYS_CLOSE_REQ);
            Send(bye);
        }
    }
}
