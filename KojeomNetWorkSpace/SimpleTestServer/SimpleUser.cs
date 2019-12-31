using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleTestServer
{
    class SimpleUser : IPeer
    {
        private UserToken UserTokenInstance;
        public SimpleUser(UserToken userToken)
        {
            UserTokenInstance = userToken;
            UserTokenInstance.SetPeer(this);
        }

        public void Disconnect()
        {
        }

        public void OnMessage(CPacket msg)
        {
            NetProtocol.PROTOCOL protocol = (NetProtocol.PROTOCOL)msg.PopProtocolID();
            //Console.WriteLine("------------------------------------------------------");
            //Console.WriteLine("protocol id " + protocol);
            switch (protocol)
            {
                case NetProtocol.PROTOCOL.CHAT_MSG_REQ:
                    {
                        string text = msg.PopString();
                        var intValue = msg.PopInt32();
                        Console.WriteLine(string.Format("text {0}, value : {1}", text, intValue));

                        CPacket response = CPacket.Create((short)NetProtocol.PROTOCOL.CHAT_MSG_ACK);
                        response.Push(text);
                        Send(response);

                        if (text.Equals("exit"))
                        {
                            // 대량의 메시지를 한꺼번에 보낸 후 종료하는 시나리오 테스트.
                            for (int i = 0; i < 1000; ++i)
                            {
                                CPacket dummy = CPacket.Create((short)NetProtocol.PROTOCOL.CHAT_MSG_ACK);
                                dummy.Push(i.ToString());
                                Send(dummy);
                            }

                            UserTokenInstance.Ban();
                        }
                    }
                    break;
            }
        }

        public void OnRemoved()
        {
            TestServerMain.OnSessionRemoved(this);
        }

        public void Send(CPacket msg)
        {
            msg.RecordSize();

            // 소켓 버퍼로 보내기 전에 복사해 놓음.
            byte[] clone = new byte[msg.Position];
            Array.Copy(msg.Buffer, clone, msg.Position);

            UserTokenInstance.Send(new ArraySegment<byte>(clone, 0, msg.Position));
        }
    }
}
