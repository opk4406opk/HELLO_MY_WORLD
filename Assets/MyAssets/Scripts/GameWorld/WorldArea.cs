using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using MapGenLib;
using Unity.Collections;
/// <summary>
/// 외부파일로 저장하게되는 World info.
/// </summary>
/// https://stackoverflow.com/questions/27932876/vector3-not-serializable-unity3d
[Serializable]
public class SubWorldExternalDataFile
{
    public string AreaUniqueID;
    public string SubWorldUniqueID;  
    public Block[,,] BlockData;
}

// 비동기 처리에 대한 참고 문서.
// http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/
// https://tech.peoplefund.co.kr/2017/08/02/non-blocking-asynchronous-concurrency.html
// https://blog.stephencleary.com/2012/02/async-and-await.html

public struct SubWorldNormalizedOffset
{
    SubWorldNormalizedOffset(int x, int y, int z) { X = x; Y = y; Z = z;  }
    public int X;
    public int Y;
    public int Z;
}
public class SubWorldState
{
    public SubWorld SubWorldInstance;
    public SubWorldRealTimeStatus RealTimeStatus = SubWorldRealTimeStatus.None;
}

public enum SubWorldRealTimeStatus
{
    None,
    ReadyToFirstLoad, // 최초로딩 대기중.
    Loading, // 로딩중.
    LoadFinish, // 로딩완료.
    Release, // 해제중.
    ReleaseFinish // 해제완료.
}

public class WorldArea : MonoBehaviour
{
    public string AreaName { get; private set; }
    [ReadOnly]
    public string AreaUniqueID;
    // WorldArea 위치값( == 오프셋값).
    public Vector3 OffsetCoordinate { get; private set; }
    // 실제 게임오브젝트로서 존재하는 위치값.
    public Vector3 RealCoordinate { get; private set; }
    public Dictionary<string, SubWorldState> SubWorldStates { get; } = new Dictionary<string, SubWorldState>();
    // XZ 평면 데이터 정보 ( SubWorld에서 블록정보 세팅할 때 필요함.)
    public WorldGenAlgorithms.TerrainValue[,] XZPlaneDataArray { get; private set; }
    public Dictionary<string ,Block[,,]> SubWorldBlocksWithPerlinNoise { get; private set; }
    public bool bRunningLoader { get; private set; }
    private IEnumerator SubWorldLoaderEnumerator;
    public bool bInitFinish { get; private set; } = false;
    public bool bSurface { get; private set; }
    public WorldGenTypes WorldGenerateType { get; private set; } = WorldGenTypes.NONE;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="worldAreaData"> 월드 아레아 데이터.</param>
    /// <param name="worldAreaXZPlaneData"> 월드 생성 XZ 평면 데이터.</param>
    /// <param name="bStartLoader"> 서브월드 생성기 시작 여부</param>
    public void Init(WorldAreaTerrainData worldAreaData, WorldTerrainGenerateStruct worldTerrainGenStruct)
    {
        KojeomLogger.DebugLog(string.Format("WorldArea : {0} 생성을 시작합니다.", AreaUniqueID));
        //
        bInitFinish = false;
        //
        XZPlaneDataArray = worldTerrainGenStruct.NormalGenData;
        SubWorldBlocksWithPerlinNoise = worldTerrainGenStruct.PerlinGenData;
        //
        AreaName = worldAreaData.AreaName;
        AreaUniqueID = worldAreaData.UniqueID;
        WorldGenerateType = worldAreaData.GenerateType;
        bSurface = worldAreaData.bSurface;
        OffsetCoordinate = new Vector3(worldAreaData.OffsetX, worldAreaData.OffsetY, worldAreaData.OffsetZ);
        RealCoordinate = OffsetCoordinate;
        SubWorldLoaderEnumerator = DynamicSubWorldLoader();
        //
        StartCoroutine(CreateAreaProcess(worldAreaData));
    }

    private async void AsyncSaveAllSubWorld()
    {
        bool bSuccessAllSave = await TaskSaveAllSubWorld();
        if (bSuccessAllSave == true) KojeomLogger.DebugLog("Success Async Save All SubWorld.", LOG_TYPE.INFO);
        else KojeomLogger.DebugLog("Failed Async Save All SubWorld.", LOG_TYPE.ERROR);
    }

