using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum POPUP_TYPE
{
    inven = 0,
    craftItem = 1,
    shop = 2,
    //
    gameMenu = 3,
    gameMessage = 4,
    //
    itemData = 5,
    purchaseItem = 6,
    sellItem = 7,
    charInfo = 8,
    // network.
    waitingConnect = 9,
    promptServerIP = 10
}

/// <summary>
/// InGame Scene에서 UIPopup 창을 관리하는 클래스.
/// </summary>
public class UIPopupSupervisor : MonoBehaviour {

    // in-game UI popups.
    public static bool isInvenOpen { get; private set; } = false;
    public static bool isGameMenuOpen { get; private set; } = false;
    public static bool isGameMessageOpen { get; private set; } = false;
    public static bool isItemDataOpen { get; private set; } = false;
    public static bool isCraftItemOpen { get; private set; } = false;
    public static bool isShopOpen { get; private set; } = false;
    public static bool isPurchaseItemOpen { get; private set; } = false;
    public static bool isSellItemOpen { get; private set; } = false;
    // network UI popups.
    public static bool isWaitingConnectOpen { get; private set; } = false;
    public static bool isPromptServerIPOpen { get; private set; } = false;

    //
    private static bool _isAllPopupClose = false;
    public static bool isInGameAllPopupClose
    {
        get
        {
            if ((isInvenOpen == false) &&
               (isGameMenuOpen == false) &&
               (isGameMessageOpen == false) &&
               (isItemDataOpen == false) &&
               (isCraftItemOpen == false) &&
               (isShopOpen == false) &&
               (isPurchaseItemOpen == false) &&
               (isSellItemOpen == false))
            {
                _isAllPopupClose = true;
                return _isAllPopupClose;
            }
            else
                return _isAllPopupClose;
        }
    }

    public static void OpenPopupUI(POPUP_TYPE popuptype)
    {
        switch (popuptype)
        {
            case POPUP_TYPE.craftItem:
                if (isCraftItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_craftItem", LoadSceneMode.Additive);
                    isCraftItemOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.inven:
                if (isInvenOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_inventory", LoadSceneMode.Additive);
                    isInvenOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.shop:
                if (isShopOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_shop", LoadSceneMode.Additive);
                    isShopOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.gameMenu:
                if (isGameMenuOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_menu", LoadSceneMode.Additive);
                    isGameMenuOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.gameMessage:
                if (isGameMessageOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_message", LoadSceneMode.Additive);
                    isGameMessageOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.itemData:
                if (isItemDataOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_ItemData", LoadSceneMode.Additive);
                    isItemDataOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.purchaseItem:
                if (isPurchaseItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_purChaseItem", LoadSceneMode.Additive);
                    isPurchaseItemOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.sellItem:
                if (isSellItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_sellItem", LoadSceneMode.Additive);
                    isSellItemOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.charInfo:
                SceneManager.LoadSceneAsync("popup_chInfo", LoadSceneMode.Additive);
                break;

            // Network UI Popups.
            case POPUP_TYPE.waitingConnect:
                if(isWaitingConnectOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_waitingConnect", LoadSceneMode.Additive);
                    isWaitingConnectOpen = true;
                }
                break;
            case POPUP_TYPE.promptServerIP:
                if (isPromptServerIPOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_prompt_server_ip", LoadSceneMode.Additive);
                    isPromptServerIPOpen = true;
                }
                break;
        }
    }

    public static void ClosePopupUI(POPUP_TYPE popuptype)
    {
        switch (popuptype)
        {
            case POPUP_TYPE.craftItem:
                SceneManager.UnloadSceneAsync("popup_craftItem");
                isCraftItemOpen = false;
                break;
            case POPUP_TYPE.inven:
                SceneManager.UnloadSceneAsync("popup_inventory");
                isInvenOpen = false;
                break;
            case POPUP_TYPE.shop:
                SceneManager.UnloadSceneAsync("popup_shop");
                isShopOpen = false;
                break;
            case POPUP_TYPE.gameMenu:
                SceneManager.UnloadSceneAsync("popup_menu");
                isGameMenuOpen = false;
                break;
            case POPUP_TYPE.gameMessage:
                SceneManager.UnloadSceneAsync("popup_message");
                isGameMessageOpen = false;
                break;
            case POPUP_TYPE.itemData:
                SceneManager.UnloadSceneAsync("popup_ItemData");
                isItemDataOpen = false;
                break;
            case POPUP_TYPE.purchaseItem:
                SceneManager.UnloadSceneAsync("popup_purChaseItem");
                isPurchaseItemOpen = false;
                break;
            case POPUP_TYPE.sellItem:
                SceneManager.UnloadSceneAsync("popup_sellItem");
                isSellItemOpen = false;
                break;
            case POPUP_TYPE.charInfo:
                SceneManager.UnloadSceneAsync("popup_chInfo");
                break;

            // Network UI Popups.
            case POPUP_TYPE.waitingConnect:
                SceneManager.UnloadSceneAsync("popup_waitingConnect");
                isWaitingConnectOpen = false;
                break;
            case POPUP_TYPE.promptServerIP:
                SceneManager.UnloadSceneAsync("popup_prompt_server_ip");
                isPromptServerIPOpen = false;
                break;
        }
    }
}
