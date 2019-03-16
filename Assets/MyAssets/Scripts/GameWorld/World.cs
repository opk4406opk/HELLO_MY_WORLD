using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChunkSlot
{
    public AChunk[] Chunks = new AChunk[(int)ChunkType.COUNT];
}

public struct MakeWorldParam
{
    public int BaseOffset;
}
/// <summary>
/// 게임내 월드를 생성 및 관리하는 클래스.
/// </summary>
public class World : MonoBehaviour
{
    #region event
    public delegate void Del_OnFinishLoadChunks(string uniqueID);
    public event Del_OnFinishLoadChunks OnFinishLoadChunks;

    public delegate void Del_OnReadyToUnload(string uniqueID);
    public event Del_OnReadyToUnload OnReadyToUnload;
    #endregion

    public ChunkSlot[,,] ChunkSlots { get; private set; }

    public bool IsLoadFinish { get; private set; } = false;

    #region world infomation.
    public string WorldName { get; private set; }
    public string UniqueID { get; private set; }
    // 월드맵 위치값( == 오프셋값).
    public Vector3 WorldCoordinate { get; private set; }
    // 실제 게임오브젝트로서 존재하는 위치값.
    public Vector3 RealCoordinate { get; private set; }
    public bool IsSurfaceWorld { get; private set; }
    #endregion

    public Block[,,] WorldBlockData { get; private set; }
    private int ChunkSize = 0;
    private int ChunkNumber = 0;
    public CustomOctree CustomOctree { get; private set; } = new CustomOctree();

    private InGameObjectRegister InGameObjRegister;

    public void Init(SubWorldData worldData)
    {
        // setting to World
        WorldName = worldData.WorldName;
        UniqueID = worldData.UniqueID;
        WorldCoordinate = new Vector3(worldData.OffsetX, worldData.OffsetY, worldData.OffsetZ);
        var configData = WorldConfigFile.Instance.GetConfig();
        RealCoordinate = new Vector3(WorldCoordinate.x * configData.sub_world_x_size,
            WorldCoordinate.y * configData.sub_world_y_size,
            WorldCoordinate.z * configData.sub_world_z_size);
        IsSurfaceWorld = worldData.IsSurface;
        // setting to GameObject
        gameObject.name = WorldName;
        //
        StartCoroutine(Tick());
    }

