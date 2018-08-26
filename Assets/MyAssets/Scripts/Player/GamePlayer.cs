using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GamePlayer : NetworkBehaviour
{
    [SerializeField]
    private bool _isMyPlayer = false;
    public bool isMyPlayer
    {
        get { return _isMyPlayer; }
    }
    private GamePlayerController playerController;
    [SerializeField]
    private int _characterType;
    [SerializeField]
    private string _characterName;
    private GameCharacter _charInstance;
    public GameCharacter charInstance
    {
        get { return _charInstance; }
    }

    private bool _isInitProcessFinish = false;
    public bool isInitProcessFinish
    {
        get { return _isInitProcessFinish; }
    }

    [SerializeField]
    private NetworkIdentity networkIdentity;

    /// <summary>
    /// 게임플레이어는 게임 시작 이전에 미리 생성되므로, DontDestroyOnLoad를 명시적으로 호출해야한다.
    /// </summary>
    public void PreInit()
    {
        KojeomLogger.DebugLog(string.Format("게임플레이어 PreInit 시작."), LOG_TYPE.INFO);
        DontDestroyOnLoad(this);
        KojeomLogger.DebugLog(string.Format("게임플레이어 PreInit 완료."), LOG_TYPE.INFO);
    }

    private void PostInit(int charType, string charName, Vector3 initPos)
    {
        // init position.
        gameObject.transform.position = initPos;
        gameObject.name = charName;
        //
        KojeomLogger.DebugLog("게임플레이어 PostInit 시작", LOG_TYPE.INFO);
        _characterName = charName;
        _characterType = charType;
        _charInstance = MakeGameChararacter(PrefabStorage.GetInstance().GetCharacterPrefab(charType));
        // 캐릭터 인스턴스는 게임플레이어 하위종속으로 설정.
        _charInstance.transform.parent = gameObject.transform;
        _charInstance.transform.localPosition = new Vector3(0, 0, 0);
        //
        playerController = gameObject.GetComponent<GamePlayerController>();
        //
        KojeomLogger.DebugLog("게임플레이어 PostInit 완료. ", LOG_TYPE.INFO);
    }
    /// <summary>
    /// 
    /// </summary>
    private void SingleGameInit()
    {

    }
    public GamePlayerController GetController()
    {
        return playerController;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    private void SetObjectLayer(bool isMine)
    {
        int layer = -999;
        if (isMine == true) layer = LayerMask.NameToLayer("PlayerCharacter");
        else layer = LayerMask.NameToLayer("OtherPlayerCharacter");

        gameObject.layer = layer;
        var childObjects = KojeomUtility.GetChilds<GameObject>(gameObject);
        foreach (var child in childObjects)
        {
            child.layer = layer;
        }
    }

    private GameCharacter MakeGameChararacter(GameObject _prefab)
    {
        GameObject characterObject = Instantiate(_prefab, new Vector3(0, 0, 0),
            new Quaternion(0, 0, 0, 0)) as GameObject;
        //
        GameCharacter gameChar = characterObject.GetComponent<GameCharacter>();
        gameChar.Init();
        return gameChar;
    }
    /// <summary>
    /// 컨트롤 가능한 오브젝트면 권한을 가진다.
    /// This is called after OnStartServer and OnStartClient.
    /// </summary>
    public override void OnStartAuthority()
    {
        KojeomLogger.DebugLog(string.Format("[OnStartAuthority] connID : {0}",
            GameNetworkManager.GetInstance().client.connection.connectionId), LOG_TYPE.NETWORK_CLIENT_INFO);
        _isMyPlayer = true;
    }

    /// <summary>
    /// 게임내에 존재하는 오브젝트들은 이 메소드가 호출되어진다.
    /// ( 컨트롤 권한이 있든 없든간에 )
    ///  Called on every NetworkBehaviour when it is activated on a client. 
    /// </summary>
    public override void OnStartClient()
    {
        PreInit();
        KojeomLogger.DebugLog(string.Format("[OnStartClient] connID : {0}",
           GameNetworkManager.GetInstance().client.connection.connectionId), LOG_TYPE.NETWORK_CLIENT_INFO);
        StartCoroutine(LateRegisterGamePlayerToUserList());
    }

    /// <summary>
    /// This happens after OnStartClient(), as it is triggered by an ownership message from the server.
    /// This is an appropriate place to activate componentsor functionality
    /// that should only be active for the local player, such as cameras and input.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
       
    }
    
    private IEnumerator LateRegisterGamePlayerToUserList()
    {
        KojeomLogger.DebugLog(string.Format("[Start]LateRegisterGamePlayerToUserList Start."), LOG_TYPE.NETWORK_CLIENT_INFO);
        while (true)
        {
            KojeomLogger.DebugLog(string.Format("Waiting for NetUserList..."), LOG_TYPE.NETWORK_CLIENT_INFO);
            if ((GameNetworkManager.GetInstance() != null) &&
                (GameNetworkManager.GetInstance().netUserList.Count > 0))
            {
                var netConnectionID = -999;
                if (GameNetworkManager.GetInstance().isHost == true) netConnectionID = networkIdentity.connectionToClient.connectionId;
                else netConnectionID = networkIdentity.connectionToServer.connectionId;
                KojeomLogger.DebugLog(string.Format("[※LateRegisterGamePlayer Done.※] connID : {0}", netConnectionID),
                   LOG_TYPE.NETWORK_CLIENT_INFO);
                var user = GameNetworkManager.GetInstance().FindUserInList(netConnectionID);
                PostInit(user.selectCharType, user.userName, PlayerManager.GetGamePlayerInitPos());
                user.gamePlayer = this;
                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
        _isInitProcessFinish = true;
        KojeomLogger.DebugLog(string.Format("[Finish]LateRegisterGamePlayerToUserList Finish."), LOG_TYPE.NETWORK_CLIENT_INFO);
    }
}
