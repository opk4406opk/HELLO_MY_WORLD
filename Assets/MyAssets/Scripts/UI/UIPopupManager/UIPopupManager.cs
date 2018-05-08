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
    sellItem = 7
}

/// <summary>
/// InGame Scene에서 UIPopup 창을 관리하는 클래스.
/// </summary>
public class UIPopupManager : MonoBehaviour {

    private static bool _isInvenOpen = false;
    public static bool isInvenOpen
    {
        get { return _isInvenOpen; }
    }
    private static bool _isGameMenuOpen = false;
    public static bool isGameMenuOpen
    {
        get { return _isGameMenuOpen; }
    }

    private static bool _isGameMessageOpen = false;
    public static bool isGameMessageOpen
    {
        get { return _isGameMenuOpen; }
    }

    private static bool _isItemDataOpen = false;
    public static bool isItemDataOpen
    {
        get { return _isItemDataOpen; }
    }

    private static bool _isCraftItemOpen = false;
    public static bool isCraftItemOpen
    {
        get { return _isCraftItemOpen; }
    }

    private static bool _isShopOpen = false;
    public static bool isShopOpen
    {
        get { return _isShopOpen; }
    }

    private static bool _isPurchaseItemOpen = false;
    public static bool isPurchaseItemOpen
    {
        get { return _isPurchaseItemOpen; }
    }

    private static bool _isSellItemOpen = false;
    public static bool isSellItemOpen
    {
        get { return _isSellItemOpen; }
    }

    private static bool _isAllPopupClose = false;
    public static bool isAllpopupClose
    {
        get
        {
            if ((_isInvenOpen == false) &&
               (_isGameMenuOpen == false) &&
               (_isGameMessageOpen == false) &&
               (_isItemDataOpen == false) &&
               (_isCraftItemOpen == false) &&
               (_isShopOpen == false) &&
               (_isPurchaseItemOpen == false) &&
               (_isSellItemOpen == false))
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
                if (_isCraftItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_craftItem", LoadSceneMode.Additive);
                    _isCraftItemOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.inven:
                if (_isInvenOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_inventory", LoadSceneMode.Additive);
                    _isInvenOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.shop:
                if (_isShopOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_shop", LoadSceneMode.Additive);
                    _isShopOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.gameMenu:
                if (_isGameMenuOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_menu", LoadSceneMode.Additive);
                    _isGameMenuOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.gameMessage:
                if (_isGameMessageOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_message", LoadSceneMode.Additive);
                    _isGameMessageOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.itemData:
                if (_isItemDataOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_ItemData", LoadSceneMode.Additive);
                    _isItemDataOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.purchaseItem:
                if (_isPurchaseItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_purChaseItem", LoadSceneMode.Additive);
                    _isPurchaseItemOpen = true;
                    _isAllPopupClose = false;
                }
                break;
            case POPUP_TYPE.sellItem:
                if (_isSellItemOpen == false)
                {
                    SceneManager.LoadSceneAsync("popup_sellItem", LoadSceneMode.Additive);
                    _isSellItemOpen = true;
                    _isAllPopupClose = false;
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
                _isCraftItemOpen = false;
                break;
            case POPUP_TYPE.inven:
                SceneManager.UnloadSceneAsync("popup_inventory");
                _isInvenOpen = false;
                break;
            case POPUP_TYPE.shop:
                SceneManager.UnloadSceneAsync("popup_shop");
                _isShopOpen = false;
                break;
            case POPUP_TYPE.gameMenu:
                SceneManager.UnloadSceneAsync("popup_menu");
                _isGameMenuOpen = false;
                break;
            case POPUP_TYPE.gameMessage:
                SceneManager.UnloadSceneAsync("popup_message");
                _isGameMessageOpen = false;
                break;
            case POPUP_TYPE.itemData:
                SceneManager.UnloadSceneAsync("popup_ItemData");
                _isItemDataOpen = false;
                break;
            case POPUP_TYPE.purchaseItem:
                SceneManager.UnloadSceneAsync("popup_purChaseItem");
                _isPurchaseItemOpen = false;
                break;
            case POPUP_TYPE.sellItem:
                SceneManager.UnloadSceneAsync("popup_sellItem");
                _isSellItemOpen = false;
                break;
        }
    }
}
