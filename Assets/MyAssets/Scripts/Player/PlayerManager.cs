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
    //
    public GameObject myGamePlayer;

    //
    public static Vector3 myPlayerInitPosition = new Vector3(10.0f, 18.0f, 0.0f);
    private GamePlayerController myPlayerController;
    //
    public static PlayerManager instance;
    //
    private GameObject gamePlayerPrefab;
    public void Init()
    {
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
        CreatePlayer(int.Parse(chType));
    }

    private void CreatePlayer(int myChType)
    {
        if (GameStatus.isMultiPlay)
        {
            // to do
            myGamePlayer = GameNetworkManager.GetInstance().GetMyGamePlayer().gameObject;
        }
        else
        {
            //싱글모드 이므로, 본인만 생성하면 된다.
            myGamePlayer = CreateSingleGamePlayer(myChType, "MyPlayer");
        }
        myGamePlayer.GetComponent<GamePlayerController>().Init(Camera.main, myGamePlayer);
        myGamePlayer.transform.parent = gameObject.transform;
        myPlayerController = myGamePlayer.GetComponent<GamePlayer>().GetController();
    }

    private GameObject CreateSingleGamePlayer(int chType, string playerName)
    {
        // playerManager로 패런팅.
        GameObject inst = Instantiate(gamePlayerPrefab, new Vector3(0,0,0), Quaternion.identity);
        inst.name = playerName;
        //
        GamePlayer gamePlayer = inst.GetComponent<GamePlayer>();
        gamePlayer.Init(chType, playerName, myPlayerInitPosition);

        return inst;
    }
}
