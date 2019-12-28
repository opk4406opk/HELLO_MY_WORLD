using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KojeomNet.FrameWork.Soruces;
using System.Net;
using System;

public enum NetProtocol
{
    BEGIN = 0,

    CHAT_MSG_REQ = 1,
    CHAT_MSG_ACK = 2,

    END
}


public class GameNetworkManager : MonoBehaviour
{
    private object LockControlObject;
    private IPeer GameServer = null;
    private NetworkServiceManager ServiceManager = new NetworkServiceManager();
    private Connector ConnectorInstance = null;

    private static GameNetworkManager Instance = null;
    
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
            KojeomLogger.DebugLog("Success Connect to Server", LOG_TYPE.P2P_NETWORK_CLIENT_INFO);
        }
    }

    public void DisConnectToGameServer()
    {
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
            case NetProtocol.CHAT_MSG_ACK:
                {
                    string text = msg.PopString();
                    //Console.WriteLine(string.Format("text {0}", text));
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
