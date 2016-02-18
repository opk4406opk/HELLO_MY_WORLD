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
        saveAndLoadManager.Save();
    }

    public void OnClickLoad()
    {
        saveAndLoadManager.Load();
    }

    public void OnClickClose()
    {
        ScaleDownEffect("CallBackPopupClose");
    }

    private void CallBackPopupClose()
    {
        SceneManager.UnloadScene("popup_menu");
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
