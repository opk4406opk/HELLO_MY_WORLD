using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;


public enum TCPServerType
{
    Login,
    Game
}

/// <summary>
/// TCP 소켓 네트워킹 매니저.
/// 참고 문서 : https://docs.microsoft.com/ko-kr/dotnet/framework/network-programming/asynchronous-client-socket-example
/// </summary>
public class SocketNetworkManager
{
    private static SocketNetworkManager Instance;
    
    public static SocketNetworkManager GetInstance()
    {
        if(Instance == null)
        {
            Instance = new SocketNetworkManager();
        }
        return Instance;
    }

    /// <summary>
    ///  client  socket.
    /// </summary>
    private Socket ClientSocket;

    public void ConnectToServer(TCPServerType type)
    {
        var gameServerData = GameServerDataFile.singleton.GetGameServerData();
        IPEndPoint ipEndpoint = null;
        switch (type)
        {
            case TCPServerType.Login:
                //
                KojeomLogger.DebugLog(string.Format("{0} 로그인서버로 접속 시도합니다.", gameServerData.login_server_ip), LOG_TYPE.SYSTEM);
                UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.waitingConnect);
                //
                ipEndpoint = new IPEndPoint(IPAddress.Parse(gameServerData.login_server_ip), gameServerData.login_server_port);
                ClientSocket = new Socket(ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                var aSyncresult = ClientSocket.BeginConnect(ipEndpoint, OnConnectToServer, ClientSocket);
                break;
            case TCPServerType.Game:
                break;
        }
    }

    public void SendToServer(TCPServerType type)
    {
        switch (type)
        {
            case TCPServerType.Login:
                ClientSocket.Send(new byte[]{ byte.Parse("TEST_DUMMY_DATA")});
                break;
            case TCPServerType.Game:
                break;
        }
    }

    private void OnConnectToServer(IAsyncResult ar)
    {
        Socket client = (Socket)ar.AsyncState;
        // call back.
        // Retrieve the socket from the state object.  
        KojeomLogger.DebugLog(string.Format("OnConnectToServer"), LOG_TYPE.SOCKET_NETWORK_INFO);
        SendToServer(TCPServerType.Login);
        // Complete the connection.  
        client.EndConnect(ar);

        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.waitingConnect);
        GameSoundManager.GetInstnace().StopSound(GAME_SOUND_TYPE.BGM_mainMenu);
        GameSceneLoader.LoadGameSceneAsync(GameSceneLoader.SCENE_TYPE.SELECT_CHARACTERS);
    }
}
