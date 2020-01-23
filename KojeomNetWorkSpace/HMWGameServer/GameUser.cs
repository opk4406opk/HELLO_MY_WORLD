using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWGameServer
{
    public enum GameUserNetType
    {
        None,
        Client,
        Host,
    }
    class GameUser : IPeer
    {
        public UserToken Token { get; private set; }

        //
        private GameUserNetType NetType = GameUserNetType.None;

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
                        // 클라이언트 에서 보내준 패킷 정보를 세팅.
                        SubWorldBlockPacketData receivedData;
                        receivedData.AreaID = msg.PopString();
                        receivedData.SubWorldID = msg.PopString();
                        receivedData.BlockIndex_X = msg.PopInt32();
                        receivedData.BlockIndex_Y = msg.PopInt32();
                        receivedData.BlockIndex_Z = msg.PopInt32();
                        receivedData.BlockTypeValue = msg.Popbyte();
                        // 해당 패킷에 대한 타임스탬프를 기록.
                        receivedData.TimeStampTicks = DateTime.Now.Ticks;
                        //
                        GameWorldMapManager.GetInstance().AddSubWorldData(receivedData);
                        //
                        Console.WriteLine(string.Format("AreaID: {0}, SubWorldID : {1}, BlockIndex x : {2} y : {3} z : {4}, BlockType : {5}",
                            receivedData.AreaID, receivedData.SubWorldID,
                            receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z, receivedData.BlockTypeValue));
                        //
                        CPacket response = CPacket.Create((short)NetProtocol.CHANGED_SUBWORLD_BLOCK_ACK);
                        Send(response);

                        CPacket changeBlock = CPacket.Create((short)NetProtocol.CHANGE_SUBWORLD_BLOCK_PUSH);
                        changeBlock.Push(receivedData.AreaID);
                        changeBlock.Push(receivedData.SubWorldID);
                        changeBlock.Push(receivedData.BlockIndex_X);
                        changeBlock.Push(receivedData.BlockIndex_Y);
                        changeBlock.Push(receivedData.BlockIndex_Z);
                        GameServerManager.GetInstance().BroadCasting(changeBlock, this);
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
                case NetProtocol.USER_NET_TYPE_REQ:
                    {
                        short netType = msg.PopInt16();
                        NetType = (GameUserNetType)netType;
                        CPacket response = CPacket.Create((short)NetProtocol.USER_NET_TYPE_ACK);
                        Send(response);
                    }
                    break;
                case NetProtocol.WORLD_MAP_PROPERTIES_REQ:
                    {
                        WorldMapPropertiesPacketData packetData;
                        packetData.WorldAreaRow = msg.PopInt32();
                        packetData.WorldAreaColumn = msg.PopInt32();
                        packetData.WorldAreaLayer = msg.PopInt32();
                        packetData.SubWorldRow = msg.PopInt32();
                        packetData.SubWorldColumn = msg.PopInt32();
                        packetData.SubWorldLayer = msg.PopInt32();
                        packetData.SubWorldSizeX = msg.PopInt32();
                        packetData.SubWorldSizeY = msg.PopInt32();
                        packetData.SubWorldSizeZ = msg.PopInt32();
                        GameWorldMapManager.GetInstance().WorldMapProperties = packetData;
                        GameWorldMapManager.GetInstance().MakeWorldMap();
                        //
                        Console.WriteLine(string.Format("MAP_PROPERTIES : WorldAreaRow : {0}, Column : {1}, Layer : {2}," +
                            " SubWorldRow : {3}, Column : {4}, Layer : {5}, SubWorldSizeX : {6}, SizeY : {7}, SizeZ : {8}", 
                            packetData.WorldAreaRow, packetData.WorldAreaColumn, packetData.WorldAreaLayer,
                             packetData.SubWorldRow, packetData.SubWorldColumn, packetData.SubWorldLayer,
                             packetData.SubWorldSizeX, packetData.SubWorldSizeY, packetData.SubWorldSizeZ));

                        CPacket response = CPacket.Create((short)NetProtocol.WORLD_MAP_PROPERTIES_ACK);
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
