using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KojeomNet.FrameWork.Soruces;
using System.Net;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 네트워크 프로토콜.
/// REQ : 요청
/// ACK : 응답
/// PUSH : 서버 -> 클라이언트
/// </summary>
public enum NetProtocol
{
    BEGIN = 0,

    CHANGED_SUBWORLD_BLOCK_REQ,
    CHANGED_SUBWORLD_BLOCK_ACK,

    AFTER_SESSION_INIT_REQ,
    AFTER_SESSION_INIT_ACK,

    WORLD_MAP_PROPERTIES_REQ,
    WORLD_MAP_PROPERTIES_ACK,

    CHANGE_SUBWORLD_BLOCK_PUSH,

    SUBWORLD_DATAS_REQ,
    SUBWORLD_DATAS_ACK,

    END
}

public enum GameUserNetType
{
    None,
    Client,
    Host,
}

/// <summary>
/// 서버에서 수신한 SubWorld File 포멧.
/// </summary>
class SubWorldDataFileFormat
{
    public string AreaID;
    public string SubWorldID;
    public byte[,,] BlockTypes;
}

struct SubWorldPacketData
{
    public int Size;
    public byte[] SubWorldDataFileBytes;
}

struct WorldMapPropertiesPacketData
{
    public Int32 WorldAreaRow;
    public Int32 WorldAreaColumn;
    public Int32 WorldAreaLayer;
    public Int32 SubWorldRow;
    public Int32 SubWorldColumn;
    public Int32 SubWorldLayer;
    public Int32 SubWorldSizeX;
    public Int32 SubWorldSizeY;
    public Int32 SubWorldSizeZ;
}
public struct SubWorldBlockPacketData
{
    // 패킷 데이터.
    public string AreaID;
    public string SubWorldID;
    public Int32 BlockIndex_X;
    public Int32 BlockIndex_Y;
    public Int32 BlockIndex_Z;
    public byte BlockTypeValue;
    public byte OwnerChunkType;
    // 서버에서 기록하는 타임스탬프.
    //public long TimeStampTicks;
}

public class GameNetworkManager
{
    //
    public static long INVALID_TIMESTAMP_TICKS = 0;
    //
    private object LockControlObject;
    private IPeer GameServer = null;
    private NetworkServiceManager ServiceManager = new NetworkServiceManager();
    private Connector ConnectorInstance = null;
    //
    private static GameNetworkManager Instance = null;
    //
    public GameUserNetType UserNetType = GameUserNetType.None;

    public static GameNetworkManager GetInstance()
    {
        if(Instance == null)
        {
            Instance = new GameNetworkManager();
        }
        return Instance;
    }

    private GameNetworkManager()
    {
        // CNetworkService객체는 메시지의 비동기 송,수신 처리를 수행한다.
        // 메시지 송,수신은 서버, 클라이언트 모두 동일한 로직으로 처리될 수 있으므로
        // CNetworkService객체를 생성하여 Connector객체에 넘겨준다.
        // endpoint정보를 갖고있는 Connector생성. 만들어둔 NetworkService객체를 넣어준다.
        ConnectorInstance = new Connector(ServiceManager);
        // 접속 성공시 호출될 콜백 매소드 지정.
        ConnectorInstance.OnConnectedHandler += OnConnectedGameServer;
        //
        LockControlObject = new object();
    }

    ~GameNetworkManager()
    {
    }

    public void ConnectToGameServer(string ip, int port, GameUserNetType netType)
    {
        KojeomLogger.DebugLog(string.Format("Connect to server ip : {0}, port : {1}, NetType : {2}", ip, port, netType), LOG_TYPE.NETWORK_CLIENT_INFO);
        UserNetType = netType;
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
        ConnectorInstance.Connect(endpoint);
    }

    /// <summary>
    /// 접속 성공시 호출될 콜백 매소드.
    /// </summary>
    /// <param name="serverToken"></param>
    public void OnConnectedGameServer(UserToken serverToken)
    {
        lock (LockControlObject)
        {
            IPeer server = new RemoteServerPeer(serverToken);
            serverToken.OnConnected();
            GameServer = server;
            KojeomLogger.DebugLog("Success Connect to Server", LOG_TYPE.NETWORK_CLIENT_INFO);
            // 서버에 세션을 접속 완료후에, 초기화 요청 패킷을 보낸다.
            CPacket initPacket = CPacket.Create((short)NetProtocol.AFTER_SESSION_INIT_REQ);
            initPacket.Push((byte)UserNetType); // 유저의 NetType을 보낸다.
            if (GameServer != null) GameServer.Send(initPacket);
        }
    }

