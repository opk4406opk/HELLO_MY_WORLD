using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine.Networking;
/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class PlayerManager : MonoBehaviour {
    /// <summary>
    /// 캐릭터 프리팹들.
    /// 배열의 인덱스번호는 캐릭터의 type 값과 1:1로 매칭된다.
    /// </summary>
    [SerializeField]
    private GameObject[] charPrefabs;

    private GameObject _myGamePlayer;
    public GameObject myGamePlayer
    {
        get { return _myGamePlayer; }
    }
    //
    public Vector3 gamePlayerInitPosition;
    private PlayerController myPlayerController;
    //
    public static PlayerManager instance;
    //
    private GameObject gamePlayerPrefab;
    private List<GamePlayer> gamePlayerList;
    public void Init()
    {
        gamePlayerList = new List<GamePlayer>();
        charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        if(GameStatus.isMultiPlay == true)
        {
            gamePlayerPrefab = GameNetworkManager.singleton.playerPrefab;
        }
        else if(GameStatus.isMultiPlay == false)
        {
            gamePlayerPrefab = Resources.Load<GameObject>(ConstFilePath.GAME_NET_PLAYER_PREFAB);
        }
        CreateProcess();
        instance = this;
    }
    /// <summary>
    /// 플레이어 컨트롤러를 시작합니다.
    /// </summary>
    public void StartController()
    {
        myPlayerController.StartControllProcess();
    }
    /// <summary>
    /// 플레이어 컨트롤러를 중지합니다.
    /// </summary>
    public void StopController()
    {
        myPlayerController.StopControllProcess();
    }

    private void CreateProcess()
    {
        string chType = System.String.Empty;
        Action GetUserInfo = () =>
        {
            StringBuilder conn = new StringBuilder();
            conn.AppendFormat("URI=file:{0}/StreamingAssets/GameUserDB/userDB.db", Application.dataPath);
            IDbConnection dbconn;
            IDbCommand dbcmd;
            using (dbconn = (IDbConnection)new SqliteConnection(conn.ToString()))
            {
                using (dbcmd = dbconn.CreateCommand())
                {
                    dbconn.Open(); //Open connection to the database.
                    string sqlQuery = "SELECT type FROM USER_SELECT_CHARACTER";
                    dbcmd.CommandText = sqlQuery;
                    IDataReader reader = dbcmd.ExecuteReader();
                    reader.Read();
                    // select로 가져온 정보의 0번째 필드값을 가져온다. ( 여기서는 type 값이 된다. )
                    chType = reader.GetString(0);
                    reader.Close();
                    reader = null;
                }
                dbconn.Close();
            }
        };
        GetUserInfo();
        //
        if(GameStatus.isMultiPlay == true)
        {
            KojeomLogger.DebugLog("멀티모드로 캐릭터를 생성.");
            CreatePlayerMultiMode(int.Parse(chType));
        }
        else if(GameStatus.isMultiPlay == false)
        {
            CreatePlayerSingleMode(int.Parse(chType));
        }
    }

    private void CreatePlayerSingleMode(int myChType)
    {
        //싱글모드 이므로, 본인만 생성하면 된다.
        CreateGamePlayer(myChType, gamePlayerInitPosition, "MyPlayer");
        //0번째 플레이어는 본인임을 의미한다.
        _myGamePlayer = gamePlayerList[0].GetComponent<GamePlayer>().charInstance.gameObject;
        myPlayerController = gamePlayerList[0].GetComponent<GamePlayer>().GetController();
    }

    private void CreatePlayerMultiMode(int myChType)
    {
        // 우선, 본인 캐릭터부터 생성.
        CreateGamePlayerForMulti(myChType, gamePlayerInitPosition, "MyPlayer", true);
        //0번째 플레이어는 본인임을 의미한다.
        _myGamePlayer = gamePlayerList[0].GetComponent<GamePlayer>().charInstance.gameObject;
        myPlayerController = gamePlayerList[0].GetComponent<GamePlayer>().GetController();
        //
    }

    private void CreateGamePlayer(int chType, Vector3 initPos, string playerName)
    {
        // playerManager로 패런팅.
        GameObject inst = Instantiate(gamePlayerPrefab, initPos, Quaternion.identity);
        inst.transform.parent = gameObject.transform;
        inst.name = playerName;
        //
        GamePlayer gamePlayer = inst.GetComponent<GamePlayer>();
        gamePlayer.Init(MakeGameChararacter(charPrefabs[chType]), chType);
        gamePlayerList.Add(gamePlayer);
    }
   
    private void CreateGamePlayerForMulti(int chType, Vector3 initPos, string playerName, bool isAutho)
    {
        // playerManager로 패런팅.
        GameObject inst = Instantiate(gamePlayerPrefab, initPos, Quaternion.identity);
        inst.transform.parent = gameObject.transform;
        inst.name = playerName;
        // spawnning from server.
        if (isAutho == false) GameNetworkSpawner.GetInstance().CmdSpawnFromServer(inst);
        else GameNetworkSpawner.GetInstance().CmdSpawnWithAuthoFromServer(inst);

        GamePlayer gamePlayer = inst.GetComponent<GamePlayer>();
        gamePlayer.Init(MakeGameChararacter(charPrefabs[chType]), chType);
        gamePlayerList.Add(gamePlayer);
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

}
