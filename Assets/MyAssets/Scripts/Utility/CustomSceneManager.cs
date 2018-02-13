using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// 유니티 API SceneManagement 클래스를 레핑한 클래스.
/// </summary>
public class CustomSceneManager : MonoBehaviour
{
    public enum SCENE_TYPE
    {
        //
        MAIN_MENU = 0,
        MULTIPLAY_GAME_LOBBY = 1,
        SELECT_CHARACTERS = 2,
        IN_GAME = 3,
        GAME_LOADING = 4,
        //
        UI_POPUP_SHOP = 5,
        UI_POPUP_SELL_ITEM = 6,
        UI_POPUP_PURCHASE_ITEM = 7,
        UI_POPUP_MSESSAGE = 8,
        UI_POPUP_MENU = 9,
        UI_POPUP_ITEM_DATA = 10,
        UI_POPUP_INVENTORY = 11,
        UI_POPUP_CRAFTITEM = 12,
        UI_POPUP_CHAR_INFO = 13
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
            case SCENE_TYPE.UI_POPUP_SHOP:
                SceneManager.LoadSceneAsync("popup_shop", loadMode);
                break;
            case SCENE_TYPE.UI_POPUP_SELL_ITEM:
                SceneManager.LoadSceneAsync("popup_sellItem", loadMode);
                break;
            case SCENE_TYPE.UI_POPUP_MSESSAGE:
                SceneManager.LoadSceneAsync("popup_message", loadMode);
                break;
            case SCENE_TYPE.UI_POPUP_MENU:
                SceneManager.LoadSceneAsync("popup_menu", loadMode);
                break;
            case SCENE_TYPE.UI_POPUP_ITEM_DATA:
                SceneManager.LoadSceneAsync("popup_ItemData", loadMode);
                break;
            case SCENE_TYPE.UI_POPUP_INVENTORY:
                SceneManager.LoadSceneAsync("popup_inventory", loadMode);
                break;
            case SCENE_TYPE.UI_POPUP_CRAFTITEM:
                SceneManager.LoadSceneAsync("popup_craftItem", loadMode);
                break;
            case SCENE_TYPE.UI_POPUP_CHAR_INFO:
                SceneManager.LoadSceneAsync("popup_chInfo", loadMode);
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
            case SCENE_TYPE.UI_POPUP_SHOP:
                SceneManager.UnloadSceneAsync("popup_shop");
                break;
            case SCENE_TYPE.UI_POPUP_SELL_ITEM:
                SceneManager.UnloadSceneAsync("popup_sellItem");
                break;
            case SCENE_TYPE.UI_POPUP_MSESSAGE:
                SceneManager.UnloadSceneAsync("popup_message");
                break;
            case SCENE_TYPE.UI_POPUP_MENU:
                SceneManager.UnloadSceneAsync("popup_menu");
                break;
            case SCENE_TYPE.UI_POPUP_ITEM_DATA:
                SceneManager.UnloadSceneAsync("popup_ItemData");
                break;
            case SCENE_TYPE.UI_POPUP_INVENTORY:
                SceneManager.UnloadSceneAsync("popup_inventory");
                break;
            case SCENE_TYPE.UI_POPUP_CRAFTITEM:
                SceneManager.UnloadSceneAsync("popup_craftItem");
                break;
            case SCENE_TYPE.UI_POPUP_CHAR_INFO:
                SceneManager.UnloadSceneAsync("popup_chInfo");
                break;
            default:
                break;
        }
    }
}
