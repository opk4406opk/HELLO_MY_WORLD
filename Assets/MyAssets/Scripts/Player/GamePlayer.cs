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
    public GameCharacter charInstance { get; private set; }
    public bool isInitProcessFinish { get; private set; } = false;

    [SerializeField]
    private NetworkIdentity networkIdentity;
    private NetworkAnimator networkAnimator;

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
        charInstance = MakeGameChararacter(PrefabStorage.GetInstance().GetCharacterPrefab(charType));
        // 캐릭터 인스턴스는 게임플레이어 하위종속으로 설정.
        charInstance.transform.parent = gameObject.transform;
        charInstance.transform.localPosition = new Vector3(0, 0, 0);
        //
        playerController = gameObject.GetComponent<GamePlayerController>();
        networkAnimator = gameObject.AddComponent<NetworkAnimator>();
        // 네트워크 애니메이터를 붙이고 나서 디폴트로 생기는 Animator 컴포넌트를 disable 한다.
        gameObject.GetComponent<Animator>().enabled = false;
        // 캐릭터 인스턴스에 있는 실제 애니메이터 컴포넌트를 새롭게 등록.
        networkAnimator.animator = charInstance.GetAnimator();
        //SetObjectLayer(_isMyPlayer);
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
        var childObjects = KojeomUtility.GetChilds<GameObject>(charInstance.gameObject);
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
        base.OnStartAuthority();
        KojeomLogger.DebugLog(string.Format("[OnStartAuthority] connID : {0}",
            GameNetworkManager.GetNetworkManagerInstance().client.connection.connectionId), LOG_TYPE.NETWORK_CLIENT_INFO);
        _isMyPlayer = true;
    }

    /// <summary>
    /// 게임내에 존재하는 오브젝트들은 이 메소드가 호출되어진다.
    /// ( 컨트롤 권한이 있든 없든간에 )
    ///  Called on every NetworkBehaviour when it is activated on a client. 
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        _isMyPlayer = false;
        PreInit();
        StartCoroutine(LateRegisterGamePlayerToUserList());
    }

    /// <summary>
    /// This happens after OnStartClient(), as it is triggered by an ownership message from the server.
    /// This is an appropriate place to activate componentsor functionality
    /// that should only be active for the local player, such as cameras and input.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
    }
    
    private IEnumerator LateRegisterGamePlayerToUserList()
    {
        KojeomLogger.DebugLog(string.Format("[Start]LateRegisterGamePlayerToUserList Start."), LOG_TYPE.NETWORK_CLIENT_INFO);
        while (true)
        {
            if ((GameNetworkManager.GetNetworkManagerInstance() != null) &&
                (GameNetworkManager.GetNetworkManagerInstance().netUserList.Count > 0))
            {
                var netConnectionID = -999;
                if (networkIdentity.connectionToClient != null) netConnectionID = networkIdentity.connectionToClient.connectionId;
                else if(networkIdentity.connectionToServer != null) netConnectionID = networkIdentity.connectionToServer.connectionId;
                else if(networkIdentity.connectionToServer == null && networkIdentity.connectionToClient == null)
                {
                    KojeomLogger.DebugLog(string.Format("NetworkIdentity connection null pointer error."), LOG_TYPE.ERROR);
                }
                KojeomLogger.DebugLog(string.Format("[※LateRegisterGamePlayer Done.※] connID : {0}", netConnectionID),
                   LOG_TYPE.NETWORK_CLIENT_INFO);

                // 서버에서 Connection에 대해 유저를 생성하는 작업이 성공적으로 완료 되기 이전에
                // 게임플레이어가 먼저 로컬에 생성될 수 있다. 따라서, 유저리스트에서 해당 Connection에 대한 정보를
                // 가져올 수 있을때까지 계속 기다린다.
                // -> 접속, 생성에 따른 절차가 일목요연하게 정리되지 않아 일단 임시로 이러한 방법을 사용한다.
                GameNetUser user = null;
                while(true)
                {
                    user = GameNetworkManager.GetNetworkManagerInstance().FindUserInList(netConnectionID);
                    if (user != null) break;
                    yield return new WaitForSeconds(0.25f);
                }
                PostInit(user.selectCharType, user.userName, PlayerManager.GetGamePlayerInitPos());
                user.gamePlayer = this;
                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
        isInitProcessFinish = true;
        KojeomLogger.DebugLog(string.Format("[Finish]LateRegisterGamePlayerToUserList Finish."), LOG_TYPE.NETWORK_CLIENT_INFO);
    }
}