    /// <summary>
    /// 서브월드에서 변경된 블록의 정보를 보냅니다.
    /// </summary>
    /// <param name="packetData"></param>
    public void SendChangedSubWorldBlock(SubWorldBlockPacketData packetData)
    {
        CPacket packet = CPacket.Create((short)NetProtocol.CHANGED_SUBWORLD_BLOCK_REQ);
        // 1) areaID
        // 2) subWorldID
        // 3) BlockIndex_X, Y, Z
        // 4) block byte value
        packet.Push(packetData.AreaID);
        packet.Push(packetData.SubWorldID);
        packet.Push(packetData.BlockIndex_X);
        packet.Push(packetData.BlockIndex_Y);
        packet.Push(packetData.BlockIndex_Z);
        packet.Push(packetData.BlockTypeValue);
        packet.Push(packetData.OwnerChunkType);
        //
        if (GameServer != null)
        {
            KojeomLogger.DebugLog("Send to Server (Changed Block Data) ", LOG_TYPE.NETWORK_CLIENT_INFO);
            GameServer.Send(packet);
        }
    }

    public void SendWorldMapProperties()
    {
        WorldConfig config = WorldConfigFile.Instance.GetConfig();
        WorldMapData data = WorldMapDataFile.Instance.MapData;

        WorldMapPropertiesPacketData packetData;
        packetData.WorldAreaRow = data.WorldAreaRow;
        packetData.WorldAreaColumn = data.WorldAreaColumn;
        packetData.WorldAreaLayer = data.WorldAreaLayer;
        packetData.SubWorldRow = data.SubWorldRow;
        packetData.SubWorldColumn = data.SubWorldColumn;
        packetData.SubWorldLayer = data.SubWorldLayer;
        packetData.SubWorldSizeX = config.SubWorldSizeX;
        packetData.SubWorldSizeY = config.SubWorldSizeY;
        packetData.SubWorldSizeZ = config.SubWorldSizeZ;

        CPacket packet = CPacket.Create((short)NetProtocol.WORLD_MAP_PROPERTIES_REQ);
        packet.Push(packetData.WorldAreaRow);
        packet.Push(packetData.WorldAreaColumn);
        packet.Push(packetData.WorldAreaLayer);
        packet.Push(packetData.SubWorldRow);
        packet.Push(packetData.SubWorldColumn);
        packet.Push(packetData.SubWorldLayer);
        packet.Push(packetData.SubWorldSizeX);
        packet.Push(packetData.SubWorldSizeY);
        packet.Push(packetData.SubWorldSizeZ);
        //
        if (GameServer != null)
        {
            KojeomLogger.DebugLog("Send to Server (World map properties) ", LOG_TYPE.NETWORK_CLIENT_INFO);
            GameServer.Send(packet);
        }
    }

    public void DisConnectToGameServer()
    {
        KojeomLogger.DebugLog("DisConnectToGameServer ", LOG_TYPE.NETWORK_CLIENT_INFO);
        ((RemoteServerPeer)GameServer).UserTokenInstance.Ban();
    }
}

class RemoteServerPeer : IPeer
{
    public UserToken UserTokenInstance { get; private set; }

    public RemoteServerPeer(UserToken token)
    {
        this.UserTokenInstance = token;
        this.UserTokenInstance.SetPeer(this);
    }

