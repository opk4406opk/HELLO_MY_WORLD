using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class InGameMenuManager : MonoBehaviour {

    private SaveAndLoadManager saveAndLoadManager;
    [SerializeField]
    private GameObject obj_menu;

    void Start()
    {
        GameObject saveAndLoad = GameObject.Find("SaveAndLoadManager");
        saveAndLoadManager = saveAndLoad.GetComponent<SaveAndLoadManager>();

        ScaleUpEffect();
    }

    public void OnClickSave()
    {
        if(saveAndLoadManager.Save())
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_SAVE_SUCCESS;
            GameMessage.SetMessage("게임 세이브에 성공했습니다.");
            UIPopupManager.OpenGameMessage();
        }
        else
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_SAVE_FAIL;
            GameMessage.SetMessage("게임 세이브에 실패했습니다.");
            UIPopupManager.OpenGameMessage();
        }
    }

    public void OnClickLoad()
    {
        if (saveAndLoadManager.Load())
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_LOAD_SUCCESS;
            GameMessage.SetMessage("게임 로드에 성공했습니다.");
            UIPopupManager.OpenGameMessage();
        }
        else
        {
            GameMessage.SetGameMsgType = GameMessage.MESSAGE_TYPE.WORLD_LOAD_FAIL;
            GameMessage.SetMessage("게임 로드에 실패했습니다.");
            UIPopupManager.OpenGameMessage();
        }
    }

    public void OnClickClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    private void CallBackPopupClose()
    {
        UIPopupManager.CloseInGameMenu();
    }

    public void OnClickExit()
    {
        Application.Quit();
    }

    private void ScaleUpEffect()
    {
        obj_menu.transform.localScale = new Vector3(0, 0, 0);
        Vector3 scaleUp = new Vector3(1, 1, 1);
        iTween.ScaleTo(obj_menu, iTween.Hash("scale", scaleUp,
            "name", "scaleUp",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none));
    }
    private void ScaleDownEffect(string _callBack)
    {
        obj_menu.transform.localScale = new Vector3(1, 1, 1);
        Vector3 scaleDown = new Vector3(0, 0, 0);
        iTween.ScaleTo(obj_menu, iTween.Hash("scale", scaleDown,
            "name", "scaleDown",
            "time", 1.0f,
            "speed", 10.0f,
            "easetype", iTween.EaseType.linear,
            "looptype", iTween.LoopType.none,
            "oncomplete", _callBack,
            "oncompletetarget", gameObject));
    }
}
