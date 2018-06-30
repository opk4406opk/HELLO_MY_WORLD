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
public struct USER_ITEM
{
    public string id;
    public string name;
    public int type;
    public int amount;
}

public class GameDBHelper
{
    private static GameDBHelper instance;
    public static GameDBHelper GetInstance()
    {
        if (instance == null) instance = new GameDBHelper();
        return instance;
    }
    /// <summary>
    /// 게임 DB파일이 있는 path.
    /// https://docs.unity3d.com/ScriptReference/Application-streamingAssetsPath.html
    /// </summary>
    private string dbFilePath;
    private GameDBHelper()
    {
        InitProcess();
    }

    private void InitProcess()
    {
        var platform = Application.platform;
        if (platform == RuntimePlatform.Android)
        {
            dbFilePath = string.Format("URI=file:{0}{1}", Application.streamingAssetsPath, "/GameUserDB/userDB.db");
            if (dbFilePath.Contains("://") == true)
            {
                WWW www = new WWW(dbFilePath);
                var downloaded = www.bytesDownloaded;
                while (!www.isDone);
                // 영구데이터 path로 dbfile을 재설정한다.
                dbFilePath = string.Format("{0}{1}", Application.persistentDataPath, "/userDB.db");
                File.WriteAllBytes(dbFilePath, www.bytes);
            }
        }
        else if (platform == RuntimePlatform.WindowsEditor || platform == RuntimePlatform.WindowsPlayer)
        {
            dbFilePath = string.Format("URI=file:{0}{1}", Application.streamingAssetsPath, "/GameUserDB/userDB.db");
        }
    }

    public string GetGameDBPath()
    {
        return dbFilePath;
    }
    /// <summary>
    /// 선택한 게임 캐릭터의 타입 정보를 DB에서 가져온다.
    /// </summary>
    /// <returns>Chactacter type</returns>
    public int GetSelectCharType()
    {
        string chType = System.String.Empty;
        Action GetUserInfo = () =>
        {
            StringBuilder conn = new StringBuilder();
            conn.AppendFormat(dbFilePath, Application.dataPath);
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
