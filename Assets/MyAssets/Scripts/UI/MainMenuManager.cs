using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MainMenuManager : MonoBehaviour {

    public void OnClickStart()
    {
        GameStatus.isLoadGame = false;
        SceneManager.LoadSceneAsync("SelectCharacter");
    }

    public void OnClickLoad()
    {
        GameStatus.isLoadGame = true;
        SceneManager.LoadSceneAsync("GameLoading");   
    }
    
    public void OnClickSettings()
    {
        // to do
    }

    public void OnClickExit()
    {
        Application.Quit();
    }
     
}
