using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameWorldConfig
{
    private static int _worldX = 256;
    public static int worldX
    {
        get { return _worldX; }
    }
    private static int _worldY = 32;
    public static int worldY
    {
        get { return _worldY; }
    }
    private static int _worldZ = 256;
    public static int worldZ
    {
        get { return _worldZ; }
    }
    private static int _chunkSize = 8;
    public static int chunkSize
    {
        get { return _chunkSize; }
    }
}

public class SubWorldDataFile
{
    private JSONObject subWorldJsonObj;
    private TextAsset jsonFile;
    private List<Dictionary<string, string>> jsonDataSheet;
    
    private int _maxSubWorld = 0;
    public int maxSubWorld
    {
        get { return _maxSubWorld; }
    }

    public SubWorldDataFile()
    {
        jsonDataSheet = new List<Dictionary<string, string>>();
        jsonFile = Resources.Load("TextAsset/SubWorldDefaultData/subworld_default") as TextAsset;
        subWorldJsonObj = new JSONObject(jsonFile.text);
        AccessData(subWorldJsonObj);
    }

    public int GetPosValue(int idx, string str)
    {
        string value;
        jsonDataSheet[idx].TryGetValue(str, out value);
        return int.Parse(value);
    }

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                //to do
                break;
            case JSONObject.Type.ARRAY:
                _maxSubWorld = jsonObj.Count;
                for (int idx = 0; idx < jsonObj.Count; ++idx)
                {
                    jsonDataSheet.Add(jsonObj.list[idx].ToDictionary());
                }
                break;
            default:
                Debug.Log("Json Level Data Sheet Access ERROR");
                break;
        }

    }
}

    public class GameManager : MonoBehaviour
{
    private int offsetX = GameWorldConfig.worldX / GameWorldConfig.chunkSize;
    private int offsetZ = GameWorldConfig.worldZ / GameWorldConfig.chunkSize;
    private int MAX_SUB_WORLD = 0;

    private List<GameObject> list_SubWorlds = new List<GameObject>();

    private SubWorldDataFile subWorldData;

    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private GameObject worldPrefab;
    [SerializeField]
    private Transform worldGroupTrans;
    [SerializeField]
    private Transform playerTrans;
    
    void Start ()
    {
        subWorldData = new SubWorldDataFile();
        MAX_SUB_WORLD = subWorldData.maxSubWorld;
        CreateGameWorld();
    }

    private void CreateGameWorld()
    {
        for (int idx = 0; idx < MAX_SUB_WORLD; ++idx)
        {
            int subWorldPosX = subWorldData.GetPosValue(idx, "X") * offsetX;
            int subWorldPosZ = subWorldData.GetPosValue(idx, "Z") * offsetZ;

            GameObject newSubWorld = Instantiate(worldPrefab, new Vector3(0, 0, 0),
                new Quaternion(0, 0, 0, 0)) as GameObject;
            newSubWorld.GetComponent<World>().chunkPrefab = chunkPrefab;
            newSubWorld.GetComponent<World>().playerTrans = playerTrans;
            newSubWorld.GetComponent<World>().Init(subWorldPosX, subWorldPosZ);
            newSubWorld.name = "SUB_WORLD_" + idx.ToString();
            newSubWorld.transform.parent = worldGroupTrans;
            list_SubWorlds.Add(newSubWorld);
        }
    }
	
}
