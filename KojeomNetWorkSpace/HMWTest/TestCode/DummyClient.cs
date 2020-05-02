using KojeomNet.FrameWork.Soruces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMWTest.TestCode
{
    public enum GameUserNetType
    {
        None,
        Client,
        Host,
    }

    struct SubWorldBlockPacketData
    {
        // 실제 패킷 데이터.
        public string AreaID;
        public string SubWorldID;
        public int BlockIndex_X;
        public int BlockIndex_Y;
        public int BlockIndex_Z;
        public byte BlockTypeValue;
        public byte OwnerChunkType;
        // 서버에서 기록하는 타임스탬프.
        //public long TimeStampTicks;


        public static bool operator ==(SubWorldBlockPacketData a, SubWorldBlockPacketData b)
        {
            bool bAreaID = a.AreaID == b.AreaID;
            bool bSubWorldID = a.SubWorldID == b.SubWorldID;
            bool bIndexX = a.BlockIndex_X == b.BlockIndex_X;
            bool bIndexY = a.BlockIndex_Y == b.BlockIndex_Y;
            bool bIndexZ = a.BlockIndex_Z == b.BlockIndex_Z;

            return bAreaID ^ bSubWorldID ^ bIndexX ^ bIndexY ^ bIndexZ;
        }
        public static bool operator !=(SubWorldBlockPacketData a, SubWorldBlockPacketData b)
        {
            bool bAreaID = a.AreaID == b.AreaID;
            bool bSubWorldID = a.SubWorldID == b.SubWorldID;
            bool bIndexX = a.BlockIndex_X == b.BlockIndex_X;
            bool bIndexY = a.BlockIndex_Y == b.BlockIndex_Y;
            bool bIndexZ = a.BlockIndex_Z == b.BlockIndex_Z;

            return !(bAreaID ^ bSubWorldID ^ bIndexX ^ bIndexY ^ bIndexZ);
        }

    }
    class DummyClient : IPeer
    {
        public UserToken UserTokenInstance { get; private set; }
        private int RecvCount = 0;

        public DummyClient(UserToken token)
        {
            this.UserTokenInstance = token;
            this.UserTokenInstance.SetPeer(this);
        }

        public void Disconnect()
        {
            UserTokenInstance.Disconnect();
        }

       
        public void OnMessage(CPacket msg)
        {
            System.Threading.Interlocked.Increment(ref this.RecvCount);

            NetProtocol protocolID = (NetProtocol)msg.PopProtocolID();
            switch (protocolID)
            {
                case NetProtocol.CHANGED_SUBWORLD_BLOCK_ACK:
                    Console.WriteLine("CHANGED_SUBWORLD_BLOCK_ACK packet received.");
                    break;
                case NetProtocol.AFTER_SESSION_INIT_ACK:
                    Console.WriteLine("AFTER_SESSION_INIT_ACK packet received.");
                    // test.
                    //WorldMapPropertiesPacketData packetData;
                    //packetData.WorldAreaRow = 1;
                    //packetData.WorldAreaColumn = 1;
                    //packetData.WorldAreaLayer = 1;
                    //packetData.SubWorldRow = 2;
                    //packetData.SubWorldColumn = 2;
                    //packetData.SubWorldLayer = 2;
                    //packetData.SubWorldSizeX = 32;
                    //packetData.SubWorldSizeY = 32;
                    //packetData.SubWorldSizeZ = 32;

                    //CPacket packet = CPacket.Create((short)NetProtocol.WORLD_MAP_PROPERTIES_REQ);
                    //packet.Push(packetData.WorldAreaRow);
                    //packet.Push(packetData.WorldAreaColumn);
                    //packet.Push(packetData.WorldAreaLayer);
                    //packet.Push(packetData.SubWorldRow);
                    //packet.Push(packetData.SubWorldColumn);
                    //packet.Push(packetData.SubWorldLayer);
                    //packet.Push(packetData.SubWorldSizeX);
                    //packet.Push(packetData.SubWorldSizeY);
                    //packet.Push(packetData.SubWorldSizeZ);
                    ////
                    //UserTokenInstance.Send(packet);
                    break;
                case NetProtocol.WORLD_MAP_PROPERTIES_ACK:
                    Console.WriteLine("WORLD_MAP_PROPERTIES_ACK packet received.");
                    break;
                case NetProtocol.CHANGE_SUBWORLD_BLOCK_PUSH:
                    Console.WriteLine("CHANGE_SUBWORLD_BLOCK_PUSH packet received.");
                    break;
                case NetProtocol.SUBWORLD_DATAS_ACK:
                    Console.WriteLine("SUBWORLD_DATAS_ACK packet received.");
                    break;
            }
        }

        public void OnRemoved()
        {
        }

        public void Send(CPacket msg)
        {
            msg.RecordSize();
            UserTokenInstance.Send(new ArraySegment<byte>(msg.Buffer, 0, msg.Position));
        }
    }
}
