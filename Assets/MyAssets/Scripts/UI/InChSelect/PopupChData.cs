using UnityEngine;
using Mono.Data.Sqlite;
using System;
/// <summary>
/// 캐릭터 선택창에서 팝업되는 세부정보를 관리하는 클래스.
/// </summary>
public class PopupChData : APopupUI
{
    [SerializeField]
    private UILabel chName;
    [SerializeField]
    private UILabel chLevel;
    [SerializeField]
    private UILabel chType;
    [SerializeField]
    private UILabel chDetailScript;


    void Start()
    {
        SetData();
        ScaleUpEffect();
    }

    public void OnClickClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    public void OnClickSelect()
    {
        Action InsertInfo = () =>
        {
            string conn = GameDBHelper.GetInstance().GetDBConnectionPath();
            using (var dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                using (var dbcmd = dbconn.CreateCommand())
                {
                    // 사용자가 선택한 캐릭터 히스토리 테이블에 저장.
                    SQL_SelectCharInfoToHistory(dbcmd);
                    //사용자가 선택한 캐릭터를 선택 테이블에 저장.
                    SQL_SelectCharToSelectedData(dbcmd);
                }
                dbconn.Close();
            }
        };
        InsertInfo();
        if (GameStatus.CurrentGameModeState == GameModeState.MULTI)
        {
            ScaleDownEffect("CallBackGoToLobby");
        }
        else
        {
            ScaleDownEffect("CallBackSingleGameProcess");
        }
    }

    private void CallBackGoToLobby()
    {
        GameSceneLoader.LoadGameSceneAsync(GameSceneLoader.SCENE_TYPE.MULTIPLAY_GAME_LOBBY);
    }

    /// <summary>
    /// ScaleDown 애니메이션이 종료된 후, 호출되어지는 singleGame Process.
    /// </summary>
    private void CallBackSingleGameProcess()
    {
        GameLocalDataManager.GetInstance().CharacterName = ChSelectManager.singleton.GetSelectCharData().chName;
        GameLocalDataManager.GetInstance().CharacterType = int.Parse(ChSelectManager.singleton.GetSelectCharData().chType);
    }

    private void SQL_SelectCharInfoToHistory(SqliteCommand dbcmd)
    {
        try
        {
            string sqlQuery = string.Format("INSERT INTO USER_SELECTED_CHAR_HISTORY(name, level, type) VALUES('{0}','{1}','{2}'",
                    chName.text, chLevel.text, chType.text);
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
        }
        catch(SqliteException e)
        {
            KojeomLogger.DebugLog(e.ToString(), LOG_TYPE.ERROR);
        }
    }

    private void SQL_SelectCharToSelectedData(SqliteCommand dbcmd)
    {
        try
        {
            string sqlQuery = "SELECT * FROM USER_SELECT_CHARACTER";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
            var reader = dbcmd.ExecuteReader();
            // SELECT 질의 후 데이터가 1개라도 있다면 UPDATE로 모든 데이터를 갱신한다.
            // read()는 row가 1개이상이면 true, 아니면 false.
            if (reader.Read())
            {
                reader.Close();
                sqlQuery = string.Format("UPDATE USER_SELECT_CHARACTER SET name ='{0}', level ='{1}', type = '{2}'",
                    chName.text, chLevel.text, chType.text);
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
            }
            else
            {
                reader.Close();
                sqlQuery = string.Format("INSERT INTO USER_SELECT_CHARACTER(name, level, type) VALUES('{0}','{1}','{2}'",
                    chName.text, chLevel.text, chType.text);
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
            }
        }
        catch (SqliteException e)
        {
            KojeomLogger.DebugLog(e.ToString(), LOG_TYPE.ERROR);
        }
    }


    private void SetData()
    {
        var selectCharData = ChSelectManager.singleton.GetSelectCharData();
        chName.text = selectCharData.chName;
        chLevel.text = selectCharData.chLevel.ToString();
        chType.text = selectCharData.chType;
        chDetailScript.text = selectCharData.detailScript;
    }

    protected override void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.charInfo);
    }
}
