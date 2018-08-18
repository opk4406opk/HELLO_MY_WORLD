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
    public GamePlayer myGamePlayer;
    //
    private GamePlayerController myPlayerController;
    //
    public static PlayerManager instance;
    //
    private GameObject gamePlayerPrefab;
    public void Init()
    {
        gamePlayerPrefab = GameNetworkManager.singleton.playerPrefab;
        CreateProcess();
        instance = this;
    }

    public static Vector3 GetGamePlayerInitPos()
    {
        //임시로 정해진 값.
        return new Vector3(0.0f, 48.0f, 0.0f);
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
            conn.AppendFormat(GameDBHelper.GetInstance().GetDBConnectionPath(), Application.dataPath);
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
        myGamePlayer = GameNetworkManager.GetInstance().GetMyGamePlayer();
        // 멀티게임의 경우, gameUserList를 요청한다.
        if (GameStatus.isMultiPlay == true)
        {
            GameNetworkManager.GetInstance().ReqInGameUserList();
        }
        myGamePlayer.GetController().Init(Camera.main, myGamePlayer);
        //Player Manager 하위 종속으로 변경.
        myGamePlayer.transform.parent = gameObject.transform;
        myPlayerController = myGamePlayer.GetController();
    }

    [Obsolete("this is legacy method. Don't used it.")]
    private GamePlayer CreateSingleGamePlayer(int chType, string playerName)
    {
        // playerManager로 패런팅.
        GameObject inst = Instantiate(gamePlayerPrefab, new Vector3(0,0,0), Quaternion.identity);
        inst.name = playerName;
        var netIdentity = gameObject.GetComponent<NetworkIdentity>();
        
        //
        GamePlayer gamePlayer = inst.GetComponent<GamePlayer>();
        gamePlayer.Init(chType, playerName, GetGamePlayerInitPos());

        return gamePlayer;
    }
}
