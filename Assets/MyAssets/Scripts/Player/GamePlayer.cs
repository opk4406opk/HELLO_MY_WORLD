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
    [SerializeField]
    private int _characterType;
    [SerializeField]
    private string _characterName;
    private GameCharacter _charInstance;
    public GameCharacter charInstance
    {
        get { return _charInstance; }
    }
    private NetworkIdentity networkIdentity;

    public void Init(int charType, string charName, Vector3 initPos)
    {
        // init position.
        gameObject.transform.position = initPos;
        //test code.   
        DontDestroyOnLoad(this);
        //
        KojeomLogger.DebugLog("게임플레이어 초기화 시작. ", LOG_TYPE.INFO);
        _characterName = charName;
        _characterType = charType;
        _charInstance = MakeGameChararacter(PrefabStorage.GetInstance().GetCharacterPrefab(charType));
        // 캐릭터 인스턴스는 게임플레이어 하위종속으로 설정.
        _charInstance.transform.parent = gameObject.transform;
        _charInstance.transform.localPosition = new Vector3(0, 0, 0);
        //
        playerController = gameObject.GetComponent<GamePlayerController>();
        KojeomLogger.DebugLog("게임플레이어 초기화 완료. ", LOG_TYPE.INFO);
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
        networkIdentity = gameObject.GetComponent<NetworkIdentity>();
        _isMyPlayer = true;
        KojeomLogger.DebugLog(string.Format("[connectionID : {0}] this gameplayer client with Authority",
            networkIdentity.connectionToServer.connectionId),
            LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public override void OnStartClient()
    {
        KojeomLogger.DebugLog("this gameplayer is just client..", LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public override void OnStartLocalPlayer()
    {
        networkIdentity = gameObject.GetComponent<NetworkIdentity>();
        KojeomLogger.DebugLog(string.Format("[connectionID : {0}] this gameplayer LocalPlayer",
            networkIdentity.connectionToServer.connectionId),
           LOG_TYPE.NETWORK_CLIENT_INFO);
        // 로컬 플레이어로서 초기화되면, 유저리스트에서 본인을 찾아 게임플레이어 객체를 할당해준다.
        var myUser = GameNetworkManager.GetInstance().FindUserInList(networkIdentity.connectionToServer.connectionId);
        if(GameNetworkManager.GetInstance().isHost == false)
        {
            Init(myUser.selectCharType, myUser.userName, PlayerManager.myPlayerInitPosition);
        }
        myUser.gamePlayer = this;
    }
}
