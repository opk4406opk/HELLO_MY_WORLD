using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    class GameUser : IPeer
    {
        public UserToken Token { get; private set; }

        public GameUser(UserToken userToken)
        {
            Token = userToken;
            Token.SetPeer(this);
        }
        public void Disconnect()
        {
            Token.Ban();
        }

        public void OnMessage(CPacket msg)
        {
            NetProtocol protocol = (NetProtocol)msg.PopProtocolID();
            switch (protocol)
            {
                case NetProtocol.CHAT_MSG_REQ:
                    {
                        string text = msg.PopString();

                        CPacket response = CPacket.Create((short)NetProtocol.CHAT_MSG_ACK);
                        response.Push(text);
                        Send(response);
                    }
                    break;
            }
        }

        public void OnRemoved()
        {
            GameServerManager.GetInstance().OnSessionRemoved(this);
        }

        public void Send(CPacket msg)
        {
            Token.Send(msg);
        }
    }
}
