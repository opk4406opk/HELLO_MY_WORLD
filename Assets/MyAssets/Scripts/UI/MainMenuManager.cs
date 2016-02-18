using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour {

    public void OnClickStart()
    {
        SceneManager.LoadSceneAsync("SelectCharacter");
    }

    public void OnClickLoad()
    {
        
    }
    
    public void OnClickSettings()
    {

    }

    public void OnClickExit()
    {
        Application.Quit();
    }
     
}
