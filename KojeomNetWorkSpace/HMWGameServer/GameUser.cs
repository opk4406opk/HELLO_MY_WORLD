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
                case NetProtocol.CHANGED_SUBWORLD_BLOCK_REQ:
                    {
                        SubWorldBlockPacketData receivedData;
                        //receivedData.ToChangedTileValue = msg.Popbyte();
                        //receivedData.BlockIndex_Z = msg.PopInt32();
                        //receivedData.BlockIndex_Y = msg.PopInt32();
                        receivedData.BlockIndex_X = msg.PopInt32();
                        receivedData.SubWorldID = msg.PopString();
                        //receivedData.AreaID = msg.PopString();
                        //Console.WriteLine(string.Format("AreaID: {0}, SubWorldID : {1}", receivedData.AreaID, receivedData.SubWorldID));
                        CPacket response = CPacket.Create((short)NetProtocol.CHANGED_SUBWORLD_BLOCK_ACK);
                        Send(response);
                    }
                    break;
                case NetProtocol.INIT_RANDOM_SEED_REQ:
                    {
                        short seed = msg.PopInt16();
                        GameServerManager.GetInstance().WorldMapRandomSeed = seed;
                        CPacket response = CPacket.Create((short)NetProtocol.INIT_RANDOM_SEED_ACK);
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
