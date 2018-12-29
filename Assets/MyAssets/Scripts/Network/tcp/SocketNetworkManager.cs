using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public enum TCPServerType
{
    Main,
    Login
}

/// <summary>
/// TCP 소켓 네트워킹 매니저.
/// 참고 문서 : https://docs.microsoft.com/ko-kr/dotnet/framework/network-programming/asynchronous-client-socket-example
/// </summary>
public class SocketNetworkManager : MonoBehaviour {

    private static SocketNetworkManager _singleton = null;
    public static SocketNetworkManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("SocketNetworkManager 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }

    public void Init()
    {
        KojeomLogger.DebugLog("SocketNetworkManager Init.", LOG_TYPE.INFO);
        _singleton = this;
    }

    /// <summary>
    ///  client  socket.
    /// </summary>
    private Socket clientSocket;

    public void ConnectToServer(TCPServerType type)
    {
        var gameServerData = GameServerDataFile.singleton.GetGameServerData();
        IPEndPoint ipEndpoint = null;
        switch (type)
        {
            case TCPServerType.Login:
                ipEndpoint = new IPEndPoint(IPAddress.Parse(gameServerData.login_server_ip), gameServerData.login_server_port);
                clientSocket = new Socket(ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                var aSyncresult = clientSocket.BeginConnect(ipEndpoint, OnConnectToServer, clientSocket);
                break;
            case TCPServerType.Main:
                break;
        }
    }

    public void SendToServer(TCPServerType type)
    {

    }

    private void OnConnectToServer(IAsyncResult ar)
    {
        // call back.
        // Retrieve the socket from the state object.  
        Socket client = (Socket)ar.AsyncState;
        // Complete the connection.  
        client.EndConnect(ar);
    }
}
