using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 외부파일로 저장하게되는 World info.
/// </summary>
/// https://stackoverflow.com/questions/27932876/vector3-not-serializable-unity3d
[Serializable]
public class SubWorldExternalDataFile
{
    public int WorldDataArrayIndex;
    public string UniqueID;  
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
    NotLoaded,
    Loading, // 로딩중.
    LoadFinish, // 로딩완료.
    Release, // 해제중.
    ReleaseFinish // 해제완료.
}

public class WorldArea : MonoBehaviour
{
    public string AreaName { get; private set; }
    public string AreaUniqueID { get; private set; }
    // WorldArea 위치값( == 오프셋값).
    public Vector3 OffsetCoordinate { get; private set; }
    // 실제 게임오브젝트로서 존재하는 위치값.
    public Vector3 RealCoordinate { get; private set; }

    public Dictionary<string, SubWorldState> SubWorldStates { get; } = new Dictionary<string, SubWorldState>();

    public void Init(WorldAreaData worldAreaData)
    {
        KojeomLogger.DebugLog("WorldArea 생성을 시작합니다.");
        AreaName = worldAreaData.AreaName;
        AreaUniqueID = worldAreaData.UniqueID;
        OffsetCoordinate = new Vector3(worldAreaData.OffsetX, worldAreaData.OffsetY, worldAreaData.OffsetZ);
        RealCoordinate = OffsetCoordinate;
        //
        CreateArea(worldAreaData);
        if (GameSupervisor.Instance != null && GameSupervisor.Instance.bSubWorldDataSave == true)
        {
            SaveAllSubWorld();
        }
        StartCoroutine(DynamicSubWorldLoader());
        KojeomLogger.DebugLog("WorldArea 생성을 종료합니다.");
    }
 
    /// <summary>
    /// C# sereialization 기능을 이용한
    /// subWorld를 외부파일로 저장하는 메소드.
    /// </summary>
    private void SaveAllSubWorld()
    {
        Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
        int idx = 0;
        foreach(var element in SubWorldStates)
        {
            string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", element.Value.SubWorldInstance.WorldName);
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

            SubWorldExternalDataFile dataFile = new SubWorldExternalDataFile
            {
                BlockData = element.Value.SubWorldInstance.WorldBlockData,
                UniqueID = element.Value.SubWorldInstance.UniqueID
            };
            // 시리얼라이징.
            bf.Serialize(fileStream, dataFile);
            fileStream.Close();
            //
            idx++;
        }
    }

