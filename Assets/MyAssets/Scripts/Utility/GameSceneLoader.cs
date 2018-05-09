using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 유니티 API SceneManagement 클래스를 레핑한 클래스.
/// </summary>
public class GameSceneLoader
{
    public enum SCENE_TYPE
    {
        //
        MAIN_MENU = 0,
        MULTIPLAY_GAME_LOBBY = 1,
        SELECT_CHARACTERS = 2,
        IN_GAME = 3,
        GAME_LOADING = 4,
    }
    public static void LoadGameSceneAsync(SCENE_TYPE sceneType, LoadSceneMode loadMode = LoadSceneMode.Single)
    {
        switch (sceneType)
        {
            case SCENE_TYPE.MAIN_MENU:
                SceneManager.LoadSceneAsync("MainMenu", loadMode);
                break;
            case SCENE_TYPE.MULTIPLAY_GAME_LOBBY:
                SceneManager.LoadSceneAsync("MultiPlayGameLobby", loadMode);
                break;
            case SCENE_TYPE.SELECT_CHARACTERS:
                SceneManager.LoadSceneAsync("SelectCharacter", loadMode);
                break;
            case SCENE_TYPE.IN_GAME:
                SceneManager.LoadSceneAsync("InGame", loadMode);
                break;
            case SCENE_TYPE.GAME_LOADING:
                SceneManager.LoadSceneAsync("GameLoading", loadMode);
                break;
            default:
                break;
        }
    }

    public static void UnLoadGameSceneAsync(SCENE_TYPE sceneType)
    {
        switch (sceneType)
        {
            case SCENE_TYPE.MAIN_MENU:
                SceneManager.UnloadSceneAsync("MainMenu");
                break;
            case SCENE_TYPE.MULTIPLAY_GAME_LOBBY:
                SceneManager.UnloadSceneAsync("MultiPlayGameLobby");
                break;
            case SCENE_TYPE.SELECT_CHARACTERS:
                SceneManager.UnloadSceneAsync("SelectCharacter");
                break;
            case SCENE_TYPE.IN_GAME:
                SceneManager.UnloadSceneAsync("InGame");
                break;
            case SCENE_TYPE.GAME_LOADING:
                SceneManager.UnloadSceneAsync("GameLoading");
                break;
            default:
                break;
        }
    }
}
