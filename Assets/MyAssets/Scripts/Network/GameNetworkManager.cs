using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

// ref : https://msdn.microsoft.com/ko-kr/library/system.net.httpwebrequest.begingetrequeststream(v=vs.110).aspx
public struct HTTP_REQUEST_METHOD
{
    public static string POST = "POST";
}

/// <summary>
///  서버, 클라이언트간의 메세지를 주고받는 프로토콜 열거형.
///  통신 프로토콜의 접두사는 아래와 같다.
///
///  1. push : 클라이언트에서 서버로 특정 데이터를 전달.
///  2. req : 클라이언트에서 서버로 특정 데이터를 요청.
///  3. res : 서버에서 클라이언트로 특정 데이터를 전달.
/// </summary>
public enum GAME_NETWORK_PROTOCOL
{
    push_clientInfo = 1000,
    res_inGameUserList = 1001,
    req_inGameUserList = 1002
}

public class GameNetPlayerData : MessageBase
{
    public string playerName;
    public int connectionID;
    public string address;
    public int selectChType;
}

// 서버로 특정 데이터를 요청시에 보내는 클라이언트 정보 메세지.
public class ReqNetClientInfo : MessageBase
{
    public int connectionID;
}

/// <summary>
/// 네트워크 통신에 사용되는 게임유저 데이터 구조체.
/// </summary>
public struct GameNetUserData
{
    public int selectCharType;
    public int connectionID;
    public string userName;
}
public class ResGameUserList : MessageBase
{
    public GameNetUserData[] userList;
}

public class GameNetUser
{
    public GamePlayer gamePlayer;
    public int selectCharType;
    public int connectionID;
    public string userName;
    public GameNetUser(string userName, int connnectionID, int selectCharType, GamePlayer gamePlayer = null)
    {
        this.connectionID = connnectionID;
        this.userName = userName;
        this.selectCharType = selectCharType;
        this.gamePlayer = gamePlayer;
    }
}

/*
 - 메서드(method) 접두사 정리 -

    1. Req : 클라이언트에서 서버로 요청하는 경우. (request)
    2. OnRecvFromClient : 서버에서 클라이언트의 요청을 수신하는 경우. (receive)
    3. OnRevFromServer : 클라이언트에서 서버의 응답을 수신하는 경우. (receive) 
    4. Res : 서버에서 클라이언트로 데이터를 전송하는 경우. (response)
*/

