using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
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

public class WorldManager : MonoBehaviour
{
    [SerializeField]
    private GameObject worldPrefab;
    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private Transform worldGroupTrans;

    private Dictionary<int, string> subWorldFileNameCache= new Dictionary<int, string>();

    private List<World> _worldList = new List<World>();
    public List<World> worldList
    {
        get { return _worldList; }
    }
    private int maxSubWorld = 0;

    public static WorldManager instance;

    public void Init()
    {
        KojeomLogger.DebugLog("GameWorld 생성을 시작합니다.");
        CreateGameWorld();
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
        foreach(var world in _worldList)
        {
            string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", world.worldName);
            // 파일 생성.
            BinaryFormatter bf = new BinaryFormatter();
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

            WorldDataFile dataFile = new WorldDataFile();
            dataFile.blockData = world.worldBlockData;
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
    private WorldDataFile LoadSubWorldFile(int subWorldIdx)
    {
        string fileName;
        subWorldFileNameCache.TryGetValue(subWorldIdx, out fileName);
        string filePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH + "{0}", fileName);
        // 파일 열기.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(filePath, FileMode.OpenOrCreate);
        // deserializing.
        var deserializedData = bf.Deserialize(fileStream);
        return deserializedData as WorldDataFile;
    }

    private void CreateGameWorld()
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
            subWorld.Init(subWorldPosX, subWorldPosZ);
            subWorld.worldName = subWorldName;
            subWorld.idx = idx;
            newSubWorld.transform.parent = worldGroupTrans;
            //add world.
            _worldList.Add(subWorld);
            // 
            subWorldFileNameCache.Add(idx, subWorld.worldName);
        }
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

        return _worldList[x+z];
    }
}
