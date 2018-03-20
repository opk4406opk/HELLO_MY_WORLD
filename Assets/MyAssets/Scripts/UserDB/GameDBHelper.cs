using UnityEngine;
using System.Collections;
using System;
using System.Text;
using System.Data;
using Mono.Data.Sqlite;

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

public class GameDBHelper : MonoBehaviour
{
    /// <summary>
    /// 선택한 게임 캐릭터의 타입 정보를 DB에서 가져온다.
    /// </summary>
    /// <returns>Chactacter type</returns>
    public static int GetSelectCharType()
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
        return int.Parse(chType);
    }
}
