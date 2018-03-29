using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GamePlayer : NetworkBehaviour
{
    [SerializeField]
    private bool _isMyPlayer;
    public bool isMyPlayer
    {
        get { return _isMyPlayer; }
    }
    private GamePlayerController playerController;
    private int _characterType;
    private string _characterName;
    private GameCharacter _charInstance;
    public GameCharacter charInstance
    {
        get { return _charInstance; }
    }

    [SerializeField]
    private int netConnectionId;
    private NetworkIdentity networkIdentity;

    public void Init(int charType, string charName, Vector3 initPos, int netConnId = 0)
    {
        // init position.
        gameObject.transform.position = initPos;
        //test code.   
        DontDestroyOnLoad(this);
        //
        KojeomLogger.DebugLog("GamePlayer Init ", LOG_TYPE.INFO);
        netConnectionId = netConnId;
        _characterName = charName;
        _characterType = charType;
        _charInstance = MakeGameChararacter(PrefabStorage.GetInstance().GetCharacterPrefab(charType));
        // 캐릭터 인스턴스는 게임플레이어 하위종속으로 설정.
        _charInstance.transform.parent = gameObject.transform;
        _charInstance.transform.localPosition = new Vector3(0, 0, 0);
        //
        playerController = gameObject.GetComponent<GamePlayerController>();
        networkIdentity = gameObject.GetComponent<NetworkIdentity>();
    }
    public GamePlayerController GetController()
    {
        return playerController;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public NetworkIdentity GetNetworkIdentity()
    {
        return networkIdentity;
    }

    public int GetNetworkConnectionID()
    {
        return netConnectionId;
    }
    
    public bool IsMyPlayer()
    {
        return _isMyPlayer;
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

    public override void OnStartAuthority()
    {
        _isMyPlayer = true;
        KojeomLogger.DebugLog(string.Format("[connectionID : {0}] this gameplayer client with Authority", netConnectionId),
            LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public override void OnStartClient()
    {
        KojeomLogger.DebugLog(string.Format("[connectionID : {0}] this gameplayer client", netConnectionId),
            LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public override void OnStartLocalPlayer()
    {
        KojeomLogger.DebugLog(string.Format("[connectionID : {0}] this gameplayer LocalPlayer", netConnectionId),
           LOG_TYPE.NETWORK_CLIENT_INFO);
        // 로컬 플레이어로서 초기화되면, 유저리스트에서 본인을 찾아 게임플레이어 객체를 할당해준다.
        var myUser = GameNetworkManager.GetInstance().FindUserInList(netConnectionId);
        myUser.gamePlayer = this;
    }
}
