using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldAreaManager : MonoBehaviour
{
    private struct WorldAreaGenerateParam
    {
        public int AreaSizeX;
        public int AreaSizeZ;
    }
    public Dictionary<string, WorldArea> WorldAreas { get; private set; } = new Dictionary<string, WorldArea>();
    public static WorldAreaManager Instance { get; private set; }
    public bool bFinishLoad { get; private set; }

    public async void Init()
    {
        //
        Instance = this;
        //
        bFinishLoad = false;
        List<WorldAreaGenerateParam> worldAreaGenParamGroup = new List<WorldAreaGenerateParam>();
        foreach (var worldAreaData in WorldMapDataFile.Instance.MapData.WorldAreaDatas)
        {
            var worldConfig = WorldConfigFile.Instance.GetConfig();
            var worldMapData = WorldMapDataFile.Instance.MapData;
            int worldAreaSizeX = worldMapData.SubWorldRow * worldConfig.SubWorldSizeX;
            int worldAreaSizeZ = worldMapData.SubWorldColumn * worldConfig.SubWorldSizeZ;
            WorldAreaGenerateParam param;
            param.AreaSizeX = worldAreaSizeX;
            param.AreaSizeZ = worldAreaSizeZ;
            worldAreaGenParamGroup.Add(param);
        }
        // 모든 비동기 맵 데이터 생성이 완료되기를 기다린다.
        var mapData = await AsyncGenerateAreaMapDatas(worldAreaGenParamGroup);
        // 완료되면, 월드 아레아를 생성하며 해당 맵 데이터를 설정.
        int idx = 0;
        List<WorldArea> areaList = new List<WorldArea>();
        foreach (var worldAreaData in WorldMapDataFile.Instance.MapData.WorldAreaDatas)
        {
            GameObject newWorldArea = Instantiate(GameResourceSupervisor.GetInstance().WorldAreaPrefab.LoadSynchro(), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
            newWorldArea.transform.parent = transform;
            newWorldArea.gameObject.name = worldAreaData.AreaName;
            WorldArea worldAreaInstance = newWorldArea.GetComponent<WorldArea>();
            worldAreaInstance.Init(worldAreaData, mapData[idx], false);
            WorldAreas.Add(worldAreaData.UniqueID, worldAreaInstance);
            areaList.Add(worldAreaInstance);
            idx++;
        }
        // 여러개의 WorldArea중에 1개만 골라, 맵 로딩을 시킨다. ( 이후에 플레이어 생성.)
        int randIndex = KojeomUtility.RandomInteger(0, areaList.Count);
        string selectedAreaID = areaList[randIndex].AreaUniqueID;
        foreach (var keyValuePair in areaList[randIndex].SubWorldStates)
        {
            SubWorldState subWorldState = keyValuePair.Value;
            if (subWorldState.RealTimeStatus == SubWorldRealTimeStatus.ReadyToFirstLoad)
            {
                subWorldState.RealTimeStatus = SubWorldRealTimeStatus.Loading;
                subWorldState.SubWorldInstance.AsyncLoading(null, true, ()=> {
                    if (GamePlayerManager.Instance != null)
                    {
                        if (GamePlayerManager.Instance.bInitialize == false)
                        {
                            Vector3 randPos = subWorldState.SubWorldInstance.GetRandomRealPositionAtSurface();
                            GamePlayerManager.Instance.Make(randPos, ()=> {
                                SwitchAllAreaDynamicWorldLoader(true);
                            });
                        }
                    }
                });
            }
        }

        bFinishLoad = true;
    }

    /// <summary>
    /// 모든 월드 아레아의 서브월드 Loader를 Enable/Disable
    /// </summary>
    /// <param name="enable"></param>
    public void SwitchAllAreaDynamicWorldLoader(bool enable)
    {
        foreach(var keyValuePair in WorldAreas)
        {
            WorldArea area = keyValuePair.Value;
            if(area != null)
            {
                area.bRunDynamicLoader = enable;
            }
        }
    }

    private async Task<List<WorldGenAlgorithms.TerrainValue[,]>> AsyncGenerateAreaMapDatas(List<WorldAreaGenerateParam> paramGroup)
    {
        return await Task.Run(() => {
            List<WorldGenAlgorithms.TerrainValue[,]> mapDatas = new List<WorldGenAlgorithms.TerrainValue[,]>();
            foreach(var param in paramGroup)
            {
                mapDatas.Add(WorldGenAlgorithms.GenerateNormalTerrain(param.AreaSizeX, param.AreaSizeZ,
                             WorldMapDataFile.Instance.MapData.SubWorldLayer,
                             WorldConfigFile.Instance.GetConfig().SubWorldSizeY, 900));
            }
            return mapDatas;    
        });
    }

    public WorldArea GetWorldArea(string worldAreaUniqueID)
    {
        return WorldAreas[worldAreaUniqueID];
    }

    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public SubWorld ContainedSubWorld(Vector3 pos)
    {
        var worldArea = WorldAreas[GetWorldAreaUniqueID(pos)];
        if(worldArea != null)
        {
            var subWorldInstance = worldArea.ContainedSubWorld(pos);
            if (subWorldInstance == null)
            {
                return null;
            }
            return subWorldInstance;
        }
        return null;
    }

    public SubWorld ContainedSubWorld(GamePlayerController playerController)
    {
        var subWorldInstance = WorldAreas[GetWorldAreaUniqueID(playerController)].ContainedSubWorld(playerController.GetPosition());
        if (subWorldInstance == null)
        {
            return null;
        }
        return subWorldInstance;
    }

    public SubWorld ContainedSubWorld(ActorController actorController)
    {
        var subWorldInstance = WorldAreas[GetWorldAreaUniqueID(actorController)].ContainedSubWorld(actorController.GetPosition());
        if (subWorldInstance == null)
        {
            return null;
        }
        return subWorldInstance;
    }

    public WorldArea ContainedWorldArea(Vector3 pos)
    {
        var worldAreaInstnace = WorldAreas[GetWorldAreaUniqueID(pos)];
        if (worldAreaInstnace == null)
        {
            return null;
        }
        return worldAreaInstnace;
    }

    public static string GetWorldAreaUniqueID(GamePlayerController playerController)
    {
        if(playerController.CharacterInstance.ContainedWorld != null)
        {
            return playerController.CharacterInstance.ContainedWorld.GetWorldAreaUniqueID();
        }
        return "";
    }

    public static string GetWorldAreaUniqueID(ActorController actorController)
    {
        if(actorController.GetContainedWorld() != null)
        {
            return actorController.GetContainedWorld().GetWorldAreaUniqueID();
        }
        return "";
    }

    public static string GetWorldAreaUniqueID(Vector3 objectPos)
    {
        //
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int areaOffsetX = Mathf.CeilToInt(Mathf.CeilToInt(objectPos.x) / gameWorldConfig.SubWorldSizeX) / WorldMapDataFile.Instance.MapData.WorldAreaRow;
        int areaOffsetY = Mathf.CeilToInt(Mathf.CeilToInt(objectPos.y) / gameWorldConfig.SubWorldSizeY) / WorldMapDataFile.Instance.MapData.WorldAreaLayer;
        int areaOffsetZ = Mathf.CeilToInt(Mathf.CeilToInt(objectPos.z) / gameWorldConfig.SubWorldSizeZ) / WorldMapDataFile.Instance.MapData.WorldAreaColumn;
        // 테스트 코드........
        areaOffsetX = Mathf.Clamp(areaOffsetX, 0, WorldMapDataFile.Instance.MapData.WorldAreaRow - 1);
        areaOffsetY = Mathf.Clamp(areaOffsetY, 0, WorldMapDataFile.Instance.MapData.WorldAreaLayer - 1);
        areaOffsetZ = Mathf.Clamp(areaOffsetZ, 0, WorldMapDataFile.Instance.MapData.WorldAreaColumn - 1);
        //..................
        return MakeUniqueID(areaOffsetX, areaOffsetY, areaOffsetZ);
    }

    /// <summary>
    /// 게임 속 실제 좌표(=Real) 값을 월드배열 인덱스값으로 변환.
    /// </summary>
    /// <param name="objectPos"></param>
    /// <returns></returns>
    public static Vector3 GetRealCoordToWorldDataCoord(Vector3 objectPos)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int ceilPosX = Mathf.CeilToInt(objectPos.x);
        int ceilPosY = Mathf.CeilToInt(objectPos.y);
        int ceilPosZ = Mathf.CeilToInt(objectPos.z);
        // offset
        int offsetX = ceilPosX / gameWorldConfig.SubWorldSizeX;
        int offsetY = ceilPosY / gameWorldConfig.SubWorldSizeY;
        int offsetZ = ceilPosZ / gameWorldConfig.SubWorldSizeZ;
        // calc..
        int x = ceilPosX - (offsetX * gameWorldConfig.SubWorldSizeX * WorldMapDataFile.Instance.MapData.WorldAreaRow);
        int y = ceilPosY - (offsetY * gameWorldConfig.SubWorldSizeY * WorldMapDataFile.Instance.MapData.WorldAreaLayer);
        int z = ceilPosZ - (offsetZ * gameWorldConfig.SubWorldSizeZ * WorldMapDataFile.Instance.MapData.WorldAreaColumn);
        // clamp
        x = Mathf.Clamp(x, 0, gameWorldConfig.SubWorldSizeX);
        x = Mathf.Clamp(x, 0, gameWorldConfig.SubWorldSizeY);
        x = Mathf.Clamp(x, 0, gameWorldConfig.SubWorldSizeZ);
        return new Vector3(x, y, z);
    }
    
    /// <summary>
    /// 월드배열 인덱스값을 게임 속 실제 좌표(=Real)로 변환.
    /// </summary>
    /// <param name="worldCoord"></param>
    /// <returns></returns>
    public static Vector3 GetWorldDataCoordToRealCoord(Vector3 worldCoord)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int ceilPosX = Mathf.CeilToInt(worldCoord.x);
        int ceilPosY = Mathf.CeilToInt(worldCoord.y);
        int ceilPosZ = Mathf.CeilToInt(worldCoord.z);
        // offset
        int offsetX = ceilPosX / gameWorldConfig.SubWorldSizeX;
        int offsetY = ceilPosY / gameWorldConfig.SubWorldSizeY;
        int offsetZ = ceilPosZ / gameWorldConfig.SubWorldSizeZ;
        //
        int x = ceilPosX + (offsetX * gameWorldConfig.SubWorldSizeX * WorldMapDataFile.Instance.MapData.WorldAreaRow);
        int y = ceilPosY + (offsetY * gameWorldConfig.SubWorldSizeY * WorldMapDataFile.Instance.MapData.WorldAreaLayer);
        int z = ceilPosZ + (offsetZ * gameWorldConfig.SubWorldSizeZ * WorldMapDataFile.Instance.MapData.WorldAreaColumn);
        return new Vector3(x, y, z);
    }
    /// <summary>
    /// 블록 인덱스를 청크 인덱스로 변환.
    /// </summary>
    /// <param name="blockIndex"></param>
    /// <returns></returns>
    public static Vector3 ConvertBlockIdxToChunkIdx(Vector3 blockIndex)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        return new Vector3(blockIndex.x / gameWorldConfig.ChunkSize, blockIndex.y / gameWorldConfig.ChunkSize, blockIndex.z / gameWorldConfig.ChunkSize);
    }
    /// <summary>
    /// 블록 인덱스를 청크 인덱스로 변환.
    /// </summary>
    /// <param name="blockIndex"></param>
    /// <returns></returns>
    public static Vector3 ConvertBlockIdxToChunkIdx(int x, int y, int z)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        return new Vector3(x / gameWorldConfig.ChunkSize, y / gameWorldConfig.ChunkSize, z / gameWorldConfig.ChunkSize);
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
        // basic form : unique_0:0:0  ( x:y:z )
        var sub = uniqueID.Substring(uniqueID.IndexOf("_") + 1);
        var split = sub.Split(':');
        return new Vector3(float.Parse(split[0]), float.Parse(split[1]), float.Parse(split[2]));
    }

}
