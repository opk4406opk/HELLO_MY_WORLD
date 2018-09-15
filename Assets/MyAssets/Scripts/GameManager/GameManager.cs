using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public interface IManager
{
    void Init();
    void ResetManager();
}

/// <summary>
/// 게임 상태(single, multi, load, save)를 관리하는 클래스.
/// </summary>
public class GameStatus
{
    public static bool isLoadGame = false;
    public static bool isMultiPlay = false;
    public static bool isSingleHostPlay = false;
    public static bool isInGameSceneStart
    {
        get
        {
            if(isMultiPlay == false && isSingleHostPlay == false)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

/// <summary>
/// 게임에 전반적인 관리를 하는 클래스.
/// ( 게임의 시작점 )
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Inspector variables.

    public bool isSoundOn = false;
    public bool isSubWorldDataSave = false;
    public bool isLockCursor = false;

    [SerializeField]
    private SubWorldDataFile subWorldDataFile;
    [SerializeField]
    private TileDataFile tileDataFile;
    [SerializeField]
    private ItemDataFile itemDataFile;
    [SerializeField]
    private CraftItemListDataFile craftItemListDataFile;
    [SerializeField]
    private NPCDataFile npcDataFile;
    [SerializeField]
    private GameConfigDataFile gameConfigDataFile;
    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private BeltItemSelector beltItemSelector;
    [SerializeField]
    private SaveAndLoadManager saveAndLoadManager;
    [SerializeField]
    private LootingSystem lootingSystem;
    [SerializeField]
    private WorldManager worldManager;
    [SerializeField]
    private GameSoundManager soundManager;
    [SerializeField]
    private WeatherManager weatherManager;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private KojeomCoroutineHelper kojeomCoroutineHelper;
    [SerializeField]
    private DayAndNightManager dayAndNightManager;
    //
    [SerializeField]
    private InGameUISupervisor ingameUISupervisor;

    [SerializeField]
    private NPCManager npcManager;
    [SerializeField]
    private ActorCollideManager actorCollideManager;
    #endregion

    public static GameManager instance = null;

    void Start ()
    {
        KojeomLogger.DebugLog(string.Format("Multi_game : {0}, Loaded_game : {1}, SingleHost_game : {2}",
               GameStatus.isMultiPlay, GameStatus.isLoadGame, GameStatus.isSingleHostPlay), LOG_TYPE.SYSTEM);
        instance = this;
        // single or multi play 게임이 아니라면
        // InGame Scene에서 바로 시작하는 경우 ( in editor mode )
        if (GameStatus.isInGameSceneStart)
        {
            InitInGameSceneStart();
        }
        InitSettings();
        InitDataFiles();
        StartCoroutine(WaitingLogin());
    }

    private void InitSettings()
    {
        if(isLockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// InGameScene에서 바로 시작되는 경우에 호출되는 초기화 메소드.
    /// </summary>
    private void InitInGameSceneStart()
    {
        KojeomLogger.DebugLog("InGameScene Start.", LOG_TYPE.INFO);
        //
        GameObject netMgr = Resources.Load<GameObject>(ConstFilePath.GAME_NET_MGR_PREFAB);
        Instantiate(netMgr, new Vector3(0, 0, 0), Quaternion.identity);
        //
        KojeomLogger.DebugLog("StartHost", LOG_TYPE.INFO);
        GameNetworkManager.GetNetworkManagerInstance().onlineScene = "InGame";
        var netClient = GameNetworkManager.GetNetworkManagerInstance().StartHost();
        GameNetworkManager.GetNetworkManagerInstance().isHost = true;
        GameNetworkManager.GetNetworkManagerInstance().LateInit();
        // InGame씬에서 바로 시작하는 경우에는 해당 flag를 true로 설정.
        GameNetworkStateFlags.isReceivedRandomSeedFormServer = true;
        GameNetworkStateFlags.isReceiveGameUserList = true;
        GameNetworkManager.InitGameRandomSeed(System.DateTime.Now.Second);
    }

    /// <summary>
    /// 게임에 사용되는 데이터파일들을 초기화합니다. ( 게임 매니저 초기화보다 먼저 호출되야 합니다. )
    /// </summary>
    private void InitDataFiles()
    {
        KojeomLogger.DebugLog("게임 데이터 파일 초기화 시작.");
        //GameDataFiles Init
        // 제작아이템 데이타파일은 아이템데이타 파일을 읽어들인 후에 읽어야함.
        gameConfigDataFile.Init();
        itemDataFile.Init();
        tileDataFile.Init();
        subWorldDataFile.Init();
        craftItemListDataFile.Init();
        npcDataFile.Init();
        KojeomLogger.DebugLog("게임 데이터 파일 초기화 완료.");
    }
    /// <summary>
    /// 게임내 각종 매니저 클래스들을 초기화합니다. ( 게임 데이터파일들이 초기화 된 이후에 호출되야 합니다. )
    /// </summary>
    public void InitManagers()
    {
        KojeomLogger.DebugLog("게임매니저 클래스들을 초기화 합니다.");
        // sound init.
        if(isSoundOn == true)
        {
            GameSoundManager.GetInstnace().PlaySound(GAME_SOUND_TYPE.BGM_InGame);
        }
        //
        dayAndNightManager.Init();
        dayAndNightManager.StartDayAndNight();
        //
        kojeomCoroutineHelper.Init();
        inputManager.Init();
        //player Init
        playerManager.Init();
        //GameWorld Init
        worldManager.Init();
        //player controller start.
        playerManager.StartController();
        //LootingSystem Init;
        lootingSystem.Init();
        //InGameUI Init
        beltItemSelector.Init();
        //saveAndLoad Init
        saveAndLoadManager.Init();
        //
        npcManager.Init();
        npcManager.GenerateNPC();
        //
        actorCollideManager.Init();
        //
        ingameUISupervisor.Init();
        //
        // 날씨 매니저의 경우, 향후 4계절/눈/비 등을 관리한다.
        // 프로토타입의 수준으로 기능이 매우 미흡한 수준임.
        weatherManager.Init();

        if (GameStatus.isLoadGame == true) { saveAndLoadManager.Load(); }
        KojeomLogger.DebugLog("게임매니저 클래스 초기화 완료.");
    }

    private IEnumerator WaitingLogin()
    {
        if(GameStatus.isInGameSceneStart == false)
        {
            GameNetworkManager.GetNetworkManagerInstance().ReqInGameUserList();
        }
        KojeomLogger.DebugLog("네트워크 접속이 완료될 때 까지 대기합니다.", LOG_TYPE.SYSTEM);
        while (true)
        {
            if(GameNetworkStateFlags.isReceivedRandomSeedFormServer == true &&
                GameNetworkStateFlags.isReceiveGameUserList == true &&
                GameNetworkStateFlags.isCreatedMyGamePlayer == true)
            {
                break;
            }
          
            yield return null;
        }
        KojeomLogger.DebugLog("네트워크 접속 -> 플레이어 생성까지 완료되었습니다. Manager Class 초기화 시작합니다.", LOG_TYPE.SYSTEM);
        InitManagers();
    }
}
