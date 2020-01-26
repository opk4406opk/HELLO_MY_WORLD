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
        MainMenu = 0,
        MultiPlayGameLobby = 1,
        SelectCharacter = 2,
        InGame = 3,
        GameLoading = 4,
    }
    public static void LoadGameSceneAsync(SCENE_TYPE sceneType, LoadSceneMode loadMode = LoadSceneMode.Single)
    {
        AsyncOperation operation = null;
        switch (sceneType)
        {
            case SCENE_TYPE.MainMenu:
                operation = SceneManager.LoadSceneAsync("MainMenu", loadMode);
                break;
            case SCENE_TYPE.MultiPlayGameLobby:
                operation = SceneManager.LoadSceneAsync("MultiPlayGameLobby", loadMode);
                break;
            case SCENE_TYPE.SelectCharacter:
                operation = SceneManager.LoadSceneAsync("SelectCharacter", loadMode);
                break;
            case SCENE_TYPE.InGame:
                operation = SceneManager.LoadSceneAsync("InGame", loadMode);
                break;
            case SCENE_TYPE.GameLoading:
                operation = SceneManager.LoadSceneAsync("GameLoading", loadMode);
                break;
        }
        if(operation != null)
        {
            operation.completed += (op) => 
            {
                if(op.isDone == true) KojeomLogger.DebugLog(string.Format("Loading Completed! SceneType : {0}, LoadMode : {1}",
                    KojeomUtility.EnumToString<SCENE_TYPE>(sceneType), KojeomUtility.EnumToString<LoadSceneMode>(loadMode)));
            };
        }
    }

    public static void UnLoadGameSceneAsync(SCENE_TYPE sceneType)
    {
        AsyncOperation operation = null;
        switch (sceneType)
        {
            case SCENE_TYPE.MainMenu:
                SceneManager.UnloadSceneAsync("MainMenu");
                break;
            case SCENE_TYPE.MultiPlayGameLobby:
                SceneManager.UnloadSceneAsync("MultiPlayGameLobby");
                break;
            case SCENE_TYPE.SelectCharacter:
                SceneManager.UnloadSceneAsync("SelectCharacter");
                break;
            case SCENE_TYPE.InGame:
                SceneManager.UnloadSceneAsync("InGame");
                break;
            case SCENE_TYPE.GameLoading:
                SceneManager.UnloadSceneAsync("GameLoading");
                break;
            default:
                break;
        }
        if (operation != null)
        {
            operation.completed += (op) =>
            {
                if (op.isDone == true) KojeomLogger.DebugLog(string.Format("UnLoading Completed! SceneType : {0}", KojeomUtility.EnumToString<SCENE_TYPE>(sceneType)));
            };
        }
    }
}
