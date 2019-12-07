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
        private MessageResolver MessageResolverInstance;
        private Queue<CPacket> SendingQueue;
        public UserToken()
        {
            MessageResolverInstance = new MessageResolver();
            SendingQueue = new Queue<CPacket>();
        }

        public void SetAsyncEventArgs(SocketAsyncEventArgs receiveArgs, SocketAsyncEventArgs sendArgs)
        {
            ReceiveArgs = receiveArgs;
            SendArgs = sendArgs;
        }

        public void OnReceive(byte[] buffer, int offset, int bytesTransferred)
        {
            MessageResolverInstance.OnReceive(buffer, offset, bytesTransferred, OnMessage);
        }

        public void Send(CPacket msg)
        {
            CPacket clone = new CPacket();
            msg.CopyTo(clone);
            lock(SendingQueue)
            {
                if(SendingQueue.Count <= 0)
                {
                    SendingQueue.Enqueue(msg);
                    StartSend();
                    return;
                }
                SendingQueue.Enqueue(msg);
            }
        }

        private void StartSend()
        {
            lock(SendingQueue)
            {
                CPacket msg = SendingQueue.Peek();
                msg.RecordSize();
                SendArgs.SetBuffer(SendArgs.Offset, msg.Position);

                Array.Copy(msg.Buffer, 0, SendArgs.Buffer, SendArgs.Offset, msg.Position);
                bool bPending = SocketInstance.SendAsync(SendArgs);
                if(bPending == false)
                {
                    ProcessSend(SendArgs);
                }
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.BytesTransferred <= 0 || e.SocketError != SocketError.Success)
            {
                // 연결이 끊겨서 이미 소켓이 종료된 경우일 것이다.
                //Console.WriteLine(string.Format("Failed to send. error {0}, transferred {1}", e.SocketError, e.BytesTransferred));
                return;
            }

            lock(SendingQueue)
            {
                SendingQueue.Dequeue();
                if(SendingQueue.Count > 0)
                {
                    StartSend();
                }
            }
        }

        private void Close()
        {

        }

        public void OnMessage(Utils.Const<byte[]> buffer)
        {

        }
    }
}
