using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class WorldAreaManager : MonoBehaviour
{
    public Dictionary<string, WorldArea> WorldAreas { get; private set; } = new Dictionary<string, WorldArea>();
    public static WorldAreaManager Instance { get; private set; }
    public void Init()
    {
        //
        Instance = this;
        //
        foreach(var worldAreaData in WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaDatas)
        {
            AsyncGenerateArea(worldAreaData);
        }
    }

    private async void AsyncGenerateArea(WorldAreaTerrainData worldAreaData)
    {
        var worldConfig = WorldConfigFile.Instance.GetConfig();
        var worldMapData = WorldMapDataFile.Instance.WorldMapDataInstance;
        int worldAreaSizeX = worldMapData.SubWorldRow * worldConfig.SubWorldSizeX;
        int worldAreaSizeZ = worldMapData.SubWorldColumn * worldConfig.SubWorldSizeZ;
        //
        GameObject newWorldArea = Instantiate(GameResourceSupervisor.GetInstance().WorldAreaPrefab.LoadSynchro(), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;
        newWorldArea.transform.parent = transform;
        newWorldArea.gameObject.name = worldAreaData.AreaName;
        //
        KojeomLogger.DebugLog(string.Format("WorldArea ID : {0} ({1} * {2}) Start Generate Area.", worldAreaData.UniqueID, worldAreaSizeX, worldAreaSizeZ));
        var terrainValueData = await TaskGenerateAreaData(worldAreaSizeX, worldAreaSizeZ);
        //
        WorldArea worldAreaInstance = newWorldArea.GetComponent<WorldArea>();
        worldAreaInstance.Init(worldAreaData, terrainValueData);
        WorldAreas.Add(worldAreaData.UniqueID, worldAreaInstance);
        KojeomLogger.DebugLog(string.Format("WorldArea ID : {0} ({1} * {2}) Finish Generate Area.", worldAreaData.UniqueID, worldAreaSizeX, worldAreaSizeZ));
    }

    private async Task<WorldGenAlgorithms.TerrainValue[,]> TaskGenerateAreaData(int areaSizeX, int areaSizeZ)
    {
        return await Task.Run(() => {
            WorldGenAlgorithms.TerrainValue[,] map = WorldGenAlgorithms.GenerateNormalTerrain(areaSizeX, areaSizeZ, 900);
            return map;
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
        var subWorldInstance = WorldAreas[GetWorldAreaUniqueID(pos)].ContainedSubWorld(pos);
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


    public static string GetWorldAreaUniqueID(Vector3 objectPos)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int x = (Mathf.CeilToInt(objectPos.x) / gameWorldConfig.SubWorldSizeX) % WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaRow;
        int y = (Mathf.CeilToInt(objectPos.y) / gameWorldConfig.SubWorldSizeY) % WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaLayer;
        int z = (Mathf.CeilToInt(objectPos.z) / gameWorldConfig.SubWorldSizeZ) % WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaColumn;
        return MakeUniqueID(x, y, z);
    }

    /// <summary>
    /// 게임 속 실제 좌표(=Real) 값을 월드배열 좌표값으로 변환.
    /// </summary>
    /// <param name="objectPos"></param>
    /// <returns></returns>
    public static Vector3 GetRealCoordToWorldCoord(Vector3 objectPos)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int x = (Mathf.CeilToInt(objectPos.x) / gameWorldConfig.SubWorldSizeX) % WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldRow % WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaRow;
        int y = (Mathf.CeilToInt(objectPos.y) / gameWorldConfig.SubWorldSizeY) % WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldLayer % WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaLayer;
        int z = (Mathf.CeilToInt(objectPos.z) / gameWorldConfig.SubWorldSizeZ) % WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldColumn % WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaColumn;
        return new Vector3(x, y, z);
    }

    public static Vector3 GetWorldCoordToRealCoord(Vector3 worldCoord)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int x = (Mathf.CeilToInt(worldCoord.x) * gameWorldConfig.SubWorldSizeX) * WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldRow * WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaRow;
        int y = (Mathf.CeilToInt(worldCoord.y) * gameWorldConfig.SubWorldSizeY) * WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldLayer * WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaLayer;
        int z = (Mathf.CeilToInt(worldCoord.z) * gameWorldConfig.SubWorldSizeZ) * WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldColumn * WorldMapDataFile.Instance.WorldMapDataInstance.WorldAreaColumn;
        return new Vector3(x, y, z);
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

}
