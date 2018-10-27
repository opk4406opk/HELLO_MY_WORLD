using UnityEngine;
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
        var netClient = P2PNetworkManager.GetNetworkManagerInstance().StartHost();
        P2PNetworkManager.GetNetworkManagerInstance().LateInit();
        //
        P2PNetworkManager.GetNetworkManagerInstance().isHost = true;
        P2PNetworkManager.InitGameRandomSeed(System.DateTime.Now.Second);
        // Host로 시작하는 경우에는 랜덤시드를 서버에서 받은걸로 간주. ( Server, Client 역활을 같이 수행하므로)
        GameNetworkStateFlags.isReceivedRandomSeedFormServer = true;
    }

    public void OnClickUpdateServerList()
    {
        KojeomLogger.DebugLog("Update Server List", LOG_TYPE.INFO);
    }

    //테스트용 서버 컨넥트용 메소드.
    public void OnClickConnectToHostWithIP()
    {
        KojeomLogger.DebugLog("Connect To Host With IP", LOG_TYPE.INFO);
        var netClient = P2PNetworkManager.GetNetworkManagerInstance().StartClient();
        P2PNetworkManager.GetNetworkManagerInstance().LateInit();
        //
        P2PNetworkManager.GetNetworkManagerInstance().isHost = false;
        netClient.Connect("127.0.0.1", 8080);
    }

    private void UpdateServerList()
    {
    }
}
