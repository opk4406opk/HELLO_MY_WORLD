using UnityEngine;
using System.Collections;

public enum GameMode
{
    NONE,
    INGAME_EDITOR,
    SINGLE,
    MULTI_P2P,
    MULTI_INTERNET
}
public enum DetailSingleMode
{
    NONE,
    SAVE_GAME,
    LOAD_GAME
}
/// <summary>
/// 게임 상태(single, multi, load, save)를 관리하는 클래스.
/// </summary>
public class GameStatus
{
    // 빠른 테스트를 위한 디폴트값으로 인게임에디터로 설정.
    public static GameMode GameModeFlag = GameMode.INGAME_EDITOR;
    public static DetailSingleMode DetailSingleModeFlag = DetailSingleMode.NONE;
}

/// <summary>
/// 게임에 전반적인 관리를 하는 클래스.
/// ( 게임의 시작점 )
/// </summary>
public class GameSupervisor : MonoBehaviour
{
    #region data file.
    // data file parser.
    private WorldConfigFile WorldConfigDataFileInstance = new WorldConfigFile();
    private WorldMapDataFile SubWorldDataFileInstance = new WorldMapDataFile();
    private BlockTileDataFile TileDataFileInstance = new BlockTileDataFile();
    private ItemDataFile ItemDataFileInstance = new ItemDataFile();
    private CraftItemListDataFile CraftItemListDataFileInstance = new CraftItemListDataFile();
    private NPCDataFile NpcDataFileInstance = new NPCDataFile();
    private GameConfigDataFile GameConfigDataFileInstance = new GameConfigDataFile();
    //
    #endregion

    #region simple config.
    public bool isSoundOn = false;
    public bool isSubWorldDataSave = false;
    public bool isLockCursor = false;
    #endregion
    //
    #region Inspector variables.
    [SerializeField]
    private GamePlayerManager playerManager;
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
    private ActorCollideManager actorCollideManager;
    [SerializeField]
    private ActorSuperviosr ActorSupervisorInstance;
    #endregion


    public static GameSupervisor Instance = null;

    void Start ()
    {
        KojeomLogger.DebugLog(string.Format("GameMode : {0}, DataMode : {1}", GameStatus.GameModeFlag, GameStatus.DetailSingleModeFlag), LOG_TYPE.SYSTEM);
        Instance = this;
        // single or multi play 게임이 아니라면
        // InGame Scene에서 바로 시작하는 경우 ( in editor mode )
        if (GameStatus.GameModeFlag == GameMode.INGAME_EDITOR)
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
        GameObject netMgr = Resources.Load<GameObject>(ConstFilePath.GAME_NET_MGR_PREFAB_RESOURCE_PATH);
        Instantiate(netMgr, new Vector3(0, 0, 0), Quaternion.identity);
        //
        KojeomLogger.DebugLog("StartHost", LOG_TYPE.INFO);
        P2PNetworkManager.GetInstance().onlineScene = "InGame";
        var netClient = P2PNetworkManager.GetInstance().StartHost();
        P2PNetworkManager.GetInstance().isHost = true;
        P2PNetworkManager.GetInstance().LateInit();
        // InGame씬에서 바로 시작하는 경우에는 해당 flag를 true로 설정.
        P2PNetworkStateFlagBoard.isReceivedRandomSeedFormServer = true;
        P2PNetworkStateFlagBoard.isReceiveGameUserList = true;
        P2PNetworkManager.InitGameRandomSeed(System.DateTime.Now.Second);
    }

    /// <summary>
    /// 게임에 사용되는 데이터파일들을 초기화합니다. ( 게임 매니저 초기화보다 먼저 호출되야 합니다. )
    /// </summary>
    private void InitDataFiles()
    {
        KojeomLogger.DebugLog("게임 데이터 파일 초기화 시작.");
        //GameDataFiles Init
        // 제작아이템 데이타파일은 아이템데이타 파일을 읽어들인 후에 읽어야함.
        WorldConfigDataFileInstance.Init();
        GameConfigDataFileInstance.Init();
        ItemDataFileInstance.Init();
        TileDataFileInstance.Init();
        SubWorldDataFileInstance.Init();
        CraftItemListDataFileInstance.Init();
        NpcDataFileInstance.Init();
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
        //GameWorld Init
        worldManager.Init();
        //player Init
        playerManager.Init();
        // Actor Manager init..
        ActorSupervisorInstance.Init();
        ActorSupervisorInstance.Begin();
        // looting init.
        lootingSystem.Init();
        //InGameUI Init
        beltItemSelector.Init();
        //saveAndLoad Init
        saveAndLoadManager.Init();
        //
        actorCollideManager.Init();
        //
        ingameUISupervisor.Init();
        //
        // 날씨 매니저의 경우, 향후 4계절/눈/비 등을 관리한다.
        // 프로토타입의 수준으로 기능이 매우 미흡한 수준임.
        weatherManager.Init();

        if (GameStatus.DetailSingleModeFlag == DetailSingleMode.LOAD_GAME)
        {
            saveAndLoadManager.Load();
        }
        KojeomLogger.DebugLog("게임매니저 클래스 초기화 완료.");
    }

    private IEnumerator WaitingLogin()
    {
        if(GameStatus.GameModeFlag == GameMode.MULTI_INTERNET || GameStatus.GameModeFlag == GameMode.MULTI_P2P)
        {
            P2PNetworkManager.GetInstance().ReqInGameUserList();
        }
        KojeomLogger.DebugLog("네트워크 접속이 완료될 때 까지 대기합니다.", LOG_TYPE.SYSTEM);
        while (true)
        {
            if(P2PNetworkStateFlagBoard.isReceivedRandomSeedFormServer == true &&
                P2PNetworkStateFlagBoard.isReceiveGameUserList == true &&
                P2PNetworkStateFlagBoard.isCreatedMyGamePlayer == true)
            {
                break;
            }
          
            yield return null;
        }
        KojeomLogger.DebugLog("네트워크 접속 -> 플레이어 생성까지 완료되었습니다. Manager Class 초기화 시작합니다.", LOG_TYPE.SYSTEM);
        InitManagers();
    }
}
