using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    public enum GameNetIdentityType
    {
        None,
        Client,
        Host,
    }
    class GameUser : IPeer
    {
        public UserToken Token { get; private set; }

        //
        private GameNetIdentityType NetIdentity = GameNetIdentityType.None;

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
                        SubWorldBlockChangedData receivedData;
                        receivedData.AreaID = msg.PopString();
                        receivedData.SubWorldID = msg.PopString();
                        receivedData.BlockIndex_X = msg.PopInt32();
                        receivedData.BlockIndex_Y = msg.PopInt32();
                        receivedData.BlockIndex_Z = msg.PopInt32();
                        receivedData.ToChangedTileValue = msg.Popbyte();
                        GameWorldMapManager.GetInstance().ChangedWorldBlockHistory.Add(receivedData);
                        //
                        Console.WriteLine(string.Format("AreaID: {0}, SubWorldID : {1}, BlockIndex x : {2} y : {3} z : {4}, BlockType : {5}",
                            receivedData.AreaID, receivedData.SubWorldID,
                            receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z, receivedData.ToChangedTileValue));
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
                case NetProtocol.USER_IDENTITY_REQ:
                    {
                        short identityEnum = msg.PopInt16();
                        NetIdentity = (GameNetIdentityType)identityEnum;
                        CPacket response = CPacket.Create((short)NetProtocol.USER_IDENTITY_ACK);
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
