﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KojeomNet.FrameWork.Soruces;
using System.Net;
using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Threading.Tasks;
using MapGenLib;

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

    CHANGE_SUBWORLD_BLOCK_PUSH,

    SUBWORLD_DATAS_REQ, // 클라에서 서버로 월드맵(모든 서브월드) 요청.
    SUBWORLD_DATAS_ACK, // 서버에서 클라로 서브월드 데이터 전달.

    SUBWORLD_DATAS_SAFE_RECEIVED, // 클라이언트에서 제대로 수신했을경우 보낸다.
    SUBWORLD_DATAS_FINISH, // 서버에서 클라가 모든 서브월드 데이터 패킷을 수신했다면 종료 패킷으로 보낸다.

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
    public byte OwnerChunkType;
    // 서버에서 기록하는 타임스탬프.
    //public long TimeStampTicks;
}

public struct SubWorldPacketDataKey
{
    public string AreaID;
    public string SubWorldID;
}

public class GameNetworkManager
{
    #region CallBacks
    public delegate void OnChangeSubWorldBlock();
    public OnChangeSubWorldBlock OnChangeSubWorldBlockCallBack;
    #endregion

    #region Variables
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
    public bool bFinishReceivedAllSubWorlds = false; // 서버로부터 모든 서브월드 데이터를 수신했는지?
    public Dictionary<SubWorldPacketDataKey, List<SubWorldBlockPacketData>> InitialReceivedSubWorldDatas = new Dictionary<SubWorldPacketDataKey, List<SubWorldBlockPacketData>>();
    #endregion

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

