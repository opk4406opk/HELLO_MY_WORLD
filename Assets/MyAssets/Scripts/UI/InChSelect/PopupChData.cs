using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;
using System.Data;
using System;

/// <summary>
/// 캐릭터 선택창에서 팝업되는 세부정보를 관리하는 클래스.
/// </summary>
public class PopupChData : MonoBehaviour
{
    [SerializeField]
    private UILabel chName;
    [SerializeField]
    private UILabel chLevel;
    [SerializeField]
    private UILabel chType;
    [SerializeField]
    private UILabel chDetailScript;
    [SerializeField]
    private GameObject popupObj;

    void Start()
    {
        SetData();
        ScaleUpEffect();
    }

    private void ScaleUpEffect()
    {
        popupObj.transform.localScale = new Vector3(0, 0, 0);
        Vector3 scaleUp = new Vector3(1, 1, 1);
        iTween.ScaleTo(popupObj, iTween.Hash("scale", scaleUp,
            "name", "scaleUp",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none));
    }
    private void ScaleDownEffect(string _callBack)
    {
        popupObj.transform.localScale = new Vector3(1, 1, 1);
        Vector3 scaleDown = new Vector3(0, 0, 0);
        iTween.ScaleTo(popupObj, iTween.Hash("scale", scaleDown,
            "name", "scaleDown",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none,
            "oncomplete", _callBack,
            "oncompletetarget", gameObject));
    }

    public void OnClickClose()
    {
        PopupExitProcess();
    }

    public void OnClickSelect()
    {
        Action InsertInfo = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/StreamingAssets/GameUserDB/userDB.db";
            using (IDbConnection dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                using (IDbCommand dbcmd = dbconn.CreateCommand())
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
        if (GameStatus.isMultiPlay)
        {
            MultiPlayLobbyProcess();
        }
        else
        {
            SingleGameProcess();
        }
    }

    private void SQL_SelectCharInfoToHistory(IDbCommand dbcmd)
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

    private void SQL_SelectCharToSelectedData(IDbCommand dbcmd)
    {
        try
        {
            string sqlQuery = "SELECT * FROM USER_SELECT_CHARACTER";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();
            IDataReader reader = dbcmd.ExecuteReader();
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

    private void MultiPlayLobbyProcess()
    {
        ScaleDownEffect("CallBackGoToLobby");
    }
    private void CallBackGoToLobby()
    {
        GameSceneLoader.LoadGameSceneAsync(GameSceneLoader.SCENE_TYPE.MULTIPLAY_GAME_LOBBY);
    }

    private void SingleGameProcess()
    {
        ScaleDownEffect("CallBackSingleGameProcess");
    }
    /// <summary>
    /// ScaleDown 애니메이션이 종료된 후, 호출되어지는 singleGame Process.
    /// </summary>
    private void CallBackSingleGameProcess()
    {
        var netClient = GameNetworkManager.GetInstance().StartHost();
        GameNetworkManager.GetInstance().isHost = true;
        GameNetworkManager.GetInstance().Init();
    }

    private void PopupExitProcess()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    /// <summary>
    /// ScaleDown 애니메이션이 종료된 후, 호출되어지는 팝업창 종료 메소드.
    /// </summary>
    private void CallBackPopupClose()
    {
        UIPopupSupervisor.ClosePopupUI(POPUP_TYPE.charInfo);
    }

    private void SetData()
    {
        var selectCharData = ChSelectManager.singleton.GetSelectCharData();
        chName.text = selectCharData.chName;
        chLevel.text = selectCharData.chLevel.ToString();
        chType.text = selectCharData.chType;
        chDetailScript.text = selectCharData.detailScript;
    }
}