    private IEnumerator Tick()
    {
        KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} is Tick Start.", UniqueID));
        while(true)
        {
            if(GamePlayerManager.Instance != null && GamePlayerManager.Instance.IsInitializeFinish == true)
            {
                var curPlayerWorld = WorldManager.Instance.
                           ContainedWorld(GamePlayerManager.Instance.MyGamePlayer.Controller.GetPosition());
                if (curPlayerWorld != null)
                {
                    var dist = Vector3.Distance(curPlayerWorld.WorldCoordinate, WorldCoordinate);
                    KojeomLogger.DebugLog(string.Format("SubWorld ID : {0} away from {1} distance Player contained World",
                        UniqueID, dist));
                    // 거리값이 N 이상이 되면..Release.
                    //OnReadyToUnload(UniqueID);
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Release()
    {
        StopCoroutine(Tick());
        //
        foreach(var slot in ChunkSlots)
        {
            for (int type = 0; type < (int)ChunkType.COUNT; type++)
            {
                slot.Chunks[type].Release();
            }
        }
        ChunkSlots = null;
    }

    public void LoadSyncro(Vector3 pos)
	{
        InGameObjRegister = new InGameObjectRegister();
        InGameObjRegister.Initialize();
        //
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        CustomOctree.Init(pos, new Vector3(gameWorldConfig.sub_world_x_size + pos.x, 
            gameWorldConfig.sub_world_y_size + pos.y,
            gameWorldConfig.sub_world_z_size + pos.z));
        ChunkSize = gameWorldConfig.chunk_size;
        // init world data.
        WorldBlockData = new Block[gameWorldConfig.sub_world_x_size,
            gameWorldConfig.sub_world_y_size,
            gameWorldConfig.sub_world_z_size];
        for (int x = 0; x < gameWorldConfig.sub_world_x_size; x++)
        {
            for (int z = 0; z < gameWorldConfig.sub_world_z_size; z++)
            {
                for (int y = 0; y < gameWorldConfig.sub_world_y_size; y++)
                {
                    WorldBlockData[x, y, z] = new Block
                    {
                        isRendered = false
                    };
                }
            }
        }
        // init chunk group.
        ChunkSlots = new ChunkSlot[Mathf.FloorToInt(gameWorldConfig.sub_world_x_size / ChunkSize),
            Mathf.FloorToInt(gameWorldConfig.sub_world_y_size / ChunkSize),
            Mathf.FloorToInt(gameWorldConfig.sub_world_z_size / ChunkSize)];
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

        if (GameStatus.DetailSingleModeFlag != DetailSingleMode.LOAD_GAME)
        {
            MakeWorldParam param;
            param.BaseOffset = KojeomUtility.RandomInteger(2, 29);
            WorldGenAlgorithms.DefaultGenWorld(WorldBlockData, param);
            //
            LoadChunkProcess();
        }
        else
        {
            LoadChunkProcess();
        }
    }

    //void OnDrawGizmos()
    //{
    //    customOctree.DrawFullTree();
    //}

    public void LoadChunkProcess()
    {
        KojeomLogger.DebugLog(string.Format("World name : {0}, Chunk 로드를 시작합니다.", WorldName), LOG_TYPE.DEBUG_TEST);
        StartCoroutine(LoadChunks());
    }
    
    private IEnumerator LoadChunks()
    {
        for (int x = 0; x < ChunkSlots.GetLength(0); x++)
            for (int z = 0; z < ChunkSlots.GetLength(2); z++)
            {
                for (int y = 0; y < ChunkSlots.GetLength(1); y++)
                {
                    for (int type = 0; type < (int)ChunkType.COUNT; type++)
                    {
                        if ((ChunkSlots[x, y, z].Chunks[type] != null) &&
                        (ChunkSlots[x, y, z].Chunks[type].gameObject.activeSelf == true))
                        {
                            ChunkSlots[x, y, z].Chunks[type].gameObject.SetActive(true);
                            continue;
                        }
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
                            case ChunkType.COMMON:
                                newChunk = Instantiate(GameResourceSupervisor.Instance.CommonChunkPrefab.LoadSynchro(), new Vector3(0, 0, 0),
                                                           new Quaternion(0, 0, 0, 0)) as GameObject;
                                newChunk.transform.parent = gameObject.transform;
                                newChunk.transform.name = string.Format("CommonChunk_{0}", ChunkNumber++);
                                ChunkSlots[x, y, z].Chunks[type] = newChunk.GetComponent<CommonChunk>();
                                break;
                            case ChunkType.WATER:
                                newChunk = Instantiate(GameResourceSupervisor.Instance.WaterChunkPrefab.LoadSynchro(), new Vector3(0, 0, 0),
                                                           new Quaternion(0, 0, 0, 0)) as GameObject;
                                newChunk.transform.parent = gameObject.transform;
                                newChunk.transform.name = string.Format("WaterChunk_{0}", ChunkNumber++);
                                ChunkSlots[x, y, z].Chunks[type] = newChunk.GetComponent<WaterChunk>();
                                break;
                        }
                        ChunkSlots[x, y, z].Chunks[type].World = this;
                        ChunkSlots[x, y, z].Chunks[type].WorldDataIdxX = x * ChunkSize;
                        ChunkSlots[x, y, z].Chunks[type].WorldDataIdxY = y * ChunkSize;
                        ChunkSlots[x, y, z].Chunks[type].WorldDataIdxZ = z * ChunkSize;
                        var WorldConfig = WorldConfigFile.Instance.GetConfig();
                        ChunkSlots[x, y, z].Chunks[type].RealCoordX = chunkRealCoordX + WorldCoordinate.x * WorldConfig.sub_world_x_size;
                        ChunkSlots[x, y, z].Chunks[type].RealCoordY = chunkRealCoordY + WorldCoordinate.y * WorldConfig.sub_world_y_size;
                        ChunkSlots[x, y, z].Chunks[type].RealCoordZ = chunkRealCoordZ + WorldCoordinate.z * WorldConfig.sub_world_z_size;
                        ChunkSlots[x, y, z].Chunks[type].Init();
                        yield return new WaitForSeconds(WorldConfigFile.Instance.GetConfig().chunkLoadIntervalSeconds);
                    }

                }
            }
        KojeomLogger.DebugLog(string.Format("World name : {0} Chunk 로드를 완료했습니다.", WorldName), LOG_TYPE.DEBUG_TEST);
        IsLoadFinish = true;
        OnFinishLoadChunks(UniqueID);
    }

    private void UnloadColumn(int x, int z)
    {
		for (int y=0; y< ChunkSlots.GetLength(1); y++)
        {
            for(int type = 0; type < (int)ChunkType.COUNT; type++)
            {
                //Object.Destroy(chunkGroup [x, y, z].chunks[type].gameObject);
                ChunkSlots[x, y, z].Chunks[type].gameObject.SetActive(false);
            }
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
        if(RealCoordinate.x == 0)
        {
            ret.x = KojeomUtility.RandomInteger(0, (int)RealCoordinate.x);
        }
        else
        {
            ret.x = KojeomUtility.RandomInteger((int)RealCoordinate.x - worldConfig.sub_world_x_size, (int)RealCoordinate.x);
        }
        // 월드 표면위로 y값을 설정하고싶은데.. 어떻게 할까?
        // 일단 임시값으로 200을 설정.
        ret.y = 200.0f;

        if (RealCoordinate.z == 0)
        {
            ret.z = KojeomUtility.RandomInteger(0, (int)RealCoordinate.z);
        }
        else
        {
            ret.z = KojeomUtility.RandomInteger((int)RealCoordinate.z - worldConfig.sub_world_z_size, (int)RealCoordinate.z);
        }

        return ret;
    }
}