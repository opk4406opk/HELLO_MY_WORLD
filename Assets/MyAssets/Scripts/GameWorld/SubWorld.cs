﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ChunkSlot
{
    public AChunk[] Chunks = new AChunk[(int)ChunkType.COUNT];
}

public struct MakeWorldParam
{
    public int BaseOffset;
}
/// <summary>
/// 게임내 Sub-월드를 생성 및 관리하는 클래스.
/// </summary>
public class SubWorld : MonoBehaviour
{
    #region event
    public delegate void Del_OnFinishLoadChunks(string uniqueID);
    public event Del_OnFinishLoadChunks OnFinishLoadChunks;

    public delegate void Del_OnReadyToRelease(string uniqueID);
    public event Del_OnReadyToRelease OnReadyToRelease;

    public delegate void Del_OnFinishRelease(string uniqueID);
    public event Del_OnFinishRelease OnFinishRelease;
    #endregion

    public ChunkSlot[,,] ChunkSlots { get; private set; }

    public bool bLoadFinish { get; private set; } = false;
    public bool bTicking { get; private set; } = false;

    #region world infomation.
    public string WorldName { get; private set; }
    public string UniqueID { get; private set; }
    // 월드맵 위치값( == 오프셋값).
    public Vector3 SubWorldOffsetCoordinate { get; private set; }
    // 실제 게임오브젝트로서 존재하는 위치값.
    public Vector3 SubWorldRealCoordinate{ get; private set; }
    public bool bSurfaceWorld { get; private set; }
    #endregion

    public Block[,,] WorldBlockData { get; private set; }
    private int ChunkSize = 0;
    private int ChunkNumber = 0;
    public CustomOctree CustomOctreeInstance { get; private set; } = new CustomOctree();

    private InGameObjectRegister InGameObjRegister;
    //
    private WorldArea WorldAreaInstance;

    public void Init(SubWorldData subWorldData, WorldArea worldArea)
    {
        WorldAreaInstance = worldArea;
        bLoadFinish = false;
        InGameObjRegister = new InGameObjectRegister();
        InGameObjRegister.Initialize();
        // setting to World
        WorldName = subWorldData.WorldName;
        UniqueID = subWorldData.UniqueID;
        SubWorldOffsetCoordinate = new Vector3(subWorldData.OffsetX, subWorldData.OffsetY, subWorldData.OffsetZ);
        var configData = WorldConfigFile.Instance.GetConfig();
        var mapData = WorldMapDataFile.Instance.WorldMapDataInstance;
        float realCoordX = (SubWorldOffsetCoordinate.x * configData.SubWorldSizeX) + (WorldAreaInstance.OffsetCoordinate.x * mapData.SubWorldRow * configData.SubWorldSizeX);
        float realCoordY = (SubWorldOffsetCoordinate.y * configData.SubWorldSizeY) + (WorldAreaInstance.OffsetCoordinate.y * mapData.SubWorldColumn * configData.SubWorldSizeY);
        float realCoordZ = (SubWorldOffsetCoordinate.z * configData.SubWorldSizeZ) + (WorldAreaInstance.OffsetCoordinate.z * mapData.SubWorldLayer * configData.SubWorldSizeZ);
        SubWorldRealCoordinate = new Vector3(realCoordX, realCoordY, realCoordZ);
        bSurfaceWorld = subWorldData.IsSurface;
        // setting to GameObject
        gameObject.name = WorldName;
        // Octree init.
        CustomOctreeInstance.Init(SubWorldRealCoordinate, new Vector3(configData.SubWorldSizeX + SubWorldRealCoordinate.x,
            configData.SubWorldSizeY + SubWorldRealCoordinate.y,
            configData.SubWorldSizeZ + SubWorldRealCoordinate.z));
        //
        bTicking = true;
        StartCoroutine(Tick());
    }

    public Vector3 GetWorldAreaOffset()
    {
        return WorldAreaInstance.OffsetCoordinate;
    }

