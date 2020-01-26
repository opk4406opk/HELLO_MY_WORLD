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
        // 서버 인스턴스 생성
        // ... ( 외부 프로세스를 실행?)
        // 인스턴스로 호스트 접속.
        GameNetworkManager.GetInstance().ConnectToGameServer("127.0.0.1", 8000, GameUserNetType.Host);
    }

    public void OnClickUpdateServerList()
    {
        KojeomLogger.DebugLog("Update Server List", LOG_TYPE.INFO);
    }

    public void OnClickConnectToHostWithIP()
    {
        UIPopupSupervisor.OpenPopupUI(UI_POPUP_TYPE.PromptServerIP);
    }

    private void UpdateServerList()
    {
    }
}
