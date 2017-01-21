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

    private delegate void del_InsertNewUserInfo();
    public void ClickGameStart()
    {
        del_InsertNewUserInfo InsertInfo = () =>
        {
            string conn = "URI=file:" + Application.dataPath +
               "/StreamingAssets/GameUserDB/userDB.db";
            using (IDbConnection dbconn = new SqliteConnection(conn))
            {
                dbconn.Open(); //Open connection to the database.
                using (IDbCommand dbcmd = dbconn.CreateCommand())
                {
                    try
                    {
                        string sqlQuery = "INSERT INTO USER_INFO(name, level, type) " +
                                          "VALUES(" + "'" + chName.text + "'" + ", " +
                                          "'" + chLevel.text + "'" + ", " +
                                          "'" + chType.text + "'" + ")";
                        dbcmd.CommandText = sqlQuery;
                        dbcmd.ExecuteNonQuery();
                    }
                    catch(SqliteException e)
                    {
                        // 이미 등록된 캐릭터이다.
                        Debug.Log(e.Message);
                    }
                }
                dbconn.Close();
            }
        };
        InsertInfo();
        GameLoadingProcess();
    }

    private void GameLoadingProcess()
    {
        ScaleDownEffect("CallBackGameLoading");
    }
    /// <summary>
    /// ScaleDown 애니메이션이 종료된 후, 호출되어지는 InGame 로딩 메소드.
    /// </summary>
    private void CallBackGameLoading()
    {
        SceneManager.LoadSceneAsync("GameLoading");
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
        SceneManager.UnloadScene("popup_chInfo");
    }

    private void SetData()
    {
        GameObject sceneToSceneData = GameObject.Find("SceneToScene_datas");
        string data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameChDatas.TryGetValue("chName", out data);
        chName.text = data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameChDatas.TryGetValue("chLevel", out data);
        chLevel.text = data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameChDatas.TryGetValue("chType", out data);
        chType.text = data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameChDatas.TryGetValue("detailScript", out data);
        chDetailScript.text = data;
    }
}