    public static string GetLocalIP()
    {
        string localIP = "Not available, please check your network settings!";
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                localIP = ip.ToString();
                break;
            }
        }
        return localIP;
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
            // 접속 완료후에, 초기화 요청 패킷을 보낸다.
            CPacket initPacket = CPacket.Create((short)NetProtocol.AFTER_SESSION_INIT_REQ);
            initPacket.Push((byte)UserNetType); // 유저의 NetType을 보낸다.
            if (GameServer != null) GameServer.Send(initPacket);
        }
    }

    /// <summary>
    /// 서브월드에서 변경된 블록의 정보를 보냅니다.
    /// </summary>
    /// <param name="packetData"></param>
    public void RequestChangeSubWorldBlock(SubWorldBlockPacketData packetData, OnChangeSubWorldBlock callBack)
    {
        OnChangeSubWorldBlockCallBack = callBack;
        CPacket packet = CPacket.Create((short)NetProtocol.CHANGED_SUBWORLD_BLOCK_REQ);
        // 1) areaID
        // 2) subWorldID
        // 3) BlockIndex_X, Y, Z
        // 4) block byte value
        // 5) Owner Chunk Type
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
            KojeomLogger.DebugLog("Request to Server (Changed Block Data) ", LOG_TYPE.NETWORK_CLIENT_INFO);
            GameServer.Send(packet);
        }
    }

    public void PushSafeReceivedSubWorldData()
    {
        CPacket safeReceivePacket = CPacket.Create((short)NetProtocol.SUBWORLD_DATAS_SAFE_RECEIVED);
        if(GameServer != null)
        {
            KojeomLogger.DebugLog("Push to Server (Safe Received SubWorldData) ", LOG_TYPE.NETWORK_CLIENT_INFO);
            GameServer.Send(safeReceivePacket);
        }
    }

    public void DisconnectToGameServer()
    {
        KojeomLogger.DebugLog("DisconnectToGameServer ", LOG_TYPE.NETWORK_CLIENT_INFO);
        if(GameServer != null) ((RemoteServerPeer)GameServer).UserTokenInstance.Disconnect();
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
                    KojeomLogger.DebugLog("[ACK] Server received changed sub world data.", LOG_TYPE.NETWORK_CLIENT_INFO);
                    GameNetworkManager.GetInstance().OnChangeSubWorldBlockCallBack();
                }
                break;
            case NetProtocol.AFTER_SESSION_INIT_ACK:
                {
                    KojeomLogger.DebugLog("[ACK] Receive after session information from server.", LOG_TYPE.NETWORK_CLIENT_INFO);
                    int seedValue = msg.PopInt32();
                    KojeomUtility.ChangeSeed(seedValue);
                    // 서버에서 Seed값을 받았고 유저 타입또한 송신했으므로, 
                    // 클라이언트라면, 서버에서 맵 정보를 받아야한다.
                    if(GameNetworkManager.GetInstance().UserNetType == GameUserNetType.Client)
                    {
                        CPacket reqWorldMap = CPacket.Create((short)NetProtocol.SUBWORLD_DATAS_REQ);
                        UserTokenInstance.Send(reqWorldMap);
                    }
                }
                break;
            case NetProtocol.CHANGE_SUBWORLD_BLOCK_PUSH:
                {
                    KojeomLogger.DebugLog("[PUSH] CHANGE_SUBWORLD_BLOCK_PUSH packet received", LOG_TYPE.NETWORK_CLIENT_INFO);
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
                            float centerX = subWorldState.SubWorldInstance.WorldBlockData[receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z].CenterX;
                            float centerY = subWorldState.SubWorldInstance.WorldBlockData[receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z].CenterY;
                            float centerZ = subWorldState.SubWorldInstance.WorldBlockData[receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z].CenterZ;
                            Vector3 blockLocation = new Vector3(centerX, centerY, centerZ);
                            // 비어있는 블록이라면, 충돌 옥트리에서 해당 위치에 해당하는 노드 삭제.
                            if ((BlockTileType)receivedData.BlockTypeValue == BlockTileType.EMPTY) subWorldState.SubWorldInstance.CustomOctreeInstance.Delete(blockLocation);
                            else subWorldState.SubWorldInstance.CustomOctreeInstance.Add(blockLocation);
                            // 블록 업데이트.
                            subWorldState.SubWorldInstance.WorldBlockData[receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z].CurrentType = receivedData.BlockTypeValue;
                            Vector3 chunkIndex = WorldAreaManager.ConvertBlockIdxToChunkIdx(receivedData.BlockIndex_X, receivedData.BlockIndex_Y, receivedData.BlockIndex_Z);
                            int chunkIdxX = (int)chunkIndex.x;
                            int chunkIdxY = (int)chunkIndex.y;
                            int chunkIdxZ = (int)chunkIndex.z;
                            int ownerChunkType = (int)receivedData.OwnerChunkType;
                            subWorldState.SubWorldInstance.ChunkSlots[chunkIdxX, chunkIdxY, chunkIdxZ].Chunks[ownerChunkType].Update = true;
                            KojeomLogger.DebugLog("[PUSH] CHANGE_SUBWORLD_BLOCK_PUSH -> Success Block Update.", LOG_TYPE.INFO);
                        }
                        else
                        {
                            KojeomLogger.DebugLog("[PUSH] CHANGE_SUBWORLD_BLOCK_PUSH -> Error( SubWorldState is null )", LOG_TYPE.ERROR);
                        }
                    }
                    else
                    {
                        KojeomLogger.DebugLog("[PUSH] CHANGE_SUBWORLD_BLOCK_PUSH -> Error( WorldArea is null )", LOG_TYPE.ERROR);
                    }
                }
                break;
            case NetProtocol.SUBWORLD_DATAS_ACK:
                {
                    KojeomLogger.DebugLog("[ACK] SubWorld Block Data received from server", LOG_TYPE.NETWORK_CLIENT_INFO);
                    SubWorldBlockPacketData receivedData;
                    receivedData.AreaID = msg.PopString();
                    receivedData.SubWorldID = msg.PopString();
                    receivedData.BlockIndex_X = msg.PopInt32();
                    receivedData.BlockIndex_Y = msg.PopInt32();
                    receivedData.BlockIndex_Z = msg.PopInt32();
                    receivedData.BlockTypeValue = msg.Popbyte();
                    receivedData.OwnerChunkType = msg.Popbyte();

                    SubWorldPacketDataKey key;
                    key.AreaID = receivedData.AreaID;
                    key.SubWorldID = receivedData.SubWorldID;
                    List<SubWorldBlockPacketData> blockPackets;
                    bool bFind = GameNetworkManager.GetInstance().InitialReceivedSubWorldDatas.TryGetValue(key, out blockPackets);
                    if(bFind == false)
                    {
                        blockPackets = new List<SubWorldBlockPacketData>();
                        blockPackets.Add(receivedData);
                    }
                    else
                    {
                        blockPackets.Add(receivedData);
                    }
                    // 데이터 수신했음을 알린다.
                    GameNetworkManager.GetInstance().PushSafeReceivedSubWorldData();
                }
                break;
            case NetProtocol.SUBWORLD_DATAS_FINISH: // 모든 서브월드 데이터를 서버로부터 완전하게 수신했다.
                {
                    // 서버에서 월드맵 송신이 종료되었음을 알리는 패킷.
                    KojeomLogger.DebugLog("SUBWORLD_DATAS_FINISH [ All Subworld data received. ]", LOG_TYPE.NETWORK_CLIENT_INFO);
                    GameNetworkManager.GetInstance().bFinishReceivedAllSubWorlds = true;
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