    private IEnumerator Tick()
    {
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} is Tick Start.", UniqueID));
        while(bTicking)
        {
            if(GamePlayerManager.Instance != null && GamePlayerManager.Instance.IsInitializeFinish == true)
            {
                var curPlayerWorld = WorldAreaInstance.ContainedSubWorld(GamePlayerManager.Instance.MyGamePlayer.Controller.GetPosition());
                if (curPlayerWorld != null)
                {
                    var dist = Mathf.RoundToInt(Vector3.Distance(curPlayerWorld.SubWorldOffsetCoordinate, SubWorldOffsetCoordinate));
                    //KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} away from {1} distance Player contained World",
                    //    UniqueID, dist));
                    // 거리값이 3(테스트용 값) 이상이 되면..Release.
                    if(dist >= 3)
                    {
                        OnReadyToRelease(UniqueID);
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} is Tick Suspended.", UniqueID));
    }

    public void Release()
    {
        bLoadFinish = false;
        StopCoroutine(Tick());
        StopCoroutine(LoadTerrainChunks());
        StartCoroutine(ReleaseProcess());
    }

    private IEnumerator ReleaseProcess()
    {
        //
        foreach (var slot in ChunkSlots)
        {
            if (slot != null)
            {
                for (int type = 0; type < (int)ChunkType.COUNT; type++)
                {
                    if (slot.Chunks[type] != null)
                    {
                        slot.Chunks[type].Release();
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
        ChunkSlots = null;
        WorldBlockData = null;
        // 월드 릴리즈--> 등록된 모든 Actor Hide.
        foreach (var actor in InGameObjRegister.RegisteredActors)
        {
            actor.Hide();
        }
        // 릴리즈 작업이 완료 되었다.
        OnFinishRelease(UniqueID);
    }

    public void LoadSynchro(Block[,,] newBlockData = null)
    {
        bool isReLoaded = false;
        if(newBlockData != null)
        {
            isReLoaded = true;
        }

        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        ChunkSize = gameWorldConfig.ChunkSize;
        // init world data.
        if(newBlockData == null)
        {
            WorldBlockData = new Block[gameWorldConfig.SubWorldSizeX,
            gameWorldConfig.SubWorldSizeY,
            gameWorldConfig.SubWorldSizeZ];
            for (int x = 0; x < gameWorldConfig.SubWorldSizeX; x++)
            {
                for (int z = 0; z < gameWorldConfig.SubWorldSizeZ; z++)
                {
                    for (int y = 0; y < gameWorldConfig.SubWorldSizeY; y++)
                    {
                        WorldBlockData[x, y, z] = new Block
                        {
                            Type = (byte)BlockTileType.EMPTY,
                            bRendered = false
                        };
                    }
                }
            }
        }
        else
        {
            WorldBlockData = newBlockData;
        }
        
        // init chunk group.
        ChunkSlots = new ChunkSlot[Mathf.FloorToInt(gameWorldConfig.SubWorldSizeX / ChunkSize),
            Mathf.FloorToInt(gameWorldConfig.SubWorldSizeY / ChunkSize),
            Mathf.FloorToInt(gameWorldConfig.SubWorldSizeZ / ChunkSize)];
        for (int x = 0; x < ChunkSlots.GetLength(0); x++)
        {
            for (int z = 0; z < ChunkSlots.GetLength(2); z++)
            {
                for (int y = 0; y < ChunkSlots.GetLength(1); y++)
                {
                    ChunkSlots[x, y, z] = new ChunkSlot();
                }
            }
        }

        switch (GameStatus.DetailSingleMode)
        {
            case DetailSingleMode.SAVE_GAME:
                break;
            case DetailSingleMode.EDITOR_TEST_PLAY:
            case DetailSingleMode.LOAD_GAME:
                if(isReLoaded == false)
                {
                    if(bSurfaceWorld == true)
                    {
                        MakeWorldParam param;
                        param.BaseOffset = KojeomUtility.RandomInteger(2, 29);
                        WorldGenAlgorithms.DefaultGenSurfaceSubWorld(WorldBlockData, param);
                    }
                    else
                    {
                        WorldGenAlgorithms.DefaultGenInternalSubWorld(WorldBlockData);
                    }
                   
                }
                break;
        }
        KojeomLogger.DebugLog(string.Format("World name : {0}, Chunk 로드를 시작합니다.", WorldName), LOG_TYPE.DEBUG_TEST);
        StartCoroutine(LoadTerrainChunks());
    }

    //void OnDrawGizmos()
    //{
    //    CustomOctreeInstance.DrawFullTree();
    //}

    private IEnumerator LoadTerrainChunks()
    {
        for (int x = 0; x < ChunkSlots.GetLength(0); x++)
        {
            for (int z = 0; z < ChunkSlots.GetLength(2); z++)
            {
                for (int y = 0; y < ChunkSlots.GetLength(1); y++)
                {
                    for (int type = 0; type < (int)ChunkType.COUNT; type++)
                    {

                        // 유니티엔진에서 제공되는 게임 오브젝트들의 중점(=월드좌표에서의 위치)은
                        // 실제 게임 오브젝트의 정중앙이 된다. 
                        // 따라서, 유니티엔진에 맞춰서 오브젝트의 중점을 정중앙으로 블록들을 생성하려면(= 1개의 블록은 6개의 면을 생성한다),
                        // 아래와 같은 0.5f(offset)값을 추가한다. ( worldCoordX,Y,Z 값은 개별 블록을 생성할 때 사용된다. )
                        // p.s. 이 프로젝트에서 1개의 block의 기준점(block을 생성할 때 쓰이는)은 최상단면의 좌측하단의 포인트가 된다.(디폴트)
                        float chunkRealCoordX = x * ChunkSize - 0.5f;
                        float chunkRealCoordY = y * ChunkSize + 0.5f;
                        float chunkRealCoordZ = z * ChunkSize - 0.5f;

                        GameObject newChunk = null;
                        switch ((ChunkType)type)
                        {
                            case ChunkType.TERRAIN:
                                newChunk = Instantiate(GameResourceSupervisor.GetInstance().TerrainChunkPrefab.LoadSynchro(), new Vector3(0, 0, 0),
                                                           new Quaternion(0, 0, 0, 0)) as GameObject;
                                newChunk.transform.parent = gameObject.transform;
                                newChunk.transform.name = string.Format("TerrainChunk_{0}", ChunkNumber++);
                                ChunkSlots[x, y, z].Chunks[type] = newChunk.GetComponent<TerrainChunk>();
                                break;
                            case ChunkType.WATER:
                                newChunk = Instantiate(GameResourceSupervisor.GetInstance().WaterChunkPrefab.LoadSynchro(), new Vector3(0, 0, 0),
                                                           new Quaternion(0, 0, 0, 0)) as GameObject;
                                newChunk.transform.parent = gameObject.transform;
                                newChunk.transform.name = string.Format("WaterChunk_{0}", ChunkNumber++);
                                ChunkSlots[x, y, z].Chunks[type] = newChunk.GetComponent<WaterChunk>();
                                break;
                            case ChunkType.ENVIROMENT:
                                newChunk = Instantiate(GameResourceSupervisor.GetInstance().EnviromentChunkPrefab.LoadSynchro(), new Vector3(0, 0, 0),
                                                           new Quaternion(0, 0, 0, 0)) as GameObject;
                                newChunk.transform.parent = gameObject.transform;
                                newChunk.transform.name = string.Format("EnviromentChunk_{0}", ChunkNumber++);
                                ChunkSlots[x, y, z].Chunks[type] = newChunk.GetComponent<EnviromentChunk>();
                                break;
                        }
                        ChunkSlots[x, y, z].Chunks[type].SubWorldInstance = this;
                        ChunkSlots[x, y, z].Chunks[type].WorldDataIdxX = x * ChunkSize;
                        ChunkSlots[x, y, z].Chunks[type].WorldDataIdxY = y * ChunkSize;
                        ChunkSlots[x, y, z].Chunks[type].WorldDataIdxZ = z * ChunkSize;
                        var worldConfig = WorldConfigFile.Instance.GetConfig();
                        ChunkSlots[x, y, z].Chunks[type].RealCoordX = chunkRealCoordX + SubWorldRealCoordinate.x;
                        ChunkSlots[x, y, z].Chunks[type].RealCoordY = chunkRealCoordY + SubWorldRealCoordinate.y;
                        ChunkSlots[x, y, z].Chunks[type].RealCoordZ = chunkRealCoordZ + SubWorldRealCoordinate.z;
                        ChunkSlots[x, y, z].Chunks[type].Init();
                        yield return new WaitForSeconds(WorldConfigFile.Instance.GetConfig().ChunkLoadIntervalSeconds);
                    }

                }
            }
        }
        //
        KojeomLogger.DebugLog(string.Format("World name : {0} Chunk 로드를 완료했습니다.", WorldName), LOG_TYPE.DEBUG_TEST);
        bLoadFinish = true;
        OnFinishLoadChunks(UniqueID);

        // 월드에 등록된 모든 Actor를 Show.
        foreach (var actor in InGameObjRegister.RegisteredActors)
        {
            actor.Show();
        }
    }

    private int PerlinNoise (int x, int y, int z, float scale, float height, float power)
    {
        // noise value 0 to 1
        float rValue;
        rValue = Noise.GetNoise (((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
        rValue *= height;
   
        if (power != 0) rValue = Mathf.Pow(rValue, power);
        return (int)rValue;
    }

    public void RegisterObject(GameObject obj)
    {
        InGameObjRegister.Register(obj);
    }
    public void RegisterActor(Actor actor)
    {
        InGameObjRegister.Register(actor);
    }
    public void UnRegister(InGameObjectType type, GameObject obj)
    {
        InGameObjRegister.UnRegister(type, obj);
    }

    public Vector3 RandomPosAtSurface()
    {
        Vector3 ret;
        var worldConfig = WorldConfigFile.Instance.GetConfig();
        if(SubWorldRealCoordinate.x == 0)
        {
            ret.x = KojeomUtility.RandomInteger(0, (int)SubWorldRealCoordinate.x);
        }
        else
        {
            ret.x = KojeomUtility.RandomInteger((int)SubWorldRealCoordinate.x - worldConfig.SubWorldSizeX, (int)SubWorldRealCoordinate.x);
        }
        //
        ret.y = HighestHeightInWorld();

        if (SubWorldRealCoordinate.z == 0)
        {
            ret.z = KojeomUtility.RandomInteger(0, (int)SubWorldRealCoordinate.z);
        }
        else
        {
            ret.z = KojeomUtility.RandomInteger((int)SubWorldRealCoordinate.z - worldConfig.SubWorldSizeZ, (int)SubWorldRealCoordinate.z);
        }

        return ret;
    }

    private float HighestHeightInWorld()
    {
        float highest = 0.0f;
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        for (int x = 0; x < gameWorldConfig.SubWorldSizeX; x++)
        {
            for (int z = 0; z < gameWorldConfig.SubWorldSizeZ; z++)
            {
                for (int y = 1; y < gameWorldConfig.SubWorldSizeY; y++)
                {
                    if ((BlockTileType)WorldBlockData[x, y - 1, z].Type != BlockTileType.EMPTY)
                    {
                        if(highest < y)
                        {
                            highest = y;
                        }
                    }
                }
            }
        }
        return highest + (gameWorldConfig.SubWorldSizeY * SubWorldOffsetCoordinate.y);
    }
}