using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

/// <summary>
/// DB에서 유저 아이템정보를 담을 구조체.
/// </summary>
public struct DBUserItem
{
    public string id;
    public string name;
    public string type;
    public int amount;
}
/// streamingAssets 경로에 대한 문서.
/// https://docs.unity3d.com/ScriptReference/Application-streamingAssetsPath.html
/// https://docs.unity3d.com/Manual/StreamingAssets.html
/// 안드로이드에서 sqlite3 사용 하는 방법 문서.
/// https://answers.unity.com/questions/872068/dllnotfoundexception-sqlite3.html
public class GameDBManager
{
    private static GameDBManager Instance;
    public static GameDBManager GetInstance()
    {
        if (Instance == null) Instance = new GameDBManager();
        return Instance;
    }
    /// <summary>
    /// 게임 DB파일이 있는 path.
    /// </summary>
    private string DBConnectionPath;
    private GameDBManager()
    {
        var platform = Application.platform;
        if (platform == RuntimePlatform.Android)
        {
            string dbFilePath = System.IO.Path.Combine(Application.streamingAssetsPath, "userDB.db");
            KojeomLogger.DebugLog(string.Format("Android DB file Path : {0}", dbFilePath), LOG_TYPE.DATABASE);
            if (File.Exists(dbFilePath))
            {
                KojeomLogger.DebugLog("DB file is Exist", LOG_TYPE.DATABASE);
            }
            else
            {
                KojeomLogger.DebugLog("DB file is Not Exist", LOG_TYPE.ERROR);
                WWW www = new WWW(dbFilePath);
                var downloaded = www.bytesDownloaded;
                while (!www.isDone) ;
                KojeomLogger.DebugLog(string.Format("downloaded bytes cnt : {0}", www.bytes.Length), LOG_TYPE.DATABASE);
                dbFilePath = string.Format("{0}{1}", Application.persistentDataPath, "/userDB.db");
                KojeomLogger.DebugLog(string.Format("New DB file Path : {0}", dbFilePath), LOG_TYPE.DATABASE);
                File.WriteAllBytes(dbFilePath, www.bytes);
            }
            DBConnectionPath = string.Format("URI=file:{0}", dbFilePath);
        }
        else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
        {
            DBConnectionPath = string.Format("URI=file:{0}{1}", Application.streamingAssetsPath, "/userDB.db");
            KojeomLogger.DebugLog(string.Format("Windows DB file Path : {0}", DBConnectionPath), LOG_TYPE.DATABASE);
        }
    }

    public string GetDBConnectionPath()
    {
        return DBConnectionPath;
    }
    /// <summary>
    /// 선택한 게임 캐릭터의 타입 정보를 DB에서 가져온다.
    /// </summary>
    /// <returns>Chactacter type</returns>
    public int GetSelectedCharacterType()
    {
        string chType = System.String.Empty;
        Action GetUserInfo = () =>
        {
            StringBuilder conn = new StringBuilder();
            conn.AppendFormat(DBConnectionPath, Application.dataPath);
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
        return int.Parse(chType);
    }
}
