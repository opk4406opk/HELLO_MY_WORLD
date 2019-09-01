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

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    private Transform WorldGroupTrans;
    public static WorldManager Instance { get; private set; }

    public Dictionary<string, SubWorldState> WholeWorldStates { get; } = new Dictionary<string, SubWorldState>();

    public void Init()
    {
        KojeomLogger.DebugLog("GameWorld 생성을 시작합니다.");
        CreateWholeWorld();
        if (GameSupervisor.Instance != null && GameSupervisor.Instance.IsSubWorldDataSave == true)
        {
            SaveAllSubWorld();
        }
        StartCoroutine(DynamicSubWorldLoader());
        KojeomLogger.DebugLog("GameWorld 생성을 종료합니다.");
        Instance = this;
    }
 
    /// <summary>
    /// C# sereialization 기능을 이용한
    /// subWorld를 외부파일로 저장하는 메소드.
    /// </summary>
    private void SaveAllSubWorld()
    {
        Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
        int idx = 0;
        foreach(var element in WholeWorldStates)
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

            WholeWorldStates.TryGetValue(uniqueID, out SubWorldState state);
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
            WholeWorldStates.TryGetValue(uniqueID, out SubWorldState worldState);
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
        WholeWorldStates.TryGetValue(uniqueID, out SubWorldState worldState);
        worldState.RealTimeStatus = SubWorldRealTimeStatus.Loading;
        // 실제 파일을 로딩.
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Start and Waiting Async Load from file.", uniqueID));
        var loadedWorldData = await LoadSubWorldFile(uniqueID);
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Finish Waiting Async Load from file.", uniqueID));
        // 읽어들인 파일을 이용해 게임에서 로딩.
        worldState.SubWorldInstance.LoadSynchro(loadedWorldData.BlockData);
    }

    private void CreateWholeWorld()
    {
        var gameConfig = GameConfigDataFile.Instance.GetGameConfigData();
        foreach(var subWorldData in WorldMapDataFile.instance.WorldMapData.SubWorldDatas)
        {
            SubWorld subWorld = MakeNewWorldInstance();
            subWorld.OnFinishLoadChunks += OnFinishSubWorldLoadChunks;
            subWorld.OnReadyToRelease += OnReleaseSubWorldInstance;
            subWorld.OnFinishRelease += OnFinishReleaseSubWorldInstance;
            subWorld.Init(subWorldData);
            //add world.
            SubWorldState worldState = new SubWorldState
            {
                SubWorldInstance = subWorld,
                RealTimeStatus = SubWorldRealTimeStatus.NotLoaded
            };
            WholeWorldStates.Add(subWorldData.UniqueID, worldState);
        }
    }

    private SubWorld MakeNewWorldInstance()
    {
        GameObject newSubWorld = Instantiate(GameResourceSupervisor.GetInstance().WorldPrefab.LoadSynchro(), new Vector3(0, 0, 0),
               new Quaternion(0, 0, 0, 0)) as GameObject;
        newSubWorld.transform.parent = WorldGroupTrans;
        //
        return newSubWorld.GetComponent<SubWorld>();
    }

    private void OnFinishSubWorldLoadChunks(string uniqueID)
    {
        WholeWorldStates.TryGetValue(uniqueID, out SubWorldState worldState);
        worldState.RealTimeStatus = SubWorldRealTimeStatus.LoadFinish;
        //
        //NPC 생성 테스트. 
        if(worldState.SubWorldInstance.bSurfaceWorld == true)
        {
            ActorSuperviosr.Instance.RequestSpawnRandomNPC(NPC_TYPE.Merchant, uniqueID, 1, true);
        }
    }

    private void OnReleaseSubWorldInstance(string uniqueID)
    {
        switch(WholeWorldStates[uniqueID].RealTimeStatus)
        {
            //case WorldRealTimeStatus.Loading:
            case SubWorldRealTimeStatus.LoadFinish:
                // 릴리즈 상태로 전환.
                WholeWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Release;
                // 메모리 해제 직전, Sub WorldData를 외부 파일로 저장.
                SaveAsyncSubWorldFile(uniqueID);
                // 메모리 해제 시작.
                WholeWorldStates[uniqueID].SubWorldInstance.Release();
                KojeomLogger.DebugLog(string.Format("Subworld ID : {0} is Start Release. ", uniqueID));
                break;
        }
    }

    private void OnFinishReleaseSubWorldInstance(string uniqueID)
    {
        WholeWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.ReleaseFinish;
    }

    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public SubWorld ContainedWorld(Vector3 pos)
    {
        var state = WholeWorldStates[GetSubWorldUniqueID(pos)];
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
                offsetPos = WholeWorldStates[GetSubWorldUniqueID(playerPos)].SubWorldInstance.WorldOffsetCoordinate;
            }
            else
            {
                foreach(var state in WholeWorldStates)
                {
                    if(state.Value.SubWorldInstance != null && state.Value.SubWorldInstance.bSurfaceWorld == true)
                    {
                        offsetPos = state.Value.SubWorldInstance.WorldOffsetCoordinate;
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
                        string uniqueID = MakeUniqueID(x, y, z);
                        if(WholeWorldStates.ContainsKey(uniqueID) == true)
                        {
                            switch (WholeWorldStates[uniqueID].RealTimeStatus)
                            {
                                case SubWorldRealTimeStatus.None:
                                    // nothing to do.
                                    break;
                                case SubWorldRealTimeStatus.NotLoaded:
                                    WholeWorldStates[uniqueID].RealTimeStatus = SubWorldRealTimeStatus.Loading;
                                    WholeWorldStates[uniqueID].SubWorldInstance.LoadSynchro();
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
    /// 월드 Name으로부터 Data Array Index를 추출.
    /// </summary>
    /// <param name="worldName"></param>
    /// <returns></returns>
    public static int MakeWorldDataArrIndex(string worldName)
    {
        var splits = worldName.Split('_');
        return int.Parse(splits[2]);
    }
    /// <summary>
    /// UniqueID를 생성합니다.
    /// </summary>
    /// <param name="xyz"></param>
    /// <returns></returns>
    public static string MakeUniqueID(Vector3 pos)
    {
        // basic form : unique_0:0:0  ( x:y:z )
        return string.Format("unique_{0}:{1}:{2}", pos.x, pos.y, pos.z);
    }

    /// <summary>
    /// UniqueID를 생성합니다.
    /// </summary>
    /// <param name="xyz"></param>
    /// <returns></returns>
    public static string MakeUniqueID(int x, int y, int z)
    {
        // basic form : unique_0:0:0  ( x:y:z )
        return string.Format("unique_{0}:{1}:{2}", x, y, z);
    }

    public static Vector3 DisassembleUniqueID(string uniqueID)
    {
        var sub = uniqueID.Substring(uniqueID.IndexOf("_"));
        var split = sub.Split(':');
        return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
    }

    /// <summary>
    /// 오브젝트 위치를 통해 어느 SubWorld에 위치했는지 확인 후 해당 World UniqueID를 리턴.
    /// </summary>
    /// <param name="objectPos">오브젝트 위치</param>
    /// <returns></returns>
    public string GetSubWorldUniqueID(Vector3 objectPos)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int x = (Mathf.CeilToInt(objectPos.x) / gameWorldConfig.sub_world_x_size) % WorldMapDataFile.instance.WorldMapData.Row;
        int y = (Mathf.CeilToInt(objectPos.y) / gameWorldConfig.sub_world_y_size) % WorldMapDataFile.instance.WorldMapData.Layer;
        int z = (Mathf.CeilToInt(objectPos.z) / gameWorldConfig.sub_world_z_size) % WorldMapDataFile.instance.WorldMapData.Column;

        return MakeUniqueID(x, y, z);
    }

    /// <summary>
    /// 게임 속 실제 좌표(=Real) 값을 월드배열 좌표값으로 변환.
    /// </summary>
    /// <param name="objectPos"></param>
    /// <returns></returns>
    public Vector3 GetRealCoordToWorldCoord(Vector3 objectPos)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int x = (Mathf.CeilToInt(objectPos.x) / gameWorldConfig.sub_world_x_size) % WorldMapDataFile.instance.WorldMapData.Row;
        int y = (Mathf.CeilToInt(objectPos.y) / gameWorldConfig.sub_world_y_size) % WorldMapDataFile.instance.WorldMapData.Layer;
        int z = (Mathf.CeilToInt(objectPos.z) / gameWorldConfig.sub_world_z_size) % WorldMapDataFile.instance.WorldMapData.Column;
        return new Vector3(x, y, z);
    }

    public Vector3 GetWorldCoordToRealCoord(Vector3 worldCoord)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int x = (Mathf.CeilToInt(worldCoord.x) * gameWorldConfig.sub_world_x_size) * WorldMapDataFile.instance.WorldMapData.Row;
        int y = (Mathf.CeilToInt(worldCoord.y) * gameWorldConfig.sub_world_y_size) * WorldMapDataFile.instance.WorldMapData.Layer;
        int z = (Mathf.CeilToInt(worldCoord.z) * gameWorldConfig.sub_world_z_size) * WorldMapDataFile.instance.WorldMapData.Column;
        return new Vector3(x, y, z);
    }

}
