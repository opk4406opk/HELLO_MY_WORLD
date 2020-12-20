using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 게임 상태를 관리하는 클래스.
/// </summary>
public struct GameStatusManager
{
    public static GameModeState CurrentGameModeState = GameModeState.NONE;
    public static DetailSingleMode DetailSingleMode = DetailSingleMode.NONE;
    public static GameUserNetType GetNetType()
    {
        return GameNetworkManager.GetInstance().UserNetType;
    }

    public static bool IsAllSubWorldDataReceived()
    {
        return GameNetworkManager.GetInstance().bFinishReceivedAllSubWorlds;
    }
}

public class GameLocalDataManager
{
    private static GameLocalDataManager Instance = null;
    public static GameLocalDataManager GetInstance()
    {
        if(Instance == null)
        {
            Instance = new GameLocalDataManager();
        }
        return Instance;
    }
    public int CharacterType = 0;
    public string CharacterName = "EditorTest";

    private GameLocalDataManager() { }
}

/// <summary>
/// 게임에 전반적인 관리를 하는 클래스.
/// ( 게임의 시작점 )
/// </summary>
public class GameSupervisor : MonoBehaviour
{
    #region simple config.
    public bool IsSoundOn = false;
    public bool IsHostPlay = false;
    #endregion
    //
    #region Inspector variables.
    [SerializeField]
    private GamePlayerManager playerManager;
    [SerializeField]
    private BeltItemSelector beltItemSelector;
    //[SerializeField]
    //private SaveAndLoadManager saveAndLoadManager;
    [SerializeField]
    private WorldAreaManager WorldAreaManager;
    [SerializeField]
    private GameSoundManager soundManager;
    [SerializeField]
    private EnviromentWeatherManager WeatherManager;
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private KojeomCoroutineHelper kojeomCoroutineHelper;
    //
    [SerializeField]
    private InGameUISupervisor ingameUISupervisor;

    [SerializeField]
    private ActorCollideManager actorCollideManager;
    [SerializeField]
    private ActorSuperviosr ActorSupervisorInstance;
    [SerializeField]
    private GameParticleEffectManager GameParticleManagerInstance;
    [SerializeField]
    private GamePlayerCameraManager PlayerCameraManagerInstance;
    [SerializeField]
    private InstancingHelper InstancingHelperInstance;
    #endregion
    public static GameSupervisor Instance { get; private set; }
    public AGameModeBase[] GameModeGroup = new AGameModeBase[(int)GameModeState.COUNT];
    private GameDataManager GameDataManagerInstance = new GameDataManager();
    private GameStateManager GameStateManagerInstance = new GameStateManager();
    public static object LockObject { get; private set; } = new object();
    private void Start ()
    {
        Application.targetFrameRate = 60;
#if UNITY_EDITOR
        bool bEditor = GameStatusManager.CurrentGameModeState == GameModeState.NONE &&
                       GameStatusManager.DetailSingleMode == DetailSingleMode.NONE;
        if (bEditor == true)
        {
            GameStatusManager.CurrentGameModeState = GameModeState.SINGLE;
            GameStatusManager.DetailSingleMode = DetailSingleMode.EDITOR_PLAY;
            GameNetworkManager.GetInstance().UserNetType = IsHostPlay ? GameUserNetType.Host : GameUserNetType.Client;
            GameNetworkManager.GetInstance().ConnectToGameServer(GameNetworkManager.GetLocalIP(), 8000, GameNetworkManager.GetInstance().UserNetType);
            KojeomLogger.DebugLog(string.Format("UNITY_EDITOR Play. NetType : {0}", GameNetworkManager.GetInstance().UserNetType), LOG_TYPE.SYSTEM);
        }
#endif
        Instance = this;
        GameModeGroup[(int)GameModeState.SINGLE] = new SingleGameMode();
        GameModeGroup[(int)GameModeState.MULTI] = new MultiGameMode();
        //init game mode.
        GameModeGroup[(int)GameStatusManager.CurrentGameModeState].Init();
        //
        switch (GameStatusManager.CurrentGameModeState)
        {
            case GameModeState.SINGLE:
                KojeomLogger.DebugLog(string.Format("GameModeState : {0}, Detail : {1}, User Network Type : {2}",
                    GameStatusManager.CurrentGameModeState, GameStatusManager.DetailSingleMode, GameNetworkManager.GetInstance().UserNetType), LOG_TYPE.SYSTEM);
                break;
            case GameModeState.MULTI:
                KojeomLogger.DebugLog(string.Format("GameModeState : {0}, User Network Type : {1}",
                    GameStatusManager.CurrentGameModeState, GameNetworkManager.GetInstance().UserNetType), LOG_TYPE.SYSTEM);
                break;
        }
        // 게임 데이터 파일 초기화.
        GameDataManagerInstance.Initialize();
        // 게임 플레이 관련 초기화.
        InitializeManagers();
        // 게임 스테이트 시작.
        GameStateManagerInstance.ChangeState(GameStateType.Prepare);
    }

    void Update()
    {
        if(GameModeGroup[(int)GameStatusManager.CurrentGameModeState] != null) GameModeGroup[(int)GameStatusManager.CurrentGameModeState].UpdateProcess(Time.deltaTime);
        if (GameStateManagerInstance != null) GameStateManagerInstance.UpdateProcess();
    }

    /// <summary>
    /// 게임내 각종 매니저 클래스들을 초기화합니다. ( 게임 데이터파일들이 초기화 된 이후에 호출되야 합니다. )
    /// </summary>
    public void InitializeManagers()
    {
        KojeomLogger.DebugLog("게임매니저 클래스들을 초기화 합니다.");
        // sound init.
        if(IsSoundOn == true) GameSoundManager.GetInstnace().PlaySound(GAME_SOUND_TYPE.BGM_InGame);
        //
        InstancingHelperInstance.Init();
        //
        kojeomCoroutineHelper.Init();
        inputManager.Init();
        GameParticleManagerInstance.Init();
        //Game Total World Init
        WorldAreaManager.Init();
        //player Init
        PlayerCameraManagerInstance.Init();
        playerManager.Init();
        // Actor Manager init..
        ActorSupervisorInstance.Init();
        ActorSupervisorInstance.Begin();
        //InGameUI Init
        beltItemSelector.Init();
        //saveAndLoad Init
        //saveAndLoadManager.Init();
        //
        actorCollideManager.Init();
        //
        ingameUISupervisor.Init();
        //
        // 날씨 매니저의 경우, 향후 4계절/눈/비 등을 관리한다.
        // 프로토타입의 수준으로 기능이 매우 미흡한 수준임.
        WeatherManager.Init();

        KojeomLogger.DebugLog("게임매니저 클래스 초기화 완료.");
    }

    void OnApplicationQuit()
    {
        GameNetworkManager.GetInstance().DisconnectToGameServer();
    }
}
