using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class GamePlayer : NetworkBehaviour
{
    public bool isMyPlayer;
    private GamePlayerController playerController;
    private int _characterType;
    private string _characterName;
    private GameCharacter _charInstance;
    public GameCharacter charInstance
    {
        get { return _charInstance; }
    }

    private int _netConnectionId;
    public int netConnectionId
    {
        get { return _netConnectionId; }
    }

    private NetworkIdentity networkIdentity;

    public void Init(int charType, string charName, Vector3 initPos, int netConnId = 0)
    {
        // init position.
        gameObject.transform.position = initPos;
        //test code.   
        DontDestroyOnLoad(this);
        //
        KojeomLogger.DebugLog("GamePlayer Init ", LOG_TYPE.INFO);
        _netConnectionId = netConnId;
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
        isMyPlayer = true;
        KojeomLogger.DebugLog("[START_CLIENT] this gameplayer client with Authority", LOG_TYPE.NETWORK_CLIENT_INFO);
    }

    public override void OnStartClient()
    {
        KojeomLogger.DebugLog("[START_CLIENT] this gameplayer client", LOG_TYPE.NETWORK_CLIENT_INFO);
    }
}
