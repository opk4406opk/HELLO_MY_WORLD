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
public class WorldDataFile
{
    public int worldDataArrayIndex;
    public string uniqueID;
    public Block[,,] blockData;
}

// 비동기 처리에 대한 참고 문서.
// http://www.stevevermeulen.com/index.php/2017/09/using-async-await-in-unity3d-2017/
// https://tech.peoplefund.co.kr/2017/08/02/non-blocking-asynchronous-concurrency.html
// https://blog.stephencleary.com/2012/02/async-and-await.html

public struct SubWorldNormalizedOffset
{
    SubWorldNormalizedOffset(int x, int y, int z) { this.x = x; this.y = y; this.z = z;  }
    public int x;
    public int y;
    public int z;
}
public class WorldState
{
    public World subWorldInstance;
    public WorldRealTimeStatus realTimeStatus = WorldRealTimeStatus.None;
}

public enum WorldRealTimeStatus
{
    None,
    NotLoaded,
    Loading,
    LoadSuccess,
    Released
}

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    private Transform WorldGroupTrans;
    public static WorldManager Instance { get; private set; }

    public Dictionary<string, WorldState> WholeWorldStates { get; } = new Dictionary<string, WorldState>();

    public void Init()
    {
        KojeomLogger.DebugLog("GameWorld 생성을 시작합니다.");
        CreateWholeWorld();
        if (GameSupervisor.Instance != null && GameSupervisor.Instance.isSubWorldDataSave == true)
        {
            SaveAllSubWorld();
        }
        StartCoroutine(DynamicSubWorldLoader());
        KojeomLogger.DebugLog("GameWorld 생성을 종료합니다.");

        //debug
        KojeomLogger.DebugLog(string.Format("seed_val : {0}", P2PNetworkManager.GetGameRandomSeed()), LOG_TYPE.DEBUG_TEST);
        KojeomLogger.DebugLog(string.Format("rand_val : {0}", KojeomUtility.RandomInteger(1, 5)), LOG_TYPE.DEBUG_TEST);
        //
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
            string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", element.Value.subWorldInstance.WorldName);
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

            WorldDataFile dataFile = new WorldDataFile
            {
                blockData = element.Value.subWorldInstance.WorldBlockData,
                uniqueID = element.Value.subWorldInstance.UniqueID
            };
            // 시리얼라이징.
            bf.Serialize(fileStream, dataFile);
            fileStream.Close();
            //
            idx++;
        }
    }

    private void SaveSpecificSubWorld(string uniqueID)
    {
        Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);

        WholeWorldStates.TryGetValue(uniqueID, out WorldState state);
        string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", state.subWorldInstance.WorldName);
        // 파일 생성.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

        WorldDataFile dataFile = new WorldDataFile
        {
            blockData = state.subWorldInstance.WorldBlockData,
            uniqueID = state.subWorldInstance.UniqueID
        };
        // 시리얼라이징.
        bf.Serialize(fileStream, dataFile);
        fileStream.Close();
    }

    /// <summary>
    /// 특정 idx를 가진 서브월드를 로드합니다.
    /// </summary>
    /// <param name="uniqueID"></param>
    /// <returns></returns>
    private async Task<WorldDataFile> LoadSubWorldFile(string uniqueID)
    {
        WholeWorldStates.TryGetValue(uniqueID, out WorldState worldState);
        string fileName = worldState.subWorldInstance.WorldName;
        string filePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", fileName);
        // 파일 열기.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        // deserializing.
        return await Task.FromResult(bf.Deserialize(fileStream) as WorldDataFile);
    }

    public async void LoadAsyncSubWorldFile(string uniqueID)
    {
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} Start and Waiting Async Load from file.", uniqueID));
        var loadedWorldData = await LoadSubWorldFile(uniqueID);
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} End Waiting Async Load from file.", uniqueID));
        WholeWorldStates.TryGetValue(uniqueID, out WorldState worldState);
        worldState.realTimeStatus = WorldRealTimeStatus.Loading;
        worldState.subWorldInstance.LoadSyncro(loadedWorldData.blockData);
    }

    private void CreateWholeWorld()
    {
        var gameConfig = GameConfigDataFile.Instance.GetGameConfigData();
        foreach(var subWorldData in WorldMapDataFile.instance.WorldMapData.SubWorldDatas)
        {
            World subWorld = MakeNewWorldInstance();
            subWorld.OnFinishLoadChunks += OnSubWorldFinishLoadChunks;
            subWorld.OnReadyToUnload += OnReleaseSubWorldInstance;
            subWorld.Init(subWorldData);
            //add world.
            WorldState worldState = new WorldState
            {
                subWorldInstance = subWorld,
                realTimeStatus = WorldRealTimeStatus.NotLoaded
            };
            WholeWorldStates.Add(subWorldData.UniqueID, worldState);
        }
    }

    private World MakeNewWorldInstance()
    {
        GameObject newSubWorld = Instantiate(GameResourceSupervisor.Instance.WorldPrefab.LoadSynchro(), new Vector3(0, 0, 0),
               new Quaternion(0, 0, 0, 0)) as GameObject;
        newSubWorld.transform.parent = WorldGroupTrans;
        //
        return newSubWorld.GetComponent<World>();
    }

    private void OnSubWorldFinishLoadChunks(string uniqueID)
    {
        WholeWorldStates.TryGetValue(uniqueID, out WorldState worldState);
        worldState.realTimeStatus = WorldRealTimeStatus.LoadSuccess;
        //
        //NCP 생성 테스트. 
        ActorSuperviosr.Instance.RequestSpawnRandomNPC(NPC_TYPE.Merchant, uniqueID, 1, true);
    }

    private void OnReleaseSubWorldInstance(string uniqueID)
    {
        switch(WholeWorldStates[uniqueID].realTimeStatus)
        {
            case WorldRealTimeStatus.Loading:
            case WorldRealTimeStatus.LoadSuccess:
                // 메모리 해제 직전, Sub WorldData를 외부 파일로 저장.
                SaveSpecificSubWorld(uniqueID);
                // 메모리 해제 시작.
                WholeWorldStates[uniqueID].subWorldInstance.Release();
                WholeWorldStates[uniqueID].realTimeStatus = WorldRealTimeStatus.Released;
                KojeomLogger.DebugLog(string.Format("Subworld ID : {0} is Released. ", uniqueID));
                break;
        }
    }

    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public World ContainedWorld(Vector3 pos)
    {
        var state = WholeWorldStates[GetSubWorldUniqueID(pos)];
        if(state == null)
        {
            return null;
        }
        return state.subWorldInstance;
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
                offsetPos = WholeWorldStates[GetSubWorldUniqueID(playerPos)].subWorldInstance.WorldCoordinate;
            }
            else
            {
                foreach(var state in WholeWorldStates)
                {
                    if(state.Value.subWorldInstance != null && state.Value.subWorldInstance.IsSurfaceWorld == true)
                    {
                        offsetPos = state.Value.subWorldInstance.WorldCoordinate;
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
                            switch (WholeWorldStates[uniqueID].realTimeStatus)
                            {
                                case WorldRealTimeStatus.None:
                                    // nothing to do.
                                    break;
                                case WorldRealTimeStatus.NotLoaded:
                                    WholeWorldStates[uniqueID].realTimeStatus = WorldRealTimeStatus.Loading;
                                    WholeWorldStates[uniqueID].subWorldInstance.LoadSyncro();
                                    break;
                                case WorldRealTimeStatus.Released:
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

    public static Vector3 DisAssembleUniqueID(string uniqueID)
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
