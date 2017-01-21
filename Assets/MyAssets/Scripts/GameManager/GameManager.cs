using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임 월드의 설정을 관리하는 클래스.
/// </summary>
public class GameWorldConfig
{
    private static readonly int _worldX = 32;
    public static int worldX
    {
        get { return _worldX; }
    }
    private static readonly int _worldY = 32;
    public static int worldY
    {
        get { return _worldY; }
    }
    private static readonly int _worldZ = 32;
    public static int worldZ
    {
        get { return _worldZ; }
    }
    private static readonly int _chunkSize = 8;
    public static int chunkSize
    {
        get { return _chunkSize; }
    }
}

/// <summary>
/// 게임 상태( Load, Save, etc...)를 관리하는 클래스.
/// </summary>
public class GameStatus
{
    private static bool _isLoadGame = false;
    public static bool isLoadGame
    {
        set { _isLoadGame = value; }
        get { return _isLoadGame; }
    }
}

/// <summary>
/// 게임에 전반적인 관리를 하는 클래스.
/// ( 게임의 시작점 )
/// </summary>
public class GameManager : MonoBehaviour
{
    private int MAX_SUB_WORLD = 0;

    private List<World> _worldList = new List<World>();
    public List<World> worldList
    {
        get { return _worldList; }
    }

    [SerializeField]
    private SubWorldDataFile subWorldData;
    [SerializeField]
    private TileDataFile tileData;
    [SerializeField]
    private ItemDataFile itemData;
    [SerializeField]
    private CraftItemListDataFile craftItemListDataFile;

    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private GameObject worldPrefab;
    [SerializeField]
    private Transform worldGroupTrans;
    private Transform playerTrans;

    [SerializeField]
    private PlayerManager playerManager;
    [SerializeField]
    private BlockSelector blockSelector;
    [SerializeField]
    private SaveAndLoadManager saveAndLoadManager;
    [SerializeField]
    private LootingSystem lootingSystem;

    [SerializeField]
    private NPC_Controller yukoNPC;
    public NPC_Controller GetYuKoNPC() { return yukoNPC; }

    void Start ()
    {
        //player Init
        playerManager.Init();
        playerTrans = playerManager.gamePlayer.transform;

        //GameData Init
        itemData.Init();
        tileData.Init();
        subWorldData.Init();
        craftItemListDataFile.Init();

        //LootingSystem Init;
        lootingSystem.Init();

        //InGameUI Init
        blockSelector.Init(tileData);

        //GameWorld Init
        MAX_SUB_WORLD = subWorldData.maxSubWorld;
        CreateGameWorld();

        //saveAndLoad Init
        saveAndLoadManager.Init();

        if (GameStatus.isLoadGame == true) { saveAndLoadManager.Load(); }
    }
		
    private void CreateGameWorld()
    {
        for (int idx = 0; idx < MAX_SUB_WORLD; ++idx)
        {
            int subWorldPosX = subWorldData.GetPosValue(idx, "X") * GameWorldConfig.worldX;
            int subWorldPosZ = subWorldData.GetPosValue(idx, "Z") * GameWorldConfig.worldZ;
            string subWorldName = subWorldData.GetWorldName(idx, "WORLD_NAME");

            GameObject newSubWorld = Instantiate(worldPrefab, new Vector3(0, 0, 0),
                new Quaternion(0, 0, 0, 0)) as GameObject;
            newSubWorld.GetComponent<World>().chunkPrefab = chunkPrefab;
            newSubWorld.GetComponent<World>().playerTrans = playerTrans;
            newSubWorld.GetComponent<World>().Init(subWorldPosX, subWorldPosZ, tileData);
            newSubWorld.GetComponent<World>().worldName = subWorldName;
            newSubWorld.transform.parent = worldGroupTrans;
            //add world.
            _worldList.Add(newSubWorld.GetComponent<World>());
        }
    }
	
}
