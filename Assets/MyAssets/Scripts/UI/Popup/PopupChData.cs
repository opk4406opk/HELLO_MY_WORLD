using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        SetData();
    }

    public void ClickExit()
    {
        SceneManager.UnloadScene("popup_chInfo");
    }

    private void SetData()
    {
        GameObject sceneToSceneData = GameObject.Find("SceneToScene_datas");
        string data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameDatas.TryGetValue("chName", out data);
        chName.text = data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameDatas.TryGetValue("chLevel", out data);
        chLevel.text = data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameDatas.TryGetValue("chType", out data);
        chType.text = data;
        sceneToSceneData.GetComponent<SceneToScene_Data>().gameDatas.TryGetValue("detailScript", out data);
        chDetailScript.text = data;

    }
}
