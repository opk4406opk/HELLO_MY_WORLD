using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 게임 월드의 설정을 관리하는 클래스.
/// </summary>
public class GameConfig
{
    public static readonly int subWorldX = 32;
    public static readonly int subWorldY = 32;
    public static readonly int subWorldZ = 32;
    public static readonly int subWorldChunkSize = 8;

    public static readonly int inGameFontSize = 10;
 
}

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
    private PlayerManager playerManager;
    [SerializeField]
    private BlockSelector blockSelector;
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
    private NPCManager npcManager;
    [SerializeField]
    private ActorCollideManager actorCollideManager;
    #endregion
    
    void Start ()
    {
        KojeomLogger.DebugLog("GameManager is Setup Start.");
        //GameDataFiles Init
        // 제작아이템 데이타파일은 아이템데이타 파일을 읽어들인 후에 읽어야함.
        itemDataFile.Init();
        tileDataFile.Init();
        subWorldDataFile.Init();
        craftItemListDataFile.Init();
        npcDataFile.Init();

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
        blockSelector.Init();

        //saveAndLoad Init
        saveAndLoadManager.Init();

        //
        npcManager.Init();
        npcManager.GenerateNPC();

        //
        actorCollideManager.Init();

        //
        weatherManager.Init();
        weatherManager.StartWeatherSystem();

        if (GameStatus.isLoadGame == true) { saveAndLoadManager.Load(); }
        KojeomLogger.DebugLog("GameManager is Setup End.");
    }
}
