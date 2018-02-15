using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Data;
using Mono.Data.Sqlite;

public class MultiPlayLobbyManager : MonoBehaviour {

    [SerializeField]
    private GameObject serverListElementPrefab;
    [SerializeField]
    private GameObject uiGridObj;
    //
    private GameNetworkManager gameNetManager;

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
        gameNetManager = GameObject.FindWithTag("NetworkManager").GetComponent<GameNetworkManager>();
        if(gameNetManager == null)
        {
            KojeomLogger.DebugLog("GameNetmanager is null", LOG_TYPE.ERROR);
            return;
        }
        // set singleton.
        _instance = this;
    }

    public void OnClickStartHost()
    {
        KojeomLogger.DebugLog("StartHost", LOG_TYPE.INFO);
        gameNetManager.StartHost();
    }

    public void OnClickUpdateServerList()
    {
        KojeomLogger.DebugLog("Update Server List", LOG_TYPE.INFO);
    }

    private void UpdateServerList()
    {
        
    }
}
