﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public enum UI_POPUP_TYPE
{
    Inventory = 0,
    CraftItem,
    Shop,
    //
    GameMenu,
    GameMessage,
    LoadMapGameMessage,
    //
    ItemData,
    PurchaseItem,
    SellItem,
    CharInfo,
    // network.
    WaitingConnect,
    PromptServerIP
}

/// <summary>
/// InGame Scene에서 UIPopup 창을 관리하는 클래스.
/// </summary>
public class UIPopupSupervisor : MonoBehaviour {

    // in-game UI popups.
    public static bool bInvenOpen { get; private set; } = false;
    public static bool bGameMenuOpen { get; private set; } = false;
    public static bool bGameMessageOpen { get; private set; } = false;
    public static bool bLoadGameMessageOpen { get; private set; } = false;
    public static bool bItemDataOpen { get; private set; } = false;
    public static bool bCraftItemOpen { get; private set; } = false;
    public static bool bShopOpen { get; private set; } = false;
    public static bool bPurchaseItemOpen { get; private set; } = false;
    public static bool bSellItemOpen { get; private set; } = false;
    // network UI popups.
    public static bool bWaitingConnectOpen { get; private set; } = false;
    public static bool bPromptServerIPOpen { get; private set; } = false;
    public static int CurrentPopupCount { get; private set; } = 0;

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
               (bSellItemOpen == false) &&
               (bWaitingConnectOpen == false) &&
               (bPromptServerIPOpen == false) &&
               (bLoadGameMessageOpen == false))
            {
                _bAllPopupClose = true;
                return _bAllPopupClose;
            }
            else
                return _bAllPopupClose;
        }
    }

    public static void OpenPopupUI(UI_POPUP_TYPE popuptype)
    {
        bool bOpenPopup = false;
        switch (popuptype)
        {
            case UI_POPUP_TYPE.CraftItem:
                if (bCraftItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_craftItem", LoadSceneMode.Additive);
                    bCraftItemOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.Inventory:
                if (bInvenOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_inventory", LoadSceneMode.Additive);
                    bInvenOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.Shop:
                if (bShopOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_shop", LoadSceneMode.Additive);
                    bShopOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.GameMenu:
                if (bGameMenuOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_menu", LoadSceneMode.Additive);
                    bGameMenuOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.GameMessage:
                if (bGameMessageOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_message", LoadSceneMode.Additive);
                    bGameMessageOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.LoadMapGameMessage:
                if (bLoadGameMessageOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_map_load_message", LoadSceneMode.Additive);
                    bLoadGameMessageOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.ItemData:
                if (bItemDataOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_ItemData", LoadSceneMode.Additive);
                    bItemDataOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.PurchaseItem:
                if (bPurchaseItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_purChaseItem", LoadSceneMode.Additive);
                    bPurchaseItemOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.SellItem:
                if (bSellItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_sellItem", LoadSceneMode.Additive);
                    bSellItemOpen = true;
                    _bAllPopupClose = false;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.CharInfo:
                SceneManager.LoadSceneAsync("popup_chInfo", LoadSceneMode.Additive);
                bOpenPopup = true;
                break;

            // Network UI Popups.
            case UI_POPUP_TYPE.WaitingConnect:
                if(bWaitingConnectOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_waitingConnect", LoadSceneMode.Additive);
                    bWaitingConnectOpen = true;
                    bOpenPopup = true;
                }
                break;
            case UI_POPUP_TYPE.PromptServerIP:
                if (bPromptServerIPOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_prompt_server_ip", LoadSceneMode.Additive);
                    bPromptServerIPOpen = true;
                    bOpenPopup = true;
                }
                break;
        }

        if (bOpenPopup == true)
        {
            Cursor.lockState = CursorLockMode.Confined;
            CurrentPopupCount++;
        }
    }

    public static void ClosePopupUI(UI_POPUP_TYPE popuptype)
    {
        bool bClosePopup = false;
        switch (popuptype)
        {
            case UI_POPUP_TYPE.CraftItem:
                SceneManager.UnloadSceneAsync("popup_craftItem");
                bCraftItemOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.Inventory:
                SceneManager.UnloadSceneAsync("popup_inventory");
                bInvenOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.Shop:
                SceneManager.UnloadSceneAsync("popup_shop");
                bShopOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.GameMenu:
                SceneManager.UnloadSceneAsync("popup_menu");
                bGameMenuOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.GameMessage:
                SceneManager.UnloadSceneAsync("popup_message");
                bGameMessageOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.LoadMapGameMessage:
                SceneManager.UnloadSceneAsync("popup_map_load_message");
                bLoadGameMessageOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.ItemData:
                SceneManager.UnloadSceneAsync("popup_ItemData");
                bItemDataOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.PurchaseItem:
                SceneManager.UnloadSceneAsync("popup_purChaseItem");
                bPurchaseItemOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.SellItem:
                SceneManager.UnloadSceneAsync("popup_sellItem");
                bSellItemOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.CharInfo:
                SceneManager.UnloadSceneAsync("popup_chInfo");
                bClosePopup = true;
                break;

            // Network UI Popups.
            case UI_POPUP_TYPE.WaitingConnect:
                SceneManager.UnloadSceneAsync("popup_waitingConnect");
                bWaitingConnectOpen = false;
                bClosePopup = true;
                break;
            case UI_POPUP_TYPE.PromptServerIP:
                SceneManager.UnloadSceneAsync("popup_prompt_server_ip");
                bPromptServerIPOpen = false;
                bClosePopup = true;
                break;
        }
        //
        if (bClosePopup == true) CurrentPopupCount--;
        if(bInGameAllPopupClose == true) Cursor.lockState = CursorLockMode.Locked;
    }
}
