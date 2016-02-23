using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;

/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class PlayerManager : MonoBehaviour {

    [SerializeField]
    private GameObject gang_Prefab;
    [SerializeField]
    private GameObject fireFighter_Prefab;
    [SerializeField]
    private GameObject police_Prefab;
    [SerializeField]
    private GameObject sheriff_Prefab;
    [SerializeField]
    private GameObject trucker_Prefab;

    private GameObject _gamePlayer;
    public GameObject gamePlayer
    {
        get { return _gamePlayer; }
    }

    public Vector3 initPosition;

    public void Init()
    {
        CreateProcess();
    }

    private delegate void del_GetUserInfo();
    private void CreateProcess()
    {
        string chName = System.String.Empty;
        del_GetUserInfo GetUserInfo = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/StreamingAssets/GameUserDB/userDB.db";

            IDbConnection dbconn = (IDbConnection)new SqliteConnection(conn);
            IDbCommand dbcmd = dbconn.CreateCommand();
            dbconn.Open(); //Open connection to the database.

            string sqlQuery = "SELECT name FROM USER_INFO";
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            // 임시로 0번 레코드의 Name 필드의 값만 쓴다. 
            reader.Read();
            chName = reader.GetString(0); 
            
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;

            dbconn.Close();
            dbconn = null;
        };
        GetUserInfo();

        switch(chName)
        {
            case "FireFighter":
                InstanceCharacter(fireFighter_Prefab);
                break;
            case "Gang":
                InstanceCharacter(gang_Prefab);
                break;
            case "Police":
                InstanceCharacter(police_Prefab);
                break;
            case "Sheriff":
                InstanceCharacter(sheriff_Prefab);
                break;
            case "Trucker":
                InstanceCharacter(trucker_Prefab);
                break;
            default:
                break;
        }
    }
   
    private void InstanceCharacter(GameObject _prefab)
    {
        _gamePlayer = Instantiate(_prefab,
            initPosition,
            new Quaternion(0, 0, 0, 0)) as GameObject;
    }
}
