using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiPlayLobbyManager : MonoBehaviour {

    [SerializeField]
    private GameObject serverListElementPrefab;
    [SerializeField]
    private GameObject uiGridObj;
    [SerializeField]
    private CustomP2PLobby p2pLobbyManager;

    private void Start()
    {
        
    }

    public void OnClickStartHost()
    {
        KojeomLogger.DebugLog("StartHost", LOG_TYPE.INFO);
    }

    public void OnClickUpdateServerList()
    {
        KojeomLogger.DebugLog("Update Server List", LOG_TYPE.INFO);
    }

    private void UpdateServerList()
    {

    }
}
