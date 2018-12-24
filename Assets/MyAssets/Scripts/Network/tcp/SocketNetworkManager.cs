using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 참고 문서 : https://docs.microsoft.com/ko-kr/dotnet/framework/network-programming/asynchronous-client-socket-example

/// <summary>
/// TCP 소켓 네트워킹 매니저.
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
        
    }

    public void ConnectToServer()
    {
        
    }
}
