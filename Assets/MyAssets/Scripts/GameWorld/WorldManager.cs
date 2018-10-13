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

public struct SubWorldNormalizedOffset
{
    SubWorldNormalizedOffset(int x, int z) { this.x = x; this.z = z; }
    public int x;
    public int z;
}
public class WorldState
{
    public World subWorldInstance;
    public SubWorldNormalizedOffset normalizedOffset;
    public bool isInGameLoaded = false;
}

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    private GameObject worldPrefab;
    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private Transform worldGroupTrans;

    public Dictionary<SubWorldNormalizedOffset, int> worldOffsetToIndex { get; } = new Dictionary<SubWorldNormalizedOffset, int>();
    public Dictionary<int, WorldState> wholeWorldStates { get; } = new Dictionary<int, WorldState>();

    public static WorldManager instance;

    public void Init()
    {
        KojeomLogger.DebugLog("GameWorld 생성을 시작합니다.");
        CreateWholeWorld();
        if (GameManager.instance != null && GameManager.instance.isSubWorldDataSave == true)
        {
            SaveSubWorldFile();
        }
        instance = this;

        StartCoroutine(DynamicSubWorldLoader());
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
        foreach(var element in wholeWorldStates)
        {
            string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", element.Value.subWorldInstance.worldName);
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

            WorldDataFile dataFile = new WorldDataFile();
            dataFile.blockData = element.Value.subWorldInstance.worldBlockData;
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
        WorldState worldState;
        wholeWorldStates.TryGetValue(subWorldIdx, out worldState);
        string fileName = worldState.subWorldInstance.worldName;
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

    private void ReleaseSubWorldInstance(int worldIdx)
    {
        wholeWorldStates[worldIdx].subWorldInstance = null;
        wholeWorldStates[worldIdx].isInGameLoaded = false;
    }

    private void CreateWholeWorld()
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        foreach(var subWorldData in SubWorldDataFile.instance.subWorldDataList)
        {
            GameObject newSubWorld = Instantiate(worldPrefab, new Vector3(0, 0, 0),
               new Quaternion(0, 0, 0, 0)) as GameObject;
            World subWorld = newSubWorld.GetComponent<World>();
            subWorld.chunkPrefab = chunkPrefab;
            subWorld.playerTrans = PlayerManager.instance.myGamePlayer.transform;
            subWorld.worldName = subWorldData.worldName;
            subWorld.idx = subWorldData.worldIdx;
            newSubWorld.transform.parent = worldGroupTrans;
            //add world.
            WorldState worldState = new WorldState();
            worldState.subWorldInstance = subWorld;
            SubWorldNormalizedOffset normalizedSubOffset;
            normalizedSubOffset.x = subWorldData.x;
            normalizedSubOffset.z = subWorldData.z;
            worldState.normalizedOffset = normalizedSubOffset;
            worldState.isInGameLoaded = false;
            wholeWorldStates.Add(subWorldData.worldIdx, worldState);

            // offset to index 
            worldOffsetToIndex.Add(normalizedSubOffset, subWorldData.worldIdx);
        }
    }

    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public World ContainedWorld(Vector3 pos)
    {
        return wholeWorldStates[CalcSubWorldIndex(pos)].subWorldInstance;
    }

    IEnumerator DynamicSubWorldLoader()
    {
        KojeomLogger.DebugLog("DynamicSubWorldLoader Co-Routine Start.");
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        while (true)
        {
            // to do
            if(PlayerManager.instance != null)
            {
                Transform playerTrans = PlayerManager.instance.myGamePlayer.charInstance.transform;
                int playerPositionedSubWorldIdx = CalcSubWorldIndex(playerTrans.position);
                var offset = wholeWorldStates[playerPositionedSubWorldIdx].normalizedOffset;
                // 플레이어가 위치한 서브월드의 offset 위치를 기준삼아
                // 8방향(대각선, 좌우상하)의 subWorld를 활성화 시킨다. 
                // 플레이어 주변을 넘어서는 그 바깥의 영역들은 외부 파일로 저장시키는걸 비동기로..
                // 
                for(int x = offset.x - 1; x <= offset.x + 1; x++)
                {
                    for(int z = offset.z - 1; z <= offset.z + 1; z++)
                    {
                        int worldIdx = 0;
                        SubWorldNormalizedOffset subWorldOffset;
                        subWorldOffset.x = x;
                        subWorldOffset.z = z;
                        if (worldOffsetToIndex.TryGetValue(subWorldOffset, out worldIdx) == true)
                        {
                            if ((wholeWorldStates[worldIdx].subWorldInstance != null) &&
                                (wholeWorldStates[worldIdx].isInGameLoaded == false))
                            {
                                wholeWorldStates[worldIdx].isInGameLoaded = true;
                                wholeWorldStates[worldIdx].subWorldInstance.Init(x * gameConfig.sub_world_x_size,
                                    z * gameConfig.sub_world_z_size);
                            }
                        }
                    }
                }
                
            }
            yield return null;
        }
    }


    /// <summary>
    /// 오브젝트 위치를 통해 어느 SubWorld에 위치했는지 확인 후 해당 World Index를 리턴.
    /// </summary>
    /// <param name="objectPos">오브젝트 위치</param>
    /// <returns></returns>
    public int CalcSubWorldIndex(Vector3 objectPos)
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        int x = (int)objectPos.x / gameConfig.sub_world_x_size;
        int z = ((int)objectPos.z / gameConfig.sub_world_z_size) * SubWorldDataFile.instance.rowOffset;
        return x + z;
    }
}
