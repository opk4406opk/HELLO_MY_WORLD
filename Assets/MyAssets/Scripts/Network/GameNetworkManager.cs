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
    req_inGameUserList = 1002,
    push_ChatMsgToServer = 1003,
    res_ChatMsgToAllUser = 1004,
    push_charStateToServer = 1005,
    res_charStateToAllUser = 1006,
    req_gameRandomSeed = 1007,
    res_gameRandomSeed = 1008
}

public class NetMessageGameRandSeed : MessageBase
{
    public int connectionID;
    public int randomSeed;
}

public class NetMessageGameCharState : MessageBase
{
    public GAMEPLAYER_CHAR_STATE ownerCharState;
    public int ownerConnID;
}

public class NetMessageGameChat : MessageBase
{
    public string gameChatMessage;
}

public class NetMessageGameNetPlayerData : MessageBase
{
    public string playerName;
    public int connectionID;
    public string address;
    public int selectChType;
}

// 서버로 특정 데이터를 요청시에 보내는 클라이언트 정보 메세지.
public class NetMessageNetClientInfo : MessageBase
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
public class NetMessageGameUserList : MessageBase
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
    5. Push : 클라이언트에서 서버로 데이터를 전달할 때 ( push )
*/

/// <summary>
/// 게임 네트워크 상태에 대한 flag 변수를 모아놓은 클래스.
/// </summary>
public class GameNetworkStateFlags
{
    /// <summary>
    /// 서버로부터 Game에서 사용될 RandomSeed를 정상 수신 받았는지?
    /// </summary>
    public static bool isReceivedRandomSeedFormServer = false;
    /// <summary>
    /// 서버로부터 Game UserList를 정상 수신 받았는지?
    /// </summary>
    public static bool isReceiveGameUserList = false;
    /// <summary>
    /// 서버에 접속 후 나의 GamePlayer가 정상적으로 생성되었는지?
    /// </summary>
    public static bool isCreatedMyGamePlayer = false;
}

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
    
    private Dictionary<int, GameNetUser> _netUserList = new Dictionary<int, GameNetUser>();
    public Dictionary<int, GameNetUser> netUserList
    {
        get { return _netUserList; }
    }
    /// <summary>
    /// 서버와 클라이언트 랜덤함수를 동기화하기 위해 Seed값을 일치
    /// </summary>
    public static void InitGameRandomSeed(int seed)
    {
        gameRandomSeed = seed;
        KojeomUtility.SetRandomSeed(seed);
        KojeomLogger.DebugLog(string.Format("GameRandomSeed Init. seed value : {0}", seed), LOG_TYPE.NETWORK_MANAGER_INFO);
    }
   
    private static int gameRandomSeed = 0;
    public static int GetGameCurrentRandomSeed()
    {
        return gameRandomSeed;
    }

    private static GameNetworkManager instance;
    public static GameNetworkManager GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindWithTag("NetworkManager").GetComponent<GameNetworkManager>();
        }
        return instance;
    }
    
    /// <summary>
    /// 서버 및 클라이언트 세팅을 하는 함수.
    /// 플레이어가 Host 혹은 Client로 시작하는 함수가 호출되고 난 뒤에 이 함수가 호출되어야 한다.
    /// </summary>
    public void LateInit()
    {
        KojeomLogger.DebugLog("게임네트워크 매니저 늦은 초기화(서버/클라이언트 셋팅) 시작.", LOG_TYPE.NETWORK_MANAGER_INFO);
        ServerSettings();
        ClientSettings();
        KojeomLogger.DebugLog("게임네트워크 매니저 늦은 초기화(서버/클라이언트 셋팅) 완료.", LOG_TYPE.NETWORK_MANAGER_INFO);
    }

    private void ServerSettings()
    {
        //server setting
        // 프로토콜에 대한 메소드를 등록한다.
        NetworkServer.RegisterHandler((short)GAME_NETWORK_PROTOCOL.push_clientInfo,
            OnRecvFromClient_ConnectInfo);
        NetworkServer.RegisterHandler((short)GAME_NETWORK_PROTOCOL.req_inGameUserList,
            OnRecvFromClient_ReqGameUserList);
        NetworkServer.RegisterHandler((short)GAME_NETWORK_PROTOCOL.push_ChatMsgToServer,
            OnRecvFromClient_PushGameChatMsg);
        NetworkServer.RegisterHandler((short)GAME_NETWORK_PROTOCOL.push_charStateToServer,
            OnRecvFromClient_CharState);
        NetworkServer.RegisterHandler((short)GAME_NETWORK_PROTOCOL.req_gameRandomSeed,
            OnRecvFromClient_ReqGameRandSeed);
        
    }
    private void ClientSettings()
    {
        //client setting
        client.RegisterHandler((short)GAME_NETWORK_PROTOCOL.res_inGameUserList,
            OnRecvFromServer_GameUserList);
        client.RegisterHandler((short)GAME_NETWORK_PROTOCOL.res_ChatMsgToAllUser,
            OnRecvFromServer_gameChatMsg);
        client.RegisterHandler((short)GAME_NETWORK_PROTOCOL.res_charStateToAllUser,
            OnRecvFromServer_GameCharState);
        client.RegisterHandler((short)GAME_NETWORK_PROTOCOL.res_gameRandomSeed,
            OnRecvFromServer_gameRandomSeed);
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
        KojeomLogger.DebugLog(string.Format("[OnServerConnect] connection INFO :{0} 서버에 접속했습니다.", conn),
            LOG_TYPE.NETWORK_SERVER_INFO);
    }

    // client입장에서 서버에 접속시에 불려지는 콜백 메소드.
    // 이 메소드 코드 흐름을 보기좋게 정리할 필요가 있다.
    public override void OnClientConnect(NetworkConnection conn)
    {
        KojeomLogger.DebugLog("서버로 접속을 했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        //서버한테 Game에서 사용하게 될 RandomSeed를 요청한다.
        ReqGameRandomSeed();
        // 게임유저 데이터 메세지 생성.
        NetMessageGameNetPlayerData msgData = new NetMessageGameNetPlayerData();
        msgData.connectionID = conn.connectionId;
        msgData.address = conn.address;
        msgData.playerName = string.Format("[GameClient]_player_connID::{0}", conn.connectionId);
        msgData.selectChType = GameDBHelper.GetInstance().GetSelectCharType();

        // NetworkManager 프리팹에서 autoCreatePlayer 옵션을 true로 하는 경우,
        // 해당 로컬 컨넥션에 대해 자동으로 ClientScene.AddPlayer(0)을 호출한다.
        //base.OnClientConnect(conn);
        //ClientScene.Ready(conn);
        bool isSuccesAddPlayer = ClientScene.AddPlayer(conn, (short)conn.connectionId, msgData);
        if (isSuccesAddPlayer == true) KojeomLogger.DebugLog("ClientScene.AddPlayer is Success.", LOG_TYPE.NETWORK_CLIENT_INFO);
        else KojeomLogger.DebugLog("ClientScene.AddPlayer is Failed.", LOG_TYPE.NETWORK_CLIENT_INFO);
    }
    
    private void ResCharStateToAllUsers(NetMessageGameCharState charStateMsg)
    {
        foreach(var user in _netUserList)
        {
            // character state 변화가 일어난 유저를 제외한 나머지 접속중인 유저들에게
            // state 변화 정보를 전달해준다.
            if(charStateMsg.ownerConnID != user.Value.connectionID)
            {
                NetworkServer.SendToClient(user.Value.connectionID,
                    (short)GAME_NETWORK_PROTOCOL.res_charStateToAllUser, charStateMsg);
                KojeomLogger.DebugLog(string.Format("client[id:{0}] 에게 client[id:{1}]의state정보를 모든 클라이언트에 전송 했습니다.",
                    user.Value.connectionID, charStateMsg.ownerConnID), LOG_TYPE.NETWORK_SERVER_INFO);
            }
        }
    }
    #region Receive from Client
    /// <summary>
    /// 클라이언트로부터 받은 접속정보를 로그에 남기기 위한 콜백함수.
    /// </summary>
    /// <param name="netMsg"></param>
    private void OnRecvFromClient_ConnectInfo(NetworkMessage netMsg)
    {
        var netPlayerData = netMsg.ReadMessage<NetMessageGameNetPlayerData>();
        KojeomLogger.DebugLog(string.Format("conneted clinet info [ connection_id : {0}, addr : {1}, selectChType : {2} ]",
            netPlayerData.connectionID, netPlayerData.address, netPlayerData.selectChType), LOG_TYPE.NETWORK_SERVER_INFO);
    }

    private void OnRecvFromClient_CharState(NetworkMessage netMsg)
    {
        var clientCharState = netMsg.ReadMessage<NetMessageGameCharState>();
        KojeomLogger.DebugLog(string.Format("connID : {0} current chracter state : {1}",
            clientCharState.ownerConnID, clientCharState.ownerCharState), LOG_TYPE.NETWORK_SERVER_INFO);
        ResCharStateToAllUsers(clientCharState);
    }
    private void OnRecvFromClient_ReqGameUserList(NetworkMessage netMsg)
    {
        var netClientInfo = netMsg.ReadMessage<NetMessageNetClientInfo>();
        // 데이터 세팅.
        GameNetUserData[] userDatas = new GameNetUserData[_netUserList.Count];
        for(int idx = 0; idx < _netUserList.Count; idx++)
        {
            userDatas[idx].connectionID = _netUserList[idx].connectionID;
            userDatas[idx].selectCharType = _netUserList[idx].selectCharType;
            userDatas[idx].userName = _netUserList[idx].userName;
        }
        NetMessageGameUserList resUserList = new NetMessageGameUserList();
        resUserList.userList = userDatas;
        //
        ResNetUserList(netClientInfo.connectionID, resUserList);
    }
  

    private void OnRecvFromClient_ReqGameRandSeed(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NetMessageGameRandSeed>();
        NetMessageGameRandSeed responseNetMsg = new NetMessageGameRandSeed();
        responseNetMsg.randomSeed = GetGameCurrentRandomSeed();
        NetworkServer.SendToClient(msg.connectionID, (short)GAME_NETWORK_PROTOCOL.res_gameRandomSeed, responseNetMsg);
        KojeomLogger.DebugLog(string.Format("GameRandomSeed val :{0} 을 클라이언트(connID : {1})에게 전송했습니다.", 
            responseNetMsg.randomSeed, msg.connectionID),
            LOG_TYPE.NETWORK_SERVER_INFO);
    }

    private void OnRecvFromClient_PushGameChatMsg(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NetMessageGameChat>();
        NetMessageGameChat responseChatMsg = new NetMessageGameChat();
        responseChatMsg.gameChatMessage = msg.gameChatMessage;
        bool isSuccess = NetworkServer.SendToAll((short)GAME_NETWORK_PROTOCOL.res_ChatMsgToAllUser, responseChatMsg);

        if (isSuccess == true) KojeomLogger.DebugLog("게임채팅 메세지를 모든 클라이언트에 전송 했습니다.", LOG_TYPE.NETWORK_SERVER_INFO);
        else KojeomLogger.DebugLog("게임채팅 메세지를 모든 클라이언트에 전송 실패했습니다.", LOG_TYPE.NETWORK_SERVER_INFO);
    }
    #endregion

    #region Receive from Server
    private void OnRecvFromServer_GameUserList(NetworkMessage netMsg)
    {
        var userListInfo = netMsg.ReadMessage<NetMessageGameUserList>();
        KojeomLogger.DebugLog("서버로부터 유저리스트를 응답받았습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        var userList = userListInfo.userList;
        for (int idx = 0; idx < userListInfo.userList.GetLength(0); idx++)
        {
            KojeomLogger.DebugLog(string.Format("[user_info] connID : {0}, name : {1}, charType : {2}",
                userList[idx].connectionID, userList[idx].userName,
                userList[idx].selectCharType));
            // 서버로부터 받은 유저리스트를 클라이언트 유저리스트에 저장한다.
            GameNetUser netUser = new GameNetUser(userList[idx].userName, userList[idx].connectionID, userList[idx].selectCharType);
            if(_netUserList.ContainsKey(userList[idx].connectionID) == false)
            {
                _netUserList.Add(userList[idx].connectionID, netUser);
            }
        }
        GameNetworkStateFlags.isReceiveGameUserList = true;
    }

    private void OnRecvFromServer_GameCharState(NetworkMessage netMsg)
    {
        var gameCharState = netMsg.ReadMessage<NetMessageGameCharState>();
        if (gameCharState.ownerConnID != client.connection.connectionId)
        {
            var user = FindUserInList(gameCharState.ownerConnID);
            if (user.gamePlayer != null)
            {
                user.gamePlayer.GetController().SetPlayerState(gameCharState.ownerCharState);
            }
        }
        else
        {
            KojeomLogger.DebugLog("gameCharState.ownerConnID == client.connection.connectionId (OnRecvFromServer_GameCharState)",
                LOG_TYPE.NETWORK_CLIENT_INFO);
        }
    }

    private void OnRecvFromServer_gameChatMsg(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NetMessageGameChat>();
        KojeomLogger.DebugLog(string.Format("{0} : 채팅 메세지를 서버로부터 수신했습니다.", msg.gameChatMessage), 
            LOG_TYPE.NETWORK_CLIENT_INFO);
        if(InGameUISupervisor.singleton != null)
        {
            InGameUISupervisor.singleton.SetMsgToChattingLog(msg.gameChatMessage,
                client.connection.connectionId.ToString());
        }
    }

    private void OnRecvFromServer_gameRandomSeed(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NetMessageGameRandSeed>();
        KojeomLogger.DebugLog(string.Format("GameRandSeed val : {0} 을 서버로부터 수신했습니다.", msg.randomSeed),
           LOG_TYPE.NETWORK_CLIENT_INFO);
        InitGameRandomSeed(msg.randomSeed);
        GameNetworkStateFlags.isReceivedRandomSeedFormServer = true;
    }
    #endregion
    private void ResNetUserList(int clientConnID, NetMessageGameUserList resUserList)
    {
        NetworkServer.SendToClient(clientConnID, (short)GAME_NETWORK_PROTOCOL.res_inGameUserList,
            resUserList);
    }
    #region [Client] Request, Push 
    public void ReqInGameUserList()
    {
        NetMessageNetClientInfo reqClientInfo = new NetMessageNetClientInfo();
        reqClientInfo.connectionID = client.connection.connectionId;
        bool isSuccess = client.connection.Send((short)GAME_NETWORK_PROTOCOL.req_inGameUserList, reqClientInfo);

        if (isSuccess == true) KojeomLogger.DebugLog("게임유저리스트 요청을 서버로 전달했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        else KojeomLogger.DebugLog("게임유저리스트 요청이 실패했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public void ReqGameRandomSeed()
    {
        NetMessageGameRandSeed reqRandSeed = new NetMessageGameRandSeed();
        reqRandSeed.connectionID = client.connection.connectionId;
        bool isSuccess = client.connection.Send((short)GAME_NETWORK_PROTOCOL.req_gameRandomSeed, reqRandSeed);
        if (isSuccess == true) KojeomLogger.DebugLog("게임랜덤시드 요청을 서버로 전달했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        else KojeomLogger.DebugLog("게임랜덤시드 요청이 실패했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public void PushChatMessage(string chatMessage)
    {
        NetMessageGameChat pushChatMsg = new NetMessageGameChat();
        pushChatMsg.gameChatMessage = chatMessage;
        bool isSuccess = client.connection.Send((short)GAME_NETWORK_PROTOCOL.push_ChatMsgToServer, pushChatMsg);

        if (isSuccess == true) KojeomLogger.DebugLog("게임채팅 메세지 데이터를 서버로 전달했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        else KojeomLogger.DebugLog("게임채팅 메세지 데이터 전달에 실패했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public void PushCharStateMessage(GAMEPLAYER_CHAR_STATE charState)
    {
        bool isSuccess = false;
        NetMessageGameCharState pushStateMsg = new NetMessageGameCharState();
        if(PlayerManager.instance != null)
        {
            pushStateMsg.ownerCharState = PlayerManager.instance.myGamePlayer.GetController().GetPlayerState();
            pushStateMsg.ownerConnID = client.connection.connectionId;
            isSuccess = client.connection.Send((short)GAME_NETWORK_PROTOCOL.push_charStateToServer, pushStateMsg);

            if (isSuccess == false) KojeomLogger.DebugLog("현재 게임캐릭터 스테이트 정보를 서버에 전달 실패했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
            else KojeomLogger.DebugLog("현재 게임캐릭터 스테이트 정보를 서버에 전달 성공했습니다.", LOG_TYPE.NETWORK_CLIENT_INFO);
        }
    }
    #endregion
    public bool IsSameUserInList(int connectionID)
    {
        return _netUserList.ContainsKey(connectionID);
    }
    public GameNetUser FindUserInList(int connectionID)
    {
        GameNetUser user = null;
        _netUserList.TryGetValue(connectionID, out user);
        return user;
    }
    public GamePlayer GetMyGamePlayer()
    {
        var user = FindUserInList(client.connection.connectionId);
        if((user != null) && (user.gamePlayer != null))
        {
            if(user.gamePlayer.isMyPlayer) return user.gamePlayer;
        }
        return null;
    }

    /// <summary>
    /// 서버로 접속한 유저를 게임월드상에 실제하는 플레이어 오브젝트로 instancing 한다.
    /// 생성된 플레이어는 유저리스트에 등록.
    /// (서버에서만 호출되는 콜백 함수.)
    /// </summary>
    /// <param name="conn"></param>
    /// <param name="playerControllerId"></param>
    /// <param name="extraMessageReader"></param>
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId, NetworkReader extraMessageReader)
    {
        var msg = extraMessageReader.ReadMessage<NetMessageGameNetPlayerData>();
        KojeomLogger.DebugLog(string.Format("[OnServerAddPlayer] netConn : {0}, playerControllerId : {1}",
           conn, playerControllerId), LOG_TYPE.NETWORK_SERVER_INFO);
        // instancing..
        GameObject instance = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        instance.name = msg.playerName;
        // gamePlayer info init.
        GamePlayer gamePlayer = instance.GetComponent<GamePlayer>();
        gamePlayer.PreInit();
        // 네트워크 서버에 플레이어 등록.
        var addPlayerSuccess = NetworkServer.AddPlayerForConnection(conn, instance, playerControllerId);
        if (addPlayerSuccess) KojeomLogger.DebugLog(string.Format("Successed add Player to Server (connID : {0}", conn.connectionId),LOG_TYPE.NETWORK_SERVER_INFO);
        else KojeomLogger.DebugLog(string.Format("Failed add Player to Server (connID : {0}", conn.connectionId), LOG_TYPE.NETWORK_SERVER_INFO);
        // 서버에 접속한 유저의 캐릭터를 생성 후, 이를 유저리스트에 등록한다. 
        GameNetUser gameNetUser = new GameNetUser(msg.playerName, msg.connectionID, msg.selectChType, gamePlayer);
        if(_netUserList.ContainsKey(conn.connectionId) == false)
        {
            _netUserList.Add(conn.connectionId, gameNetUser);
        }
        else
        {
            KojeomLogger.DebugLog(string.Format("[OnServerAddPlayer] connection Info : {0} 는 이미 NetUserList에 있습니다. 새로 등록하지 않습니다.",
          conn), LOG_TYPE.NETWORK_SERVER_INFO);
        }
    }
    /// <summary>
    /// 로컬 Node.js 게임 로그 서버로 로그를 전달합니다.(http-post)
    /// ip : 127.0.0.1
    /// port : 8080
    /// </summary>
    /// <param name="log"></param>
    public static void LogPushToLoggerServer(string log)
    {
        //https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.Post.html
        var serverData = GameServerDataFile.singleton.GetGameServerData();
        string url = string.Format("httP://{0}:{1}", serverData.gamelog_server_ip,
            serverData.gamelog_server_port);
        WWWForm dataForm = new WWWForm();
        dataForm.AddField("log_data", log);
        using (UnityWebRequest www = UnityWebRequest.Post(url, dataForm))
        {
            // yield return 으로 예외처리를 해야하지만, 로그를 그냥 보내는것이므로 생략한다.
            www.SendWebRequest();
        }
    }
}



