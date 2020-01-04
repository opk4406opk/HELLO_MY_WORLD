using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KojeomNet.FrameWork.Soruces;
using System.Net;
using System;
using System.Runtime.InteropServices;

public enum NetProtocol
{
    BEGIN = 0,

    CHANGED_SUBWORLD_BLOCK_REQ,
    CHANGED_SUBWORLD_BLOCK_ACK,

    INIT_RANDOM_SEED_REQ, // only host
    INIT_RANDOM_SEED_ACK, // only host

    USER_NET_TYPE_REQ,
    USER_NET_TYPE_ACK,

    CHANGED_WORLD_HISTORY_REQ,
    CHANGED_WORLD_HISTORY_ACK,

    END
}

public enum GameUserNetType
{
    None,
    Client,
    Host,
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
    // 서버에서 기록하는 타임스탬프.
    public long TimeStampTicks;
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

    public void ConnectToGameServer(string ip, int port)
    {
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
            // 접속 성공후, 맵 생성에 사용되는 랜덤 시드값을 보낸다.
            CPacket seedPacket = new CPacket();
            seedPacket.SetProtocol((short)NetProtocol.INIT_RANDOM_SEED_REQ);
            seedPacket.Push(KojeomUtility.SeedValue);
            GameServer.Send(seedPacket);
            // 호스트/클라이언트 Type 패킷을 보낸다.
            CPacket typePacket = new CPacket();
            typePacket.SetProtocol((short)NetProtocol.USER_NET_TYPE_REQ);
            typePacket.Push((short)UserNetType);
            GameServer.Send(typePacket);
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
        //
        KojeomLogger.DebugLog("Send to Server (Changed Block Data) ", LOG_TYPE.NETWORK_CLIENT_INFO);
        GameServer.Send(packet);
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
            case NetProtocol.INIT_RANDOM_SEED_ACK:
                {
                    KojeomLogger.DebugLog("Server received init random seed for create world map.", LOG_TYPE.NETWORK_CLIENT_INFO);
                }
                break;
            case NetProtocol.USER_NET_TYPE_ACK:
                {
                    KojeomLogger.DebugLog("Server received user identity enum value.", LOG_TYPE.NETWORK_CLIENT_INFO);
                }
                break;
            case NetProtocol.CHANGED_WORLD_HISTORY_ACK:
                {
                    SubWorldBlockPacketData packetData;
                    packetData.AreaID = msg.PopString();
                    packetData.SubWorldID = msg.PopString();
                    packetData.BlockIndex_X = msg.PopInt32();
                    packetData.BlockIndex_Y = msg.PopInt32();
                    packetData.BlockIndex_Z = msg.PopInt32();
                    packetData.BlockTypeValue = msg.Popbyte();
                    packetData.TimeStampTicks = msg.PopInt64();
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
