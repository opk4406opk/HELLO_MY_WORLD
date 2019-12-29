using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// 게임 상태(single, multi, load, save)를 관리하는 클래스.
/// </summary>
public struct GameStatus
{
    public static GameModeState CurrentGameModeState = GameModeState.NONE;
    public static DetailSingleMode DetailSingleMode = DetailSingleMode.NONE;
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
    public bool bSoundOn = false;
    public bool bSubWorldDataSave = false;
    public bool bLockCursor = false;
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
    #endregion
    public static GameSupervisor Instance { get; private set; }
    public AGameModeBase[] GameModeGroup = new AGameModeBase[(int)GameModeState.COUNT];
    private GameDataManager GameDataManagerInstance = new GameDataManager();
    private void Start ()
    {
#if UNITY_EDITOR
        bool bEditor = GameStatus.CurrentGameModeState == GameModeState.NONE && GameStatus.DetailSingleMode == DetailSingleMode.NONE;
        if (bEditor == true)
        {
            GameStatus.CurrentGameModeState = GameModeState.SINGLE;
            GameStatus.DetailSingleMode = DetailSingleMode.EDITOR_PLAY;
            GameNetworkManager.GetInstance().IdentityType = GameNetIdentityType.Host;
        }
#endif
        Instance = this;
        KojeomLogger.DebugLog(string.Format("GameModeState : {0}, Detail : {1}", GameStatus.CurrentGameModeState, GameStatus.DetailSingleMode), LOG_TYPE.SYSTEM);

        GameModeGroup[(int)GameModeState.SINGLE] = new SingleGameMode();
        GameModeGroup[(int)GameModeState.MULTI] = new MultiGameMode();
        //init game mode.
        GameModeGroup[(int)GameStatus.CurrentGameModeState].Init();
        //
        InitSettings();
        GameDataManagerInstance.Initialize();
        InitManagers();
    }

    private void Update()
    {
        if(GameModeGroup[(int)GameStatus.CurrentGameModeState] != null)
        {
            GameModeGroup[(int)GameStatus.CurrentGameModeState].Tick(Time.deltaTime);
        }
    }

    private void InitSettings()
    {
        if(bLockCursor == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    /// <summary>
    /// 게임내 각종 매니저 클래스들을 초기화합니다. ( 게임 데이터파일들이 초기화 된 이후에 호출되야 합니다. )
    /// </summary>
    public void InitManagers()
    {
        KojeomLogger.DebugLog("게임매니저 클래스들을 초기화 합니다.");
        // sound init.
        if(bSoundOn == true)
        {
            GameSoundManager.GetInstnace().PlaySound(GAME_SOUND_TYPE.BGM_InGame);
        }
        //
        kojeomCoroutineHelper.Init();
        inputManager.Init();
        //Game Total World Init
        GameParticleManagerInstance.Init();
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

        if (GameStatus.DetailSingleMode == DetailSingleMode.LOAD_GAME)
        {
            // 게임을 로드?.
        }
        KojeomLogger.DebugLog("게임매니저 클래스 초기화 완료.");
    }
}
