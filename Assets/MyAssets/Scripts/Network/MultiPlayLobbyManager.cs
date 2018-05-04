﻿using UnityEngine;
using UnityEngine.Networking;

public class MultiPlayLobbyManager : MonoBehaviour {

    [SerializeField]
    private GameObject serverListElementPrefab;
    [SerializeField]
    private GameObject uiGridObj;

    private static MultiPlayLobbyManager _instance;
    public static MultiPlayLobbyManager instance
    {
        get
        {
            if(_instance == null) KojeomLogger.DebugLog("MultiPlayLobbyManager Singleton is NULL", LOG_TYPE.ERROR);
            return _instance;
        }
    }
    private void Start()
    {
        _instance = this;
    }

    public void OnClickStartHost()
    {
        KojeomLogger.DebugLog("StartHost", LOG_TYPE.INFO);
        var netClient = GameNetworkManager.GetInstance().StartHost();
        //
        GameNetworkManager.GetInstance().isHost = true;
        GameNetworkManager.GetInstance().Init();
    }

    public void OnClickUpdateServerList()
    {
        KojeomLogger.DebugLog("Update Server List", LOG_TYPE.INFO);
    }

    //테스트용 서버 컨넥트용 메소드.
    public void OnClickConnectToHostWithIP()
    {
        KojeomLogger.DebugLog("Connect To Host With IP", LOG_TYPE.INFO);
        var netClient = GameNetworkManager.GetInstance().StartClient();
        //
        GameNetworkManager.GetInstance().isHost = false;
        GameNetworkManager.GetInstance().Init();
        netClient.Connect("127.0.0.1", 8080);
    }

    private void UpdateServerList()
    {
    }
}