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
    public static bool bInvenOpen { get; private set; } = false;
    public static bool bGameMenuOpen { get; private set; } = false;
    public static bool bGameMessageOpen { get; private set; } = false;
    public static bool bItemDataOpen { get; private set; } = false;
    public static bool bCraftItemOpen { get; private set; } = false;
    public static bool bShopOpen { get; private set; } = false;
    public static bool bPurchaseItemOpen { get; private set; } = false;
    public static bool bSellItemOpen { get; private set; } = false;
    // network UI popups.
    public static bool bWaitingConnectOpen { get; private set; } = false;
    public static bool bPromptServerIPOpen { get; private set; } = false;

    //
    private static bool _bAllPopupClose = false;
    public static bool bInGameAllPopupClose
    {
        get
        {
            if ((bInvenOpen == false) &&
               (bGameMenuOpen == false) &&
               (bGameMessageOpen == false) &&
               (bItemDataOpen == false) &&
               (bCraftItemOpen == false) &&
               (bShopOpen == false) &&
               (bPurchaseItemOpen == false) &&
               (bSellItemOpen == false))
            {
                _bAllPopupClose = true;
                return _bAllPopupClose;
            }
            else
                return _bAllPopupClose;
        }
    }

    public static void OpenPopupUI(POPUP_TYPE popuptype)
    {
        switch (popuptype)
        {
            case POPUP_TYPE.craftItem:
                if (bCraftItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_craftItem", LoadSceneMode.Additive);
                    bCraftItemOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.inven:
                if (bInvenOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_inventory", LoadSceneMode.Additive);
                    bInvenOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.shop:
                if (bShopOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_shop", LoadSceneMode.Additive);
                    bShopOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.gameMenu:
                if (bGameMenuOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_menu", LoadSceneMode.Additive);
                    bGameMenuOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.gameMessage:
                if (bGameMessageOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_message", LoadSceneMode.Additive);
                    bGameMessageOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.itemData:
                if (bItemDataOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_ItemData", LoadSceneMode.Additive);
                    bItemDataOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.purchaseItem:
                if (bPurchaseItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_purChaseItem", LoadSceneMode.Additive);
                    bPurchaseItemOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.sellItem:
                if (bSellItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_sellItem", LoadSceneMode.Additive);
                    bSellItemOpen = true;
                    _bAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.charInfo:
                SceneManager.LoadSceneAsync("popup_chInfo", LoadSceneMode.Additive);
                break;

            // Network UI Popups.
            case POPUP_TYPE.waitingConnect:
                if(bWaitingConnectOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_waitingConnect", LoadSceneMode.Additive);
                    bWaitingConnectOpen = true;
                }
                break;
            case POPUP_TYPE.promptServerIP:
                if (bPromptServerIPOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_prompt_server_ip", LoadSceneMode.Additive);
                    bPromptServerIPOpen = true;
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
                bCraftItemOpen = false;
                break;
            case POPUP_TYPE.inven:
                SceneManager.UnloadSceneAsync("popup_inventory");
                bInvenOpen = false;
                break;
            case POPUP_TYPE.shop:
                SceneManager.UnloadSceneAsync("popup_shop");
                bShopOpen = false;
                break;
            case POPUP_TYPE.gameMenu:
                SceneManager.UnloadSceneAsync("popup_menu");
                bGameMenuOpen = false;
                break;
            case POPUP_TYPE.gameMessage:
                SceneManager.UnloadSceneAsync("popup_message");
                bGameMessageOpen = false;
                break;
            case POPUP_TYPE.itemData:
                SceneManager.UnloadSceneAsync("popup_ItemData");
                bItemDataOpen = false;
                break;
            case POPUP_TYPE.purchaseItem:
                SceneManager.UnloadSceneAsync("popup_purChaseItem");
                bPurchaseItemOpen = false;
                break;
            case POPUP_TYPE.sellItem:
                SceneManager.UnloadSceneAsync("popup_sellItem");
                bSellItemOpen = false;
                break;
            case POPUP_TYPE.charInfo:
                SceneManager.UnloadSceneAsync("popup_chInfo");
                break;

            // Network UI Popups.
            case POPUP_TYPE.waitingConnect:
                SceneManager.UnloadSceneAsync("popup_waitingConnect");
                bWaitingConnectOpen = false;
                break;
            case POPUP_TYPE.promptServerIP:
                SceneManager.UnloadSceneAsync("popup_prompt_server_ip");
                bPromptServerIPOpen = false;
                break;
        }
    }
}
