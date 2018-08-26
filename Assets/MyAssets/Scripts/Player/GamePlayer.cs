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
        //
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

    private void SetObjectLayer(int layer)
    {
        // 상위 
        gameObject.layer = layer;
        //하위
        _charInstance.gameObject.layer = layer;
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
    // 컨트롤 가능한 오브젝트면 권한을 가진다.
    public override void OnStartAuthority()
    {
        _isMyPlayer = true;
        StartCoroutine(LateRegisterMyPlayerToUserList());
        GameNetworkStateFlags.isCreatedMyGamePlayer = true;
    }

    /// <summary>
    /// 게임내에 존재하는 오브젝트들은 이 메소드가 호출되어진다.
    /// ( 컨트롤 권한이 있든 없든간에 )
    ///  Called on every NetworkBehaviour when it is activated on a client. 
    /// </summary>
    public override void OnStartClient()
    {
        
    }

    public override void OnStartLocalPlayer()
    {
       
    }
    //// test method.
    //private void SettingGamePlayerToNetUser()
    //{
    //    var user = GameNetworkManager.GetInstance().FindUserInList(networkIdentity.connectionToServer.connectionId);
    //    if (GameNetworkManager.GetInstance().isHost == false)
    //    {
    //        Init(user.selectCharType, user.userName, PlayerManager.GetGamePlayerInitPos());
    //    }
    //    user.gamePlayer = this;
    //}

    private IEnumerator LateRegisterMyPlayerToUserList()
    {
        KojeomLogger.DebugLog(string.Format("LateRegisterMyPlayerToUserList Start."), LOG_TYPE.NETWORK_CLIENT_INFO);
        while (true)
        {
            if((GameNetworkManager.GetInstance() != null) &&
                (GameNetworkManager.GetInstance().netUserList.Count > 0))
            {
                var myUser = GameNetworkManager.GetInstance().FindUserInList(GameNetworkManager.GetInstance().
                    client.connection.connectionId);
                if(myUser.gamePlayer != null)
                {
                    myUser.gamePlayer = this;
                }
                break;
            }
            yield return new WaitForSeconds(1.0f);
        }
        KojeomLogger.DebugLog(string.Format("LateRegisterMyPlayerToUserList Finish."), LOG_TYPE.NETWORK_CLIENT_INFO);
    }
}
