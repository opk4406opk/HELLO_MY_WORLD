using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
public class CustomP2PLobby : NetworkLobbyManager
{
    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        base.OnLobbyClientConnect(conn);
    }

    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        base.OnLobbyClientDisconnect(conn);
    }

    public override void OnLobbyStartClient(NetworkClient lobbyClient)
    {
        StringBuilder log = new StringBuilder();
        log.AppendFormat("connected to server_ip : {0}, server_port : {1}", lobbyClient.serverIp, lobbyClient.serverPort);
        KojeomLogger.DebugLog(log.ToString());
    }

    public override void OnLobbyStartServer()
    {
        KojeomLogger.DebugLog("LobbyStart Server");
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
    }
}
