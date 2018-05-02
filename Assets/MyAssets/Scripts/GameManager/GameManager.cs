using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임 상태(single, multi, load, save)를 관리하는 클래스.
/// </summary>
public class GameStatus
{
    private static bool _isLoadGame = false;
    public static bool isLoadGame
    {
        set { _isLoadGame = value; }
        get { return _isLoadGame; }
    }

    private static bool _isMultiPlay = false;
    public static bool isMultiPlay
    {
        set { _isMultiPlay = value; }
        get { return _isMultiPlay; }
    }
}

/// <summary>
/// 게임에 전반적인 관리를 하는 클래스.
/// ( 게임의 시작점 )
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Inspector variables.
    
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
    private NPCManager npcManager;
    [SerializeField]
    private ActorCollideManager actorCollideManager;
    #endregion

    public static GameManager instance = null;

    void Start ()
    {
        instance = this;
        InitDataFiles();
        // 싱글플레이는 바로 매니저클래스들을 초기화한다.
        if(GameStatus.isMultiPlay == false)
        {
            InitManagers();
        }
        else
        {
            // 멀티플레이의 경우, 서버로 유저들이 접속-> 플레이어 생성까지 완료된 상태를 기다린 후에
            // 매니저 클래스들을 초기화 한다.
            StartCoroutine(WatingInit());
        }
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
        //GameSoundManager.GetInstnace().PlaySound(GAME_SOUND_TYPE.BGM_InGame);
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
        weatherManager.Init();
        //weatherManager.StartWeatherSystem();
        //
        inputManager.Init();

        if (GameStatus.isLoadGame == true) { saveAndLoadManager.Load(); }
        KojeomLogger.DebugLog("게임매니저 클래스 초기화 완료.");
    }

    private IEnumerator WatingInit()
    {
        KojeomLogger.DebugLog("네트워크 접속이 완료될 때 까지 대기합니다.");
        while (true)
        {
            if(GameNetworkManager.GetInstance().GetMyGamePlayer() != null)
            {
                break;
            }
            yield return null;
        }
        KojeomLogger.DebugLog("네트워크 접속 -> 플레이어 생성까지 완료되었습니다. 매니저들을 초기화합니다.");
        InitManagers();
    }
}