    private async Task<bool> SaveSpecificSubWorld(string uniqueID)
    {
        return await Task.Run(() => {
            Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);

            SubWorldStates.TryGetValue(uniqueID, out SubWorldState state);
            string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", state.SubWorldInstance.WorldName);
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate, FileAccess.Write);

            SubWorldExternalDataFile dataFile = new SubWorldExternalDataFile
            {
                BlockData = state.SubWorldInstance.WorldBlockData,
                UniqueID = state.SubWorldInstance.UniqueID
            };
            // 시리얼라이징.
            bf.Serialize(fileStream, dataFile);
            fileStream.Close();
            return true;
        });
    }

    public async void SaveAsyncSubWorldFile(string uniqueID)
    {
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Start and Waiting Async Save.", uniqueID));
        var isSaveSuccess = await SaveSpecificSubWorld(uniqueID);
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} End Waiting Async Save.", uniqueID));
    }

    /// <summary>
    /// 특정 idx를 가진 서브월드를 로드합니다.
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <returns></returns>
    private async Task<SubWorldExternalDataFile> LoadSubWorldFile(string uniqueID)
    {
        // deserializing.
        return await Task.Run(()=> {
            SubWorldStates.TryGetValue(uniqueID, out SubWorldState worldState);
            string fileName = worldState.SubWorldInstance.WorldName;
            string filePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", fileName);
            // 파일 열기.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Read);
            var worldData = bf.Deserialize(fileStream) as SubWorldExternalDataFile;
            fileStream.Close();
            return worldData;
        });
    }

    public async void LoadAsyncSubWorldFile(string uniqueID)
    {
        // 로딩 상태로 전환.
        SubWorldStates.TryGetValue(uniqueID, out SubWorldState worldState);
        worldState.RealTimeStatus = SubWorldRealTimeStatus.Loading;
        // 실제 파일을 로딩.
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Start and Waiting Async Load from file.", uniqueID));
        var loadedWorldData = await LoadSubWorldFile(uniqueID);
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Finish Waiting Async Load from file.", uniqueID));
        // 읽어들인 파일을 이용해 게임에서 로딩.
        worldState.SubWorldInstance.LoadSynchro(loadedWorldData.BlockData);
    }

    private void CreateArea(WorldAreaData worldAreaData)
    {
        foreach(var subWorldData in worldAreaData.SubWorldDataList)
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
                RealTimeStatus = SubWorldRealTimeStatus.NotLoaded
            };
            SubWorldStates.Add(subWorldData.UniqueID, worldState);
        }
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
        //
        //NPC 생성 테스트. 
        if(worldState.SubWorldInstance.bSurfaceWorld == true)
        {
            ActorSuperviosr.Instance.RequestSpawnRandomNPC(NPC_TYPE.Merchant, subWorldUniqueID, AreaUniqueID, 1, true);
        }
    }

    private void OnReleaseSubWorldInstance(string uniqueID)
    {
        switch(SubWorldStates[uniqueID].RealTimeStatus)
        {
            //case WorldRealTimeStatus.Loading:
            case SubWorldRealTimeStatus.LoadFinish:
                // 릴리즈 상태로 전환.
                SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Release;
                // 메모리 해제 직전, Sub WorldData를 외부 파일로 저장.
                SaveAsyncSubWorldFile(uniqueID);
                // 메모리 해제 시작.
                SubWorldStates[uniqueID].SubWorldInstance.Release();
                KojeomLogger.DebugLog(string.Format("Subworld ID : {0} is Start Release. ", uniqueID));
                break;
        }
    }

    private void OnFinishReleaseSubWorldInstance(string uniqueID)
    {
        SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.ReleaseFinish;
    }

    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public SubWorld ContainedSubWorld(Vector3 pos)
    {
        var state = SubWorldStates[WorldAreaManager.GetSubWorldUniqueID(pos)];
        if(state == null)
        {
            return null;
        }
        return state.SubWorldInstance;
    }

    private IEnumerator DynamicSubWorldLoader()
    {
        KojeomLogger.DebugLog("DynamicSubWorldLoader Co-Routine Start.");
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        while (true)
        {
            // to do
            Vector3 playerPos = Vector3.zero;
            Vector3 offsetPos = Vector3.zero;
            if(GamePlayerManager.Instance != null && GamePlayerManager.Instance.IsInitializeFinish == true)
            {
                playerPos = GamePlayerManager.Instance.MyGamePlayer.Controller.GetPosition();
                offsetPos = SubWorldStates[WorldAreaManager.GetSubWorldUniqueID(playerPos)].SubWorldInstance.SubWorldOffsetCoordinate;
            }
            else
            {
                foreach(var state in SubWorldStates)
                {
                    if(state.Value.SubWorldInstance != null && state.Value.SubWorldInstance.bSurfaceWorld == true)
                    {
                        offsetPos = state.Value.SubWorldInstance.SubWorldOffsetCoordinate;
                        break;
                    }
                }
            }

            // 플레이어가 위치한 서브월드의 offset 위치를 기준삼아
            // 8방향(대각선, 좌우상하)의 subWorld를 활성화 시킨다. 
            // 플레이어 주변을 넘어서는 그 바깥의 영역들은 외부 파일로 저장시키는걸 비동기로..
            // 
            for (int x = (int)offsetPos.x - 1; x <= (int)offsetPos.x + 1; x++)
            {
                for (int y = (int)offsetPos.y - 1; y <= (int)offsetPos.y + 1; y++)
                {
                    for (int z = (int)offsetPos.z - 1; z <= (int)offsetPos.z + 1; z++)
                    {
                        string uniqueID = WorldAreaManager.MakeUniqueID(x, y, z);
                        if(SubWorldStates.ContainsKey(uniqueID) == true)
                        {
                            switch (SubWorldStates[uniqueID].RealTimeStatus)
                            {
                                case SubWorldRealTimeStatus.None:
                                    // nothing to do.
                                    break;
                                case SubWorldRealTimeStatus.NotLoaded:
                                    SubWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Loading;
                                    SubWorldStates[uniqueID].SubWorldInstance.LoadSynchro();
                                    break;
                                case SubWorldRealTimeStatus.ReleaseFinish:
                                    LoadAsyncSubWorldFile(uniqueID);
                                    break;
                            }
                        }
                    }
                }
            }
            yield return null;
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
    

}