/// <summary>
/// 현재 unity3d 엔진에서 stable .NET 3.5 버전에 맞춘 테스트 네트워크매니저 class.
/// </summary>
public class GameNetworkManager : NetworkManager {
    // TEST_loginServer_func
    // -> 구글 vm에 올려놓은 centOS http서버에 로그인 패킷을 날려보는 테스트 메소드.
    #region TEST_loginServer_func
    public delegate void del_HttpRequest(bool isSuccessLogin);
    public static event del_HttpRequest PostHttpRequest;
    private static void GetRequestStreamCallBack(IAsyncResult asynchronousResult)
	{
        Dictionary<string, string> loginData = new Dictionary<string, string>();
		loginData.Add("user_name", "TEST_CLIENT0");
		JSONObject jsonData = new JSONObject(loginData);
		byte[] byteData = Encoding.ASCII.GetBytes(jsonData.ToString());
		// set Request.
		HttpWebRequest req = (HttpWebRequest)asynchronousResult.AsyncState;
		// End the operation
		Stream postStream = req.EndGetRequestStream(asynchronousResult);
		postStream.Write(byteData, 0, byteData.Length);
		postStream.Close();
		// Start the asynchronous operation to get the response
		req.ContentType = "application/json";
		req.BeginGetResponse(new AsyncCallback(GetResponseCallBack), req);
	}
	private static void GetResponseCallBack(IAsyncResult asynchronousResult)
	{
		HttpWebRequest req = (HttpWebRequest)asynchronousResult.AsyncState;
		// End the operation
		HttpWebResponse response = (HttpWebResponse)req.EndGetResponse(asynchronousResult);
		Stream streamResponse = response.GetResponseStream();
		StreamReader streamRead = new StreamReader(streamResponse);
		string responseData = streamRead.ReadToEnd();
		
		// Close the stream object
		streamResponse.Close();
		streamRead.Close();

		// Release the HttpWebResponse
		response.Close();

		if (responseData.Equals("true")) PostHttpRequest(true);
		else PostHttpRequest(false);
	}
	public static void ConnectLoginServer()
	{
        var gameServerData = GameServerDataFile.singleton.GetGameServerData();
        string addr = string.Format("http://{0}:{1}",
            gameServerData.login_server_ip,
            gameServerData.login_server_port);
        //
        KojeomLogger.DebugLog(string.Format("{0} 로그인서버로 접속 시도합니다.", addr), LOG_TYPE.SYSTEM);
        UIPopupSupervisor.OpenPopupUI(POPUP_TYPE.waitingConnect);
        //
        HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(addr));
		webReq.Method = HTTP_REQUEST_METHOD.POST;
		webReq.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallBack), webReq);
	}


    #endregion
    //

    public bool isHost = false;

    [SerializeField]
    private GameNetworkSpawner networkSpanwer;
    public GameNetworkSpawner GetNetworkSpawner()
    {
        return networkSpanwer;
    }
    
    private List<GameNetUser> _netUserList = new List<GameNetUser>();
    public List<GameNetUser> netUserList
    {
        get { return _netUserList; }
    }

    private static GameNetworkManager instance;
    public static GameNetworkManager GetInstance()
    {
        if (instance == null) instance = GameObject.FindWithTag("NetworkManager").GetComponent<GameNetworkManager>();
        return instance;
    }
    
    public void Init()
    {
        KojeomLogger.DebugLog("게임네트워크 매니저 초기화 시작.");
        ServerSettings();
        ClientSettings();
        KojeomLogger.DebugLog("게임네트워크 매니저 초기화 완료.");
    }

    private void ServerSettings()
    {
        //server setting
        NetworkServer.RegisterHandler((short)GAME_NETWORK_PROTOCOL.push_clientInfo,
            OnRecvFromClient_ConnectInfo);
        NetworkServer.RegisterHandler((short)GAME_NETWORK_PROTOCOL.req_inGameUserList,
            OnRecvFromClient_ReqGameUserList);
    }
    private void ClientSettings()
    {
        //client setting
        client.RegisterHandler((short)GAME_NETWORK_PROTOCOL.res_inGameUserList,
            OnRecvFromServer_GameUserList);
    }
    
    public override NetworkClient StartHost(ConnectionConfig config, int maxConnections)
    {
        return base.StartHost(config, maxConnections);
    }

    public override NetworkClient StartHost(MatchInfo info)
    {
        return base.StartHost(info);
    }

    public override NetworkClient StartHost()
    {
        return base.StartHost();
    }
    // host(or server)에서 client가 접속할 때 발생되는 콜백 메소드.
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        KojeomLogger.DebugLog(string.Format("[Connection_Info-->{0}] 클라이언트가 서버에 접속했습니다.", conn),
            LOG_TYPE.NETWORK_SERVER_INFO);
    }

    // client입장에서 서버에 접속시에 불려지는 콜백 메소드.
    // 이 메소드 코드 흐름을 보기좋게 정리할 필요가 있다.
    public override void OnClientConnect(NetworkConnection conn)
    {
        // 게임유저 데이터 메세지 생성.
        GameNetPlayerData msgData = new GameNetPlayerData();
        msgData.connectionID = conn.connectionId;
        msgData.address = conn.address;
        msgData.playerName = "PLAYER";
        int charType = GameDBHelper.GetSelectCharType();
        msgData.selectChType = charType;

        KojeomLogger.DebugLog("서버로 접속을 했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        KojeomLogger.DebugLog(string.Format("ClientScene localPlayers count : {0}",
            ClientScene.localPlayers.Count),
            LOG_TYPE.NETWORK_CLIENT_INFO);
        // 게임 유저에 대한 데이터를 서버로 전송한다.
        bool isSendSuccess = conn.Send((short)GAME_NETWORK_PROTOCOL.push_clientInfo, msgData);
        if (isSendSuccess) KojeomLogger.DebugLog("Send client info to server success ", LOG_TYPE.NETWORK_CLIENT_INFO);
        else KojeomLogger.DebugLog("Send client info to server failed ", LOG_TYPE.ERROR);
        // 전송 후, 게임 유저에 대한 정보를 유저목록에 등록한다.
        if (isHost == false)
        {
            GameNetUser netUser = new GameNetUser("PLAYER", conn.connectionId, charType);
            KojeomLogger.DebugLog(string.Format("유저리스트에 클라이언트 유저를 등록합니다. user_connID : {0}, name : {1}, chType : {2}",
                netUser.connectionID, netUser.userName, netUser.selectCharType),
                LOG_TYPE.NETWORK_CLIENT_INFO);
            _netUserList.Add(netUser);
        }
        // NetworkManager 프리팹에서 autoCreatePlayer 옵션을 true로 하는 경우,
        // 해당 로컬 컨넥션에 대해 자동으로 ClientScene.AddPlayer(0)을 호출한다.
        //base.OnClientConnect(conn);
        //ClientScene.Ready(conn);
        bool isSuccesAddPlayer = ClientScene.AddPlayer(conn, 0, msgData);
        if (isSuccesAddPlayer == true) KojeomLogger.DebugLog("ClientScene.AddPlayer is Success.", LOG_TYPE.NETWORK_CLIENT_INFO);
        else KojeomLogger.DebugLog("ClientScene.AddPlayer is Failed.", LOG_TYPE.NETWORK_CLIENT_INFO);
        //
    }
    private void OnRecvFromClient_ConnectInfo(NetworkMessage netMsg)
    {
        var netPlayerData = netMsg.ReadMessage<GameNetPlayerData>();
        KojeomLogger.DebugLog(string.Format("conneted clinet info [ connection_id : {0}, addr : {1}, selectChType : {2} ]",
            netPlayerData.connectionID, netPlayerData.address, netPlayerData.selectChType), LOG_TYPE.NETWORK_SERVER_INFO);
        //
        GameNetUser netUser = new GameNetUser(netPlayerData.playerName, netPlayerData.connectionID, netPlayerData.selectChType);
        _netUserList.Add(netUser);
    }

    private void OnRecvFromClient_ReqGameUserList(NetworkMessage netMsg)
    {
        var netClientInfo = netMsg.ReadMessage<ReqNetClientInfo>();
        // 데이터 세팅.
        GameNetUserData[] userDatas = new GameNetUserData[_netUserList.Count];
        for(int idx = 0; idx < _netUserList.Count; idx++)
        {
            userDatas[idx].connectionID = _netUserList[idx].connectionID;
            userDatas[idx].selectCharType = _netUserList[idx].selectCharType;
            userDatas[idx].userName = _netUserList[idx].userName;
        }
        ResGameUserList resUserList = new ResGameUserList();
        resUserList.userList = userDatas;
        //
        ResNetUserList(netClientInfo.connectionID, resUserList);
    }
    private void ResNetUserList(int clientConnID, ResGameUserList resUserList)
    {
        NetworkServer.SendToClient(clientConnID, (short)GAME_NETWORK_PROTOCOL.res_inGameUserList,
            resUserList);
    }

    private void OnRecvFromServer_GameUserList(NetworkMessage netMsg)
    {
        var userListInfo = netMsg.ReadMessage<ResGameUserList>();
        KojeomLogger.DebugLog("서버로부터 유저리스트를 응답받았습니다.",LOG_TYPE.NETWORK_CLIENT_INFO);
        var userList = userListInfo.userList;
        for(int idx = 0; idx < userListInfo.userList.GetLength(0); idx++)
        {
            KojeomLogger.DebugLog(string.Format("[user_info] connID : {0}, name : {1}, charType : {2}",
                userList[idx].connectionID, userList[idx].userName,
                userList[idx].selectCharType));
        }
    }


    public void ReqInGameUserList()
    {
        ReqNetClientInfo reqClientInfo = new ReqNetClientInfo();
        reqClientInfo.connectionID = client.connection.connectionId;
        bool isSuccess = client.connection.Send((short)GAME_NETWORK_PROTOCOL.req_inGameUserList, reqClientInfo);

        if (isSuccess == true) KojeomLogger.DebugLog("게임유저리스트 요청을 서버로 전달했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        else KojeomLogger.DebugLog("게임유저리스트 요청이 실패했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    // 테스트 메소드. 퍼포먼스문제가 있다.
    public bool IsSameUserInList(int connectionID)
    {
        var user = _netUserList.Find((netUser) => 
        {
            if(netUser.connectionID == connectionID)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        if (user == null) return false;
        else return true;
    }
    // 테스트 메소드. 퍼포먼스문제가 있다.
    public GameNetUser FindUserInList(int connectionID)
    {
        var user = _netUserList.Find((netUser) =>
        {
            if (netUser.connectionID == connectionID)
            {
                return true;
            }
            else
            {
                return false;
            }
        });
        return user;
    }
    // 테스트 메소드. 퍼포먼스문제가 있다.
    public GamePlayer GetMyGamePlayer()
    {
        var user = FindUserInList(client.connection.connectionId);
        if((user != null) && (user.gamePlayer != null))
        {
            if(user.gamePlayer.isMyPlayer) return user.gamePlayer;
        }
        return null;
    }
    
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        var msg = extraMessageReader.ReadMessage<GameNetPlayerData>();
        KojeomLogger.DebugLog(string.Format("[method::OnServerAddPlayer] netConn : {0}, playerControllerId : {1}",
           conn, playerControllerId), LOG_TYPE.NETWORK_SERVER_INFO);
        // instancing..
        GameObject instance = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        instance.name = msg.playerName;
        // gamePlayer info init.
        GamePlayer gamePlayer = instance.GetComponent<GamePlayer>();
        gamePlayer.Init(msg.selectChType, msg.playerName, PlayerManager.myPlayerInitPosition);
        // 네트워크 서버에 플레이어 등록.
        NetworkServer.AddPlayerForConnection(conn, instance, playerControllerId);
    }
}