    int RecvCount = 0;
    void IPeer.OnMessage(CPacket msg)
    {
        System.Threading.Interlocked.Increment(ref this.RecvCount);

        NetProtocol protocolID = (NetProtocol)msg.PopProtocolID();
        switch (protocolID)
        {
            case NetProtocol.CHANGED_SUBWORLD_BLOCK_ACK:
                {
                    KojeomLogger.DebugLog("Server received changed sub world data.", LOG_TYPE.NETWORK_CLIENT_INFO);
                }
                break;
            case NetProtocol.AFTER_SESSION_INIT_ACK:
                {
                    KojeomLogger.DebugLog("Receive after session information from server.", LOG_TYPE.NETWORK_CLIENT_INFO);
                    int seedValue = msg.PopInt32();
                    KojeomUtility.ChangeSeed(seedValue);
                    // 서버에서 Seed값을 받았고 유저 타입또한 송신했으므로, 
                    // 클라이언트라면, 서버에서 맵 정보를 받아야한다.
                    CPacket reqWorldMap = CPacket.Create((short)NetProtocol.SUBWORLD_DATAS_REQ);
                    UserTokenInstance.Send(reqWorldMap);
                }
                break;
            case NetProtocol.WORLD_MAP_PROPERTIES_ACK:
                {
                    KojeomLogger.DebugLog("Server received host map properties.", LOG_TYPE.NETWORK_CLIENT_INFO);
                }
                break;
            case NetProtocol.CHANGE_SUBWORLD_BLOCK_PUSH:
                {
                    KojeomLogger.DebugLog("CHANGE_SUBWORLD_BLOCK_PUSH packet received", LOG_TYPE.NETWORK_CLIENT_INFO);
                    SubWorldBlockPacketData receivedData;
                    receivedData.AreaID = msg.PopString();
                    receivedData.SubWorldID = msg.PopString();
                    receivedData.BlockIndex_X = msg.PopInt32();
                    receivedData.BlockIndex_Y = msg.PopInt32();
                    receivedData.BlockIndex_Z = msg.PopInt32();
                    receivedData.BlockTypeValue = msg.Popbyte();
                    receivedData.OwnerChunkType = msg.Popbyte();
                    WorldAreaManager.Instance.WorldAreas.TryGetValue(receivedData.AreaID, out WorldArea worldArea);
                    if (worldArea != null)
                    {
                        worldArea.SubWorldStates.TryGetValue(receivedData.SubWorldID, out SubWorldState subWorldState);
                        if (subWorldState != null)
                        {
                            subWorldState.SubWorldInstance.WorldBlockData[receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z].Type = receivedData.BlockTypeValue;
                            Vector3 chunkIndex = WorldAreaManager.ConvertBlockIdxToChunkIdx(receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z);
                            int chunkIdxX = (int)chunkIndex.x;
                            int chunkIdxY = (int)chunkIndex.y;
                            int chunkIdxZ = (int)chunkIndex.z;
                            int ownerChunkType = (int)receivedData.OwnerChunkType;
                            subWorldState.SubWorldInstance.ChunkSlots[chunkIdxX, chunkIdxY, chunkIdxZ].Chunks[ownerChunkType].Update = true;
                            KojeomLogger.DebugLog("CHANGE_SUBWORLD_BLOCK_PUSH -> Success Block Update.", LOG_TYPE.INFO);
                        }
                        else
                        {
                            KojeomLogger.DebugLog("CHANGE_SUBWORLD_BLOCK_PUSH -> Error( SubWorldState is null )", LOG_TYPE.ERROR);
                        }
                    }
                    else
                    {
                        KojeomLogger.DebugLog("CHANGE_SUBWORLD_BLOCK_PUSH -> Error( WorldArea is null )", LOG_TYPE.ERROR);
                    }
                }
                break;
            case NetProtocol.SUBWORLD_DATAS_ACK:
                {
                    KojeomLogger.DebugLog("SubWorld Datas received from server", LOG_TYPE.NETWORK_CLIENT_INFO);
                    SubWorldPacketData packetData;
                    packetData.Size = msg.PopInt32();
                    byte[] fileBytes = new byte[packetData.Size];
                    for(int idx = 0; idx < packetData.Size; idx++)
                    {
                        fileBytes[idx] = msg.Popbyte();
                    }
                    // bytes to stream.
                    Stream stream = new MemoryStream(fileBytes);
                    BinaryFormatter bf = new BinaryFormatter();
                    SubWorldDataFileFormat fileFormat = bf.Deserialize(stream) as SubWorldDataFileFormat;
                    WorldArea worldArea = WorldAreaManager.Instance.GetWorldArea(fileFormat.AreaID);
                    if(worldArea != null)
                    {
                        worldArea.SubWorldStates.TryGetValue(fileFormat.SubWorldID, out SubWorldState subWorldState);
                        if(subWorldState != null)
                        {
                            subWorldState.SubWorldInstance.UpdateBlocks(fileFormat.BlockTypes, true);
                        }
                    }
                }
                break;
        }
    }

    void IPeer.OnRemoved()
    {
        //Console.WriteLine("Server removed.");
        //Console.WriteLine("recv count " + this.RecvCount);
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