    /// <summary>
    /// C# sereialization 기능을 이용한
    /// 모든 SubWorld를 외부파일로 저장하는 메소드.
    /// </summary>
    private async Task<bool> TaskSaveAllSubWorld()
    {
        return await Task.Run(() => 
        {
            lock(GameSupervisor.LockObject)
            {
                Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
                int idx = 0;
                foreach (var element in SubWorldStates)
                {
                    string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", element.Value.SubWorldInstance.WorldName);
                    // 파일 생성.
                    BinaryFormatter bf = new BinaryFormatter();
                    FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

                    SubWorldExternalDataFile dataFile = new SubWorldExternalDataFile
                    {
                        BlockData = element.Value.SubWorldInstance.WorldBlockData,
                        SubWorldUniqueID = element.Value.SubWorldInstance.UniqueID,
                        AreaUniqueID = element.Value.SubWorldInstance.GetWorldAreaUniqueID()
                    };
                    // 시리얼라이징.
                    bf.Serialize(fileStream, dataFile);
                    fileStream.Close();
                    //
                    idx++;
                }
                return true;
            }
        });
    }

    /// <summary>
    /// 특정 서브월드만 외부파일로 저장.
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <returns></returns>
    public async Task<bool> TaskSaveSpecificSubWorld(string uniqueID)
    {
        return await Task.Run(() => {
            lock(GameSupervisor.LockObject)
            {
                if (Directory.Exists(ConstFilePath.RAW_SUB_WORLD_DATA_PATH) == false) Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
                //
                SubWorldStates.TryGetValue(uniqueID, out SubWorldState state);
                string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", state.SubWorldInstance.WorldName);
                // 파일 생성.
                BinaryFormatter bf = null;
                FileStream fileStream = null;
                SubWorldExternalDataFile dataFile = null;
                try
                {
                    bf = new BinaryFormatter();
                    fileStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                    dataFile = new SubWorldExternalDataFile
                    {
                        BlockData = state.SubWorldInstance.WorldBlockData,
                        SubWorldUniqueID = state.SubWorldInstance.UniqueID
                    };
                }
                catch (Exception e)
                {
                    KojeomLogger.DebugLog(e.ToString(), LOG_TYPE.ERROR);
                }
                finally
                {
                    // 시리얼라이징.
                    bf.Serialize(fileStream, dataFile);
                    fileStream.Close();
                }
                return true;
            }
        });
    }

