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
        public int NetIdentityNumber { get; set; } = 0;
        private GameUserNetType NetType = GameUserNetType.None;
        private List<CPacket> SubWorldDataPackets = new List<CPacket>();
        private ulong TotalSubWorldDataPackets = 0;
        private ulong CurrentSafeReceived = 0;

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
                        bool bSuccess = GameWorldMapManager.GetInstance().AddSubWorldData(receivedData);
                        if(bSuccess == true)
                        {
                            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("Requested User ID : {0}, AreaID: {1}, SubWorldID : {2}, BlockIndex x : {3} y : {4} z : {5}, BlockType : {6}",
                                                                          NetIdentityNumber, receivedData.AreaID, receivedData.SubWorldID,
                                                                          receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z,
                                                                          receivedData.BlockTypeValue));
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
                        else
                        {
                            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("SubWorldData Change Failed. Requested User ID : {0} ", NetIdentityNumber));
                        }
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
                case NetProtocol.SUBWORLD_DATAS_SAFE_RECEIVED:
                    {
                        CurrentSafeReceived++;
                        // 모든 서브월드 데이터가 클라이언트한테 제대로 수신되었다면 완전 종료 패킷을 보낸다.
                        if(TotalSubWorldDataPackets == CurrentSafeReceived)
                        {
                            SendSubWorldDataFinishPacket();
                        }
                    }
                    break;
                case NetProtocol.SUBWORLD_DATAS_REQ:
                    {
                        // Host가 아닌 Client로 접속한 경우에만 서브월드 데이터 리스트를 전송.
                        if (NetType == GameUserNetType.Client)
                        {
                            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("User : {0} requested subWorld datas. SUBWORLD_DATAS_ACK START", NetIdentityNumber));
                            foreach(var data in GameWorldMapManager.GetInstance().WorldAreaMap)
                            {
                                WorldArea worldAreaInst = data.Value;
                                foreach(var subworldData in worldAreaInst.SubWorlds)
                                {
                                    SubWorld subWorldInst = subworldData.Value;
                                    foreach(var block in subWorldInst.Blocks)
                                    {
                                        // 초기 생성된 블록 타입값과 현재 타입이 같다면 보내지 않는다.
                                        if (block.CurrentType == block.OriginalType) continue;

                                        SubWorldBlockPacketData blockPacketData;
                                        blockPacketData.AreaID = worldAreaInst.UniqueID;
                                        blockPacketData.SubWorldID = subWorldInst.UniqueID;
                                        blockPacketData.BlockIndex_X = block.WorldDataIndexX;
                                        blockPacketData.BlockIndex_Y = block.WorldDataIndexY;
                                        blockPacketData.BlockIndex_Z = block.WorldDataIndexZ;
                                        blockPacketData.BlockTypeValue = block.CurrentType;
                                        blockPacketData.OwnerChunkType = (byte)block.OwnerChunkType;
                                        //
                                        CPacket blockPacket = CPacket.Create((short)NetProtocol.SUBWORLD_DATAS_ACK);
                                        blockPacket.Push(blockPacketData.AreaID);
                                        blockPacket.Push(blockPacketData.SubWorldID);
                                        blockPacket.Push(blockPacketData.BlockIndex_X);
                                        blockPacket.Push(blockPacketData.BlockIndex_Y);
                                        blockPacket.Push(blockPacketData.BlockIndex_Z);
                                        blockPacket.Push(blockPacketData.BlockTypeValue);
                                        blockPacket.Push(blockPacketData.OwnerChunkType);
                                        //
                                        SubWorldDataPackets.Add(blockPacket);
                                    }
                                }
                            }
                            foreach(var packet in SubWorldDataPackets)
                            {
                                Send(packet);
                            }
                            TotalSubWorldDataPackets = (ulong)SubWorldDataPackets.Count;
                            SubWorldDataPackets.Clear();
                            //
                            if (TotalSubWorldDataPackets == 0) SendSubWorldDataFinishPacket();
                            //
                            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("User : {0} requested subWorld datas. SUBWORLD_DATAS_ACK FINISH ( total : {1} )", NetIdentityNumber, TotalSubWorldDataPackets));
                        }
                    }
                    break;
            }
        }

        private void SendSubWorldDataFinishPacket()
        {
            GameLogger.SimpleConsoleWriteLineNoFileInfo(string.Format("Client Received All SubWorldData. ( User ID : {0} )", NetIdentityNumber));
            TotalSubWorldDataPackets = 0;
            CurrentSafeReceived = 0;
            CPacket finishSubWorldDatasPacket = CPacket.Create((short)NetProtocol.SUBWORLD_DATAS_FINISH);
            Send(finishSubWorldDatasPacket);
        }

        private async void AsyncSendWorldMap()
        {
            await AsyncSendSubworldDatas();
        }

        private async Task<bool> AsyncSendSubworldDatas()
        {
            return await Task.Run(() =>
            {
                foreach(var bytes in GameWorldMapManager.GetInstance().GetSubWorldBytesList())
                {
                    int bufferSize = bytes.Length + Defines.HEADER_SIZE;
                    CPacket subWorldPacket = CPacket.Create((short)NetProtocol.SUBWORLD_DATAS_ACK, bufferSize);
                    // 데이터 파일의 사이즈.
                    subWorldPacket.Push(bytes.Length);
                    // 실제 파일 bytes.
                    foreach (byte oneByte in bytes) subWorldPacket.Push(oneByte);
                    // sending.
                    Send(subWorldPacket);
                }
                return true;
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
