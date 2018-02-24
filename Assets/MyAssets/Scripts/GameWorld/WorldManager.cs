using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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


    private List<World> _worldList = new List<World>();
    public List<World> worldList
    {
        get { return _worldList; }
    }
    private int maxSubWorld = 0;

    public static WorldManager instance;

    public void Init()
    {
        CreateGameWorld();
        instance = this;
    }
    /// <summary>
    /// C# sereialization 기능을 이용한
    /// subWorld를 외부파일로 저장하는 메소드.
    /// 아직 테스트중.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="data"></param>
    /// <param name="idx"></param>
    private void SaveSubWorldFile(string fileName, Block[,,] data, int idx)
    {
        Directory.CreateDirectory(ConstFilePath.RAW_SUB_WORLD_DATA_PATH);
        string savePath = string.Format(ConstFilePath.RAW_SUB_WORLD_DATA_PATH +"{0}", fileName);
        // 파일 생성.
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);

        WorldDataFile dataFile = new WorldDataFile();
        dataFile.blockData = data;
        dataFile.idx = idx;
        // 시리얼라이징.
        bf.Serialize(fileStream, dataFile);
        fileStream.Close();
    }

    private void CreateGameWorld()
    {
        maxSubWorld = SubWorldDataFile.instance.maxSubWorld;
        for (int idx = 0; idx < maxSubWorld; ++idx)
        {
            int subWorldPosX = SubWorldDataFile.instance.GetPosValue(idx, "X") * GameConfig.subWorldX;
            int subWorldPosZ = SubWorldDataFile.instance.GetPosValue(idx, "Z") * GameConfig.subWorldZ;
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
            //SaveSubWorldFile(subWorld.name, subWorld.worldBlockData, idx);
        }
    }
    /// <summary>
    /// 주어진 위치값으로 어느 subWorld에 포함되어있는지 확인 후 해당 World를 리턴.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public World ContainedWorld(Vector3 pos)
    {
        int x = (int)pos.x / GameConfig.subWorldX;
        int z = ((int)pos.z / GameConfig.subWorldZ) * SubWorldDataFile.instance.rowOffset;

        return _worldList[x+z];
    }
}
