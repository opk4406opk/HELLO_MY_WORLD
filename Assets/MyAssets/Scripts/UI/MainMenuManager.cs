using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour {


    public void ClickStart()
    {
        SceneManager.LoadSceneAsync("SelectCharacter");
    }

    public void ClickLoad()
    {

    }
    
    public void ClickSettings()
    {

    }

    public void ClickExit()
    {
        Application.Quit();
    }
     
}
