using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 외부파일로 저장하게되는 World info.
/// </summary>
/// https://stackoverflow.com/questions/27932876/vector3-not-serializable-unity3d
[Serializable]
public class WorldDataFile
{
    public int idx;
    public Block[,,] blockData;
}

// 비동기 처리에 대한 참고 문서.
// http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/
// https://tech.peoplefund.co.kr/2017/08/02/non-blocking-asynchronous-concurrency.html
// https://blog.stephencleary.com/2012/02/async-and-await.html

public struct SubWorldOffset
{
    SubWorldOffset(int x, int z) { X = x; Z = z; }
    public int X;
    public int Z;
}
public class WorldState
{
    public World subWorldInstance;
    public SubWorldOffset offset;
    public bool isActivate = false;
    public bool isLoaded = false;
}

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    private GameObject worldPrefab;
    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private Transform worldGroupTrans;

    private Dictionary<int, string> subWorldFileNameCache= new Dictionary<int, string>();

    private List<WorldState> _worldStateList = new List<WorldState>();
    public List<WorldState> worldStateList
    {
        get { return _worldStateList; }
    }
    private int maxSubWorld = 0;

    public static WorldManager instance;

    public void Init()
    {
        KojeomLogger.DebugLog("GameWorld 생성을 시작합니다.");
        CreateDefaultSizeGameWorld();
        if (GameManager.instance != null && GameManager.instance.isSubWorldDataSave == true)
        {
            SaveSubWorldFile();
        }
        instance = this;
        KojeomLogger.DebugLog("GameWorld 생성을 종료합니다.");
    }
    /// <summary>
    /// C# sereialization 기능을 이용한
    /// subWorld를 외부파일로 저장하는 메소드.
    /// </summary>
    private void SaveSubWorldFile()
    {
        Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
        int idx = 0;
        foreach(var worldState in _worldStateList)
        {
            string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", worldState.subWorldInstance.worldName);
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

            WorldDataFile dataFile = new WorldDataFile();
            dataFile.blockData = worldState.subWorldInstance.worldBlockData;
            dataFile.idx = idx;
            // 시리얼라이징.
            bf.Serialize(fileStream, dataFile);
            fileStream.Close();
            //
            idx++;
        }
    }

    /// <summary>
    /// 특정 idx를 가진 서브월드를 로드합니다.
    /// </summary>
    /// <param name="subWorldIdx"></param>
    /// <returns></returns>
    private async Task<WorldDataFile> LoadSubWorldFile(int subWorldIdx)
    {
        string fileName;
        subWorldFileNameCache.TryGetValue(subWorldIdx, out fileName);
        string filePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", fileName);
        // 파일 열기.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        // deserializing.
        return await Task.FromResult(bf.Deserialize(fileStream) as WorldDataFile);
    }

    public async void LoadSubWorldFileAsync(int subWorldIdx)
    {
         await LoadSubWorldFile(subWorldIdx);
    }

    private void CreateDefaultSizeGameWorld()
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        maxSubWorld = SubWorldDataFile.instance.maxSubWorld;
        for (int idx = 0; idx < maxSubWorld; ++idx)
        {
            int subWorldPosX = SubWorldDataFile.instance.GetPosValue(idx, "X") * gameConfig.sub_world_x_size;
            int subWorldPosZ = SubWorldDataFile.instance.GetPosValue(idx, "Z") * gameConfig.sub_world_z_size;
            string subWorldName = SubWorldDataFile.instance.GetWorldName(idx, "WORLD_NAME");

            GameObject newSubWorld = Instantiate(worldPrefab, new Vector3(0, 0, 0),
                new Quaternion(0, 0, 0, 0)) as GameObject;
            World subWorld = newSubWorld.GetComponent<World>();
            subWorld.chunkPrefab = chunkPrefab;
            subWorld.playerTrans = PlayerManager.instance.myGamePlayer.transform;
            subWorld.worldName = subWorldName;
            subWorld.idx = idx;
            newSubWorld.transform.parent = worldGroupTrans;
            //add world.
            WorldState worldState = new WorldState();
            worldState.subWorldInstance = subWorld;
            SubWorldOffset subOffset;
            subOffset.X = subWorldPosX;
            subOffset.Z = subWorldPosZ;
            worldState.offset = subOffset;
            worldState.isActivate = false;
            worldState.isLoaded = false;
            _worldStateList.Add(worldState);
            // 
            subWorldFileNameCache.Add(idx, subWorld.worldName);
        }
    }

    private void ActivateSubWorld(int subWorldIdx)
    {

    }

    private void DeActivateSubWorld(int subWorldIdx)
    {

    }

    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public World ContainedWorld(Vector3 pos)
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        int x = (int)pos.x / gameConfig.sub_world_x_size;
        int z = ((int)pos.z / gameConfig.sub_world_z_size) * SubWorldDataFile.instance.rowOffset;

        return _worldStateList[x+z].subWorldInstance;
    }

    IEnumerator DynamicWorldLoader()
    {
        while(true)
        {
            // to do
            yield return null;
        }
    }
}
