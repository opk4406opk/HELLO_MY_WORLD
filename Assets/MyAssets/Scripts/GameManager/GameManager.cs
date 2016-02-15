using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameWorldConfig
{
    private static int _worldX = 64;
    public static int worldX
    {
        get { return _worldX; }
    }
    private static int _worldY = 32;
    public static int worldY
    {
        get { return _worldY; }
    }
    private static int _worldZ = 64;
    public static int worldZ
    {
        get { return _worldZ; }
    }
    private static int _chunkSize = 8;
    public static int chunkSize
    {
        get { return _chunkSize; }
    }
    private static int _offsetX = _worldX / _chunkSize;
    public static int offsetX
    {
        get { return _offsetX; }
    }
    private static int _offsetZ = _worldZ / _chunkSize;
    public static int offsetZ
    {
        get { return _offsetZ; }
    }
}

public class GameManager : MonoBehaviour
{
    private int MAX_SUB_WORLD = 0;
    private Dictionary<string, World> worldDictionary = new Dictionary<string, World>();

    [SerializeField]
    private SubWorldDataFile subWorldData;
    [SerializeField]
    private TileDataFile tileData;
    [SerializeField]
    private ItemDataFile itemData;

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
    
    void Start ()
    {
        //player Init
        playerManager.Init();
        playerTrans = playerManager.gamePlayer.transform;

        //GameData Init
        itemData.Init();
        tileData.Init();
        subWorldData.Init();

        //InGameUI Init
        blockSelector.Init(tileData);


        MAX_SUB_WORLD = subWorldData.maxSubWorld;
        CreateGameWorld();
    }

    private void CreateGameWorld()
    {
        for (int idx = 0; idx < MAX_SUB_WORLD; ++idx)
        {
            int subWorldPosX = subWorldData.GetPosValue(idx, "X") * GameWorldConfig.offsetX;
            int subWorldPosZ = subWorldData.GetPosValue(idx, "Z") * GameWorldConfig.offsetZ;
            string subWorldName = subWorldData.GetWorldName(idx, "WORLD_NAME");

            GameObject newSubWorld = Instantiate(worldPrefab, new Vector3(0, 0, 0),
                new Quaternion(0, 0, 0, 0)) as GameObject;
            newSubWorld.GetComponent<World>().chunkPrefab = chunkPrefab;
            newSubWorld.GetComponent<World>().playerTrans = playerTrans;
            newSubWorld.GetComponent<World>().Init(subWorldPosX, subWorldPosZ, tileData);
            newSubWorld.name = subWorldName; 
            newSubWorld.transform.parent = worldGroupTrans;
            //add world.
            worldDictionary.Add(subWorldName, newSubWorld.GetComponent<World>());
        }
    }
	
}
