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

        public int NetIdentityNumber = 0;
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
                        receivedData.OwnerChunkType = msg.Popbyte();
                        // 해당 패킷에 대한 타임스탬프를 기록.
                        //receivedData.TimeStampTicks = DateTime.Now.Ticks;
                        //
                        GameWorldMapManager.GetInstance().AddSubWorldData(receivedData);
                        //
                        GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("Modified User ID : {0}, AreaID: {1}, SubWorldID : {2}, BlockIndex x : {3} y : {4} z : {5}, BlockType : {6}",
                            NetIdentityNumber, receivedData.AreaID, receivedData.SubWorldID,
                            receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z,
                            receivedData.BlockTypeValue));
                        //
                        GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("Changed block ACK packet to GameUser (ID : {0})", NetIdentityNumber));
                        CPacket response = CPacket.Create((short)NetProtocol.CHANGED_SUBWORLD_BLOCK_ACK);
                        Send(response);

                        CPacket changeBlock = CPacket.Create((short)NetProtocol.CHANGE_SUBWORLD_BLOCK_PUSH);
                        changeBlock.Push(receivedData.AreaID);
                        changeBlock.Push(receivedData.SubWorldID);
                        changeBlock.Push(receivedData.BlockIndex_X);
                        changeBlock.Push(receivedData.BlockIndex_Y);
                        changeBlock.Push(receivedData.BlockIndex_Z);
                        changeBlock.Push(receivedData.BlockTypeValue);
                        changeBlock.Push(receivedData.OwnerChunkType);
                        GameServerManager.GetInstance().BroadCasting(changeBlock, NetIdentityNumber);
                    }
                    break;
                case NetProtocol.AFTER_SESSION_INIT_REQ:
                    {
                        GameUserNetType clientNetType = (GameUserNetType)msg.Popbyte();
                        NetType = clientNetType;

                        CPacket initPacket = CPacket.Create((short)NetProtocol.AFTER_SESSION_INIT_ACK);
                        initPacket.Push(GameWorldMapManager.GetInstance().RandomSeedValue);
                        Send(initPacket);
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
                        GameWorldMapManager.GetInstance().AsyncMakeMap();
                        //
                        GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("MAP_PROPERTIES : WorldAreaRow : {0}, Column : {1}, Layer : {2}," +
                            " SubWorldRow : {3}, Column : {4}, Layer : {5}, SubWorldSizeX : {6}, SizeY : {7}, SizeZ : {8}", 
                            packetData.WorldAreaRow, packetData.WorldAreaColumn, packetData.WorldAreaLayer,
                             packetData.SubWorldRow, packetData.SubWorldColumn, packetData.SubWorldLayer,
                             packetData.SubWorldSizeX, packetData.SubWorldSizeY, packetData.SubWorldSizeZ));

                        CPacket response = CPacket.Create((short)NetProtocol.WORLD_MAP_PROPERTIES_ACK);
                        Send(response);
                    }
                    break;
                case NetProtocol.SUBWORLD_DATAS_REQ:
                    {
                        // Host가 아닌 Client로 접속한 경우에만 서브월드 데이터 리스트를 전송.
                        if (NetType == GameUserNetType.Client)
                        {
                            //GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("User : {0} requested subWorld datas. and then push data to user.", NetIdentityNumber));
                            //AsyncMakeSubWorldDataPackets();
                        }
                    }
                    break;
            }
        }

        private async void AsyncMakeSubWorldDataPackets()
        {
            var packets = await AsyncMakeSubworldDataPackets_Internal();
            foreach(CPacket packet in packets)
            {
                Send(packet);
            }
        }

        private async Task<List<CPacket>> AsyncMakeSubworldDataPackets_Internal()
        {
            return await Task.Run(() => {
                List<CPacket> packets = new List<CPacket>();
                var mapData = GameWorldMapManager.GetInstance().GetWorldMapData();
                foreach (var data in mapData)
                {
                    int addValue = 1024;
                    CPacket pushSubWorldData = CPacket.Create((short)NetProtocol.SUBWORLD_DATAS_ACK, data.SubWorldDataFileBytes.Length + addValue);
                    // size.
                    pushSubWorldData.Push(data.SubWorldDataFileBytes.Length);
                    // file bytes.
                    for (int idx = 0; idx < data.SubWorldDataFileBytes.Length; idx++)
                    {
                        pushSubWorldData.Push(data.SubWorldDataFileBytes[idx]);
                    }
                    packets.Add(pushSubWorldData);
                }
                return packets;
            });
        }

        public void OnRemoved()
        {
            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("Session is stop. NetType : {0} ,NetID : {1}", NetType, NetIdentityNumber));
            GameServerManager.GetInstance().OnSessionRemoved(this);
        }

        public void Send(CPacket msg)
        {
            Token.Send(msg);
        }
    }
}
