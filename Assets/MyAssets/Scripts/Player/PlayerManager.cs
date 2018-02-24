using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;
using System.Collections.Generic;
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
    public Vector3 initPosition;
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
        gamePlayerPrefab = GameNetworkManager.singleton.playerPrefab;
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

        // create players.
        // 테스트로 1개의 플레이어만 존재한다고 가정./////////////////////////////////////////
        int characterType = int.Parse(chType);
        for(int idx = 0; idx < 1; idx++)
        {
            // playerManager로 패런팅.
            GameObject inst = Instantiate(gamePlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            inst.transform.parent = gameObject.transform;
            inst.name = "MyPlayer";
            //
            GamePlayer gamePlayer = inst.GetComponent<GamePlayer>();
            gamePlayer.Init(MakeGameChararacter(charPrefabs[characterType]), characterType);
            gamePlayerList.Add(gamePlayer);
        }
        //0번째 플레이어는 본인임을 의미한다.
        _myGamePlayer = gamePlayerList[0].GetComponent<GamePlayer>().charInstance.gameObject;
        myPlayerController = gamePlayerList[0].GetComponent<GamePlayer>().charInstance.GetController();
        myPlayerController.Init(Camera.main);
        //////////////////////////////////////////////////////////////////////////////////////
    }
   
    private GameCharacter MakeGameChararacter(GameObject _prefab)
    {
        GameObject characterObject = Instantiate(_prefab, initPosition,
            new Quaternion(0, 0, 0, 0)) as GameObject;
        return characterObject.GetComponent<GameCharacter>();
    }
}
