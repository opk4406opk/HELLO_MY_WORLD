using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


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
               (_isShopOpen == false))
            {
                _isAllPopupClose = true;
                return _isAllPopupClose;
            }
            else
                return _isAllPopupClose;
        }
    }
   

	public static void OpenInven()
    {
        if(_isInvenOpen == false)
        {
            SceneManager.LoadSceneAsync("popup_inventory", LoadSceneMode.Additive);
            _isInvenOpen = true;
            _isAllPopupClose = false;
        }
    }

    public static void CloseInven()
    {
        SceneManager.UnloadSceneAsync("popup_inventory");
        _isInvenOpen = false;
    }

    public static void OpenInGameMenu()
    {
        if (_isGameMenuOpen == false)
        {
            SceneManager.LoadSceneAsync("popup_menu", LoadSceneMode.Additive);
            _isGameMenuOpen = true;
            _isAllPopupClose = false;
        }
    }

    public static void CloseInGameMenu()
    {
        SceneManager.UnloadSceneAsync("popup_menu");
        _isGameMenuOpen = false;
    }

    public static void OpenGameMessage()
    {
        if(_isGameMessageOpen == false)
        {
            SceneManager.LoadSceneAsync("popup_message", LoadSceneMode.Additive);
            _isGameMessageOpen = true;
            _isAllPopupClose = false;
        }
    }

    public static void CloseGameMessage()
    {
        SceneManager.UnloadSceneAsync("popup_message");
        _isGameMessageOpen = false;
    }

    public static void OpenItemData()
    {
        if(_isItemDataOpen == false)
        {
            SceneManager.LoadSceneAsync("popup_ItemData", LoadSceneMode.Additive);
            _isItemDataOpen = true;
            _isAllPopupClose = false;
        }
    }

    public static void CloseItemData()
    {
        SceneManager.UnloadSceneAsync("popup_ItemData");
        _isItemDataOpen = false;
    }

    public static void OpenCraftItem()
    {
        if(_isCraftItemOpen == false)
        {
            SceneManager.LoadSceneAsync("popup_craftItem", LoadSceneMode.Additive);
            _isCraftItemOpen = true;
            _isAllPopupClose = false;
        }
    }

    public static void CloseCraftItem()
    {
        SceneManager.UnloadSceneAsync("popup_craftItem");
        _isCraftItemOpen = false;
    }

    public static void OpenShop()
    {
        if (_isShopOpen == false)
        {
            SceneManager.LoadSceneAsync("popup_shop", LoadSceneMode.Additive);
            _isShopOpen = true;
            _isAllPopupClose = false;
        }
    }

    public static void CloseShop()
    {
        SceneManager.UnloadSceneAsync("popup_shop");
        _isShopOpen = false;
    }
}
