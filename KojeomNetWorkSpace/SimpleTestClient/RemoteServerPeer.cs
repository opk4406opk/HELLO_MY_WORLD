using System;
using KojeomNet.FrameWork.Soruces;

namespace SimpleTestClient
{
    class RemoteServerPeer : IPeer
    {
        public UserToken UserTokenInstance { get; private set; }

        public RemoteServerPeer(UserToken token)
        {
            this.UserTokenInstance = token;
            this.UserTokenInstance.SetPeer(this);
        }

        private int RecvCount = 0;
        void IPeer.OnMessage(CPacket msg)
        {
            System.Threading.Interlocked.Increment(ref this.RecvCount);

            NetProtocol.PROTOCOL protocolID = (NetProtocol.PROTOCOL)msg.PopProtocolID();
            switch (protocolID)
            {
                case NetProtocol.PROTOCOL.CHAT_MSG_ACK:
                    {
                        string text = msg.PopString();
                        Console.WriteLine(string.Format("text {0}", text));
                    }
                    break;
            }
        }

        void IPeer.OnRemoved()
        {
            Console.WriteLine("Server removed.");
            Console.WriteLine("recv count " + this.RecvCount);
        }

        void IPeer.Send(CPacket msg)
        {
            msg.RecordSize();
            UserTokenInstance.Send(new ArraySegment<byte>(msg.Buffer, 0, msg.Position));
        }

        void IPeer.Disconnect()
        {
            UserTokenInstance.Disconnect();
        }
    }
}