    public async void AsyncReleaseSubWorldFile(string uniqueID)
    { 
        // 메모리 해제 직전, Sub WorldData를 외부 파일로 저장.
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Start and Waiting Async Save.", uniqueID));
        var bSaveSuccess = await TaskSaveSpecificSubWorld(uniqueID);
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} End Waiting Async Save.", uniqueID));
        if (bSaveSuccess == true)
        { 
            // 메모리 해제 시작.
            KojeomLogger.DebugLog(string.Format("Subworld ID : {0} is Start Release. ", uniqueID));
            SubWorldStates[uniqueID].SubWorldInstance.Release();
            SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Release;
        }
    }

    /// <summary>
    /// 특정 idx를 가진 서브월드를 로드합니다.
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <returns></returns>
    private async Task<SubWorldExternalDataFile> TaskLoadSubWorldFile(string uniqueID)
    {
        // deserializing.
        return await Task.Run(()=> {
            lock(GameSupervisor.LockObject)
            {
                SubWorldStates.TryGetValue(uniqueID, out SubWorldState worldState);
                string fileName = worldState.SubWorldInstance.WorldName;
                string filePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", fileName);
                // 파일 열기.
                BinaryFormatter bf = null;
                FileStream fileStream = null;
                SubWorldExternalDataFile worldDataFile = null;
                try
                {
                    bf = new BinaryFormatter();
                    fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);
                    worldDataFile = bf.Deserialize(fileStream) as SubWorldExternalDataFile;
                }
                catch (Exception e)
                {

                }
                finally
                {
                    fileStream.Close();
                }
                return worldDataFile;
            }
        });
    }

    public async void AsyncLoadSubWorldFile(string uniqueID)
    {
        SubWorldStates.TryGetValue(uniqueID, out SubWorldState worldState);
        // 실제 파일을 로딩.
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Start and Waiting Async Load from file.", uniqueID));
        var loadedWorldData = await TaskLoadSubWorldFile(uniqueID);
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Finish Waiting Async Load from file.", uniqueID));
        // 읽어들인 파일을 이용해 서브월드 로딩.
        worldState.SubWorldInstance.AsyncLoading(loadedWorldData.BlockData, false);
    }

    private IEnumerator CreateAreaProcess(WorldAreaTerrainData worldAreaData)
    {
        foreach(var subWorldData in worldAreaData.SubWorldDatas)
        {
            SubWorld subWorld = MakeNewSubWorldInstance();
            subWorld.OnFinishLoadChunks += OnFinishSubWorldLoadChunks;
            subWorld.OnReadyToRelease += OnReleaseSubWorldInstance;
            subWorld.OnFinishRelease += OnFinishReleaseSubWorldInstance;
            subWorld.Init(subWorldData, this);
            //add world.
            SubWorldState worldState = new SubWorldState
            {
                SubWorldInstance = subWorld,
                RealTimeStatus = SubWorldRealTimeStatus.ReadyToFirstLoad
            };
            SubWorldStates.Add(subWorldData.UniqueID, worldState);
            // wait.
            yield return new WaitForSeconds(0.1f);
        }
        bInitFinish = true;
        KojeomLogger.DebugLog(string.Format("WorldArea : {0} 생성을 종료합니다.", AreaUniqueID));
    }

    private SubWorld MakeNewSubWorldInstance()
    {
        GameObject newSubWorld = Instantiate(GameResourceSupervisor.GetInstance().SubWorldPrefab.LoadSynchro(), new Vector3(0, 0, 0),
               new Quaternion(0, 0, 0, 0)) as GameObject;
        newSubWorld.transform.parent = this.transform;
        //
        return newSubWorld.GetComponent<SubWorld>();
    }

    private void OnFinishSubWorldLoadChunks(string subWorldUniqueID)
    {
        SubWorldStates.TryGetValue(subWorldUniqueID, out SubWorldState worldState);
        worldState.RealTimeStatus = SubWorldRealTimeStatus.LoadFinish;
        worldState.SubWorldInstance.CurrentState = SubWorldRealTimeStatus.LoadFinish;
        //
        //NPC 생성 테스트. 
        if (worldState.SubWorldInstance.bSurfaceWorld == true)
        {
            ActorSuperviosr.Instance.RequestSpawnRandomNPC(NPC_TYPE.Merchant, subWorldUniqueID, AreaUniqueID, 1, true);
        }
        //Animal 생성 테스트. 
        if (worldState.SubWorldInstance.bSurfaceWorld == true)
        {
            ActorSuperviosr.Instance.RequestSpawnRandomAnimal(ANIMAL_TYPE.Chick, subWorldUniqueID, AreaUniqueID, 1, true);
        }
    }

    private void OnReleaseSubWorldInstance(string uniqueID)
    {
        switch(SubWorldStates[uniqueID].RealTimeStatus)
        {
            case SubWorldRealTimeStatus.LoadFinish:
                // 릴리즈 상태로 전환.
                SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Release;
                SubWorldStates[uniqueID].SubWorldInstance.CurrentState = SubWorldRealTimeStatus.Release;
                AsyncReleaseSubWorldFile(uniqueID);
                break;
        }
    }

    private void OnFinishReleaseSubWorldInstance(string uniqueID)
    {
        SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.ReleaseFinish;
        SubWorldStates[uniqueID].SubWorldInstance.CurrentState = SubWorldRealTimeStatus.ReleaseFinish;
    }

    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public SubWorld ContainedSubWorld(Vector3 pos)
    {
        var state = SubWorldStates[GetSubWorldUniqueID(pos)];
        if(state == null)
        {
            return null;
        }
        return state.SubWorldInstance;
    }

    public void StartSubworldLoader()
    {
        bRunningLoader = true;
        StartCoroutine(SubWorldLoaderEnumerator);
    }

    public void StopSubWorldLoader()
    {
        bRunningLoader = false;
        StopCoroutine(SubWorldLoaderEnumerator);
    }

    public void ForceLoadSubWorlds()
    {
        foreach (var state in SubWorldStates)
        {
            SubWorldState instance = state.Value;
            if(instance != null)
            {
                if(instance.RealTimeStatus == SubWorldRealTimeStatus.ReleaseFinish ||
                   instance.RealTimeStatus == SubWorldRealTimeStatus.ReadyToFirstLoad)
                {
                    instance.RealTimeStatus = SubWorldRealTimeStatus.Loading;
                    instance.SubWorldInstance.AsyncLoading(null, true);
                }
            }
        }
    }
   
    private IEnumerator DynamicSubWorldLoader()
    {
        KojeomLogger.DebugLog(string.Format("Area : {0} Dynamic SubWorldLoader Start.", AreaUniqueID));
        while (true)
        {
            if (bInitFinish == false) yield return null;
            if (bRunningLoader == false) yield return null;

            if (GamePlayerManager.Instance != null && GamePlayerManager.Instance.bFinishMake == true)
            {
                Vector3 playerPos = GamePlayerManager.Instance.MyGamePlayer.GetPosition();
                string containedAreaID = WorldAreaManager.GetWorldAreaUniqueID(playerPos);
                if (containedAreaID == AreaUniqueID)
                {
                    //KojeomLogger.DebugLog(string.Format("this area ID : {0}, player contained AreaID : {1}", AreaUniqueID, containedAreaID));
                    Vector3 offsetPos = SubWorldStates[GetSubWorldUniqueID(playerPos)].SubWorldInstance.OffsetCoordinate;
                    // 플레이어가 위치한 서브월드의 offset 위치를 기준삼아
                    // 8방향(대각선, 좌우상하)의 subWorld를 활성화 시킨다. 
                    // 플레이어 주변을 넘어서는 그 바깥의 영역들은 외부 파일로 저장시키는걸 비동기로..
                    List<Vector3> candidates = new List<Vector3>();
                    for (int x = (int)offsetPos.x - 1; x <= (int)offsetPos.x + 1; x++)
                    {
                        for (int z = (int)offsetPos.z - 1; z <= (int)offsetPos.z + 1; z++) candidates.Add(new Vector3(x, offsetPos.y, z));
                    }
                    candidates.Add(new Vector3(offsetPos.x, offsetPos.y - 1, offsetPos.z));

                    foreach(var pos in candidates)
                    {
                        string uniqueID = WorldAreaManager.MakeUniqueID((int)pos.x, (int)pos.y, (int)pos.z);
                        if (SubWorldStates.ContainsKey(uniqueID) == true)
                        {
                            switch (SubWorldStates[uniqueID].RealTimeStatus)
                            {
                                case SubWorldRealTimeStatus.None:
                                case SubWorldRealTimeStatus.Loading:
                                case SubWorldRealTimeStatus.Release:
                                    // nothing to do.
                                    break;
                                case SubWorldRealTimeStatus.ReadyToFirstLoad:
                                    SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Loading;
                                    SubWorldStates[uniqueID].SubWorldInstance.CurrentState = SubWorldRealTimeStatus.Loading;
                                    SubWorldStates[uniqueID].SubWorldInstance.AsyncLoading(null, true);
                                    break;
                                case SubWorldRealTimeStatus.ReleaseFinish:
                                    SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Loading;
                                    SubWorldStates[uniqueID].SubWorldInstance.CurrentState = SubWorldRealTimeStatus.Loading;
                                    AsyncLoadSubWorldFile(uniqueID);
                                    break;
                            }
                        }
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 서브월드 Name으로부터 Data Array Index를 추출.
    /// </summary>
    /// <param name="worldName"></param>
    /// <returns></returns>
    public static int MakeSubWorldDataIndex(string worldName)
    {
        var splits = worldName.Split('_');
        return int.Parse(splits[2]);
    }


    /// <summary>
    /// 오브젝트 위치를 통해 어느 SubWorld에 위치했는지 확인 후 해당 World UniqueID를 리턴.
    /// </summary>
    /// <param name="objectPos">오브젝트 위치</param>
    /// <returns></returns>
    public static string GetSubWorldUniqueID(Vector3 objectPos)
    {
        // 0 ~ 32 ~ 64 ~ 96 ~ 128
        //   #0   #1   #2   #3
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int subWorldOffsetX = Mathf.CeilToInt(Mathf.CeilToInt(objectPos.x) / gameWorldConfig.SubWorldSizeX) % WorldMapDataFile.Instance.MapData.SubWorldRow;
        int subWorldOffsetY = Mathf.CeilToInt(Mathf.CeilToInt(objectPos.y) / gameWorldConfig.SubWorldSizeY) % WorldMapDataFile.Instance.MapData.SubWorldLayer;
        int subWorldOffsetZ = Mathf.CeilToInt(Mathf.CeilToInt(objectPos.z) / gameWorldConfig.SubWorldSizeZ) % WorldMapDataFile.Instance.MapData.SubWorldColumn;
        return WorldAreaManager.MakeUniqueID(subWorldOffsetX, subWorldOffsetY, subWorldOffsetZ);
    }

}
