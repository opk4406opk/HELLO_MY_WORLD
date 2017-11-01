using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Text;

/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class PlayerManager : MonoBehaviour {
    // 캐릭터 프리팹들.
    [SerializeField]
    private GameObject[] charPrefabs;

    private GameObject _gamePlayer;
    public GameObject gamePlayer
    {
        get { return _gamePlayer; }
    }

    public Vector3 initPosition;

    public static PlayerManager instance;

    public void Init()
    {
        charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        CreateProcess();
        instance = this;
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
                    string sqlQuery = "SELECT type FROM USER_INFO";
                    dbcmd.CommandText = sqlQuery;
                    IDataReader reader = dbcmd.ExecuteReader();
                    // 임시로 0번 레코드의 필드의 값만 쓴다. 
                    reader.Read();
                    chType = reader.GetString(0);
                    reader.Close();
                    reader = null;
                }
                dbconn.Close();
            }
        };
        GetUserInfo();

        // instance character.
        InstanceCharacter(charPrefabs[int.Parse(chType)]);
    }
   
    private void InstanceCharacter(GameObject _prefab)
    {
        _gamePlayer = Instantiate(_prefab,
            initPosition,
            new Quaternion(0, 0, 0, 0)) as GameObject;
        PlayerController controller = _gamePlayer.GetComponent<PlayerController>();
        if(controller != null) controller.Init(Camera.main);
    }
}
