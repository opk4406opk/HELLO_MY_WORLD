using UnityEngine;
using System.Collections;

public class ChunkSlot
{
    public AChunk[] chunks = new AChunk[(int)ChunkType.COUNT];
}

public struct MakeWorldParam
{
    public int baseOffset;
}
/// <summary>
/// 게임내 월드를 생성 및 관리하는 클래스.
/// </summary>
public class World : MonoBehaviour
{
    public delegate void del_OnFinishLoadChunks(int worldIdx);
    public event del_OnFinishLoadChunks OnFinishLoadChunks;

    public ChunkSlot[,,] chunkSlots { get; private set; }

    private string _worldName;
    public string worldName
    {
        set {
            _worldName = value;
            gameObject.name = _worldName;
        }
        get { return _worldName; }
    }

    /// <summary>
    /// 월드 create idx 넘버.
    /// </summary>
    public int worldIndex;
    public Block[,,] worldBlockData { get; private set; }

    private int worldX = 0;
    private int worldY = 0;
    private int worldZ = 0;
    private int chunkSize = 0;

    public int worldOffsetX { get; private set; } = 0;
    public int worldOffsetY { get; private set; } = 0;
    public int worldOffsetZ { get; private set; } = 0;
    
    private int chunkNumber = 0;
    public CustomOctree customOctree { get; } = new CustomOctree();

    public void Init(int offsetX, int offsetY, int offsetZ)
	{
        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
        //
        customOctree.Init(new Vector3(offsetX, offsetY, offsetZ), 
            new Vector3(gameWorldConfig.sub_world_x_size + offsetX , 
            gameWorldConfig.sub_world_y_size + offsetY,
            gameWorldConfig.sub_world_z_size + offsetZ));
        worldX = gameWorldConfig.sub_world_x_size;
        worldY = gameWorldConfig.sub_world_y_size;
        worldZ = gameWorldConfig.sub_world_z_size;
        chunkSize = gameWorldConfig.chunk_size;
        worldOffsetX = offsetX;
        worldOffsetY = offsetY;
        worldOffsetZ = offsetZ;

        // init world data.
        worldBlockData = new Block[worldX, worldY, worldZ];
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                for (int y = 0; y < worldY; y++)
                {
                    worldBlockData[x, y, z] = new Block();
                    worldBlockData[x, y, z].isRendered = false;
                }
            }
        }
        // init chunk group.
        chunkSlots = new ChunkSlot[Mathf.FloorToInt(worldX / chunkSize), Mathf.FloorToInt(worldY / chunkSize),
            Mathf.FloorToInt(worldZ / chunkSize)];
        for (int x = 0; x < chunkSlots.GetLength(0); x++)
        {
            for (int z = 0; z < chunkSlots.GetLength(2); z++)
            {
                for (int y = 0; y < chunkSlots.GetLength(1); y++)
                {
                    chunkSlots[x, y, z] = new ChunkSlot();
                }
            }
        }

        if (GameStatus.isLoadGame == false)
        {
            MakeWorldParam param;
            param.baseOffset = KojeomUtility.RandomInteger(2, 29);
            WorldGenAlgorithms.DefaultGenWorld(worldBlockData, param);
            //
            LoadProcess();
        }
        else
        {
            LoadProcess();
        }
    }

    //void OnDrawGizmos()
    //{
    //    customOctree.DrawFullTree();
    //}

    public void LoadProcess()
    {
        KojeomLogger.DebugLog(string.Format("World name : {0}, Chunk 로드를 시작합니다.", worldName), LOG_TYPE.DEBUG_TEST);
        StartCoroutine(LoadChunks());
    }
    
    private IEnumerator LoadChunks()
    {
        for (int x = 0; x < chunkSlots.GetLength(0); x++)
            for (int z = 0; z < chunkSlots.GetLength(2); z++)
            {
                for (int y = 0; y < chunkSlots.GetLength(1); y++)
                {
                    for (int type = 0; type < (int)ChunkType.COUNT; type++)
                    {
                        if ((chunkSlots[x, y, z].chunks[type] != null) &&
                        (chunkSlots[x, y, z].chunks[type].gameObject.activeSelf == true))
                        {
                            chunkSlots[x, y, z].chunks[type].gameObject.SetActive(true);
                            continue;
                        }
                        // 유니티엔진에서 제공되는 게임 오브젝트들의 중점(=월드좌표에서의 위치)은
                        // 실제 게임 오브젝트의 정중앙이 된다. 
                        // 따라서, 유니티엔진에 맞춰서 오브젝트의 중점을 정중앙으로 블록들을 생성하려면(= 1개의 블록은 6개의 면을 생성한다),
                        // 아래와 같은 0.5f(offset)값을 추가한다. ( worldCoordX,Y,Z 값은 개별 블록을 생성할 때 사용된다. )
                        // p.s. 이 프로젝트에서 1개의 block의 기준점(block을 생성할 때 쓰이는)은 최상단면의 좌측하단의 포인트가 된다.(디폴트)
                        float worldCoordX = x * chunkSize - 0.5f;
                        float worldCoordY = y * chunkSize + 0.5f;
                        float worldCoordZ = z * chunkSize - 0.5f;

                        GameObject newChunk = null;
                        switch ((ChunkType)type)
                        {
                            case ChunkType.COMMON:
                                newChunk = Instantiate(PrefabStorage.instance.commonChunkPrefab, new Vector3(0, 0, 0),
                                                           new Quaternion(0, 0, 0, 0)) as GameObject;
                                newChunk.transform.parent = gameObject.transform;
                                newChunk.transform.name = string.Format("CommonChunk_{0}", chunkNumber++);
                                chunkSlots[x, y, z].chunks[type] = newChunk.GetComponent<CommonChunk>();
                                break;
                            case ChunkType.WATER:
                                newChunk = Instantiate(PrefabStorage.instance.waterChunkPrefab, new Vector3(0, 0, 0),
                                                           new Quaternion(0, 0, 0, 0)) as GameObject;
                                newChunk.transform.parent = gameObject.transform;
                                newChunk.transform.name = string.Format("WaterChunk_{0}", chunkNumber++);
                                chunkSlots[x, y, z].chunks[type] = newChunk.GetComponent<WaterChunk>();
                                break;
                        }
                        chunkSlots[x, y, z].chunks[type].world = this;
                        chunkSlots[x, y, z].chunks[type].worldDataIdxX = x * chunkSize;
                        chunkSlots[x, y, z].chunks[type].worldDataIdxY = y * chunkSize;
                        chunkSlots[x, y, z].chunks[type].worldDataIdxZ = z * chunkSize;
                        chunkSlots[x, y, z].chunks[type].realCoordX = worldCoordX + worldOffsetX;
                        chunkSlots[x, y, z].chunks[type].realCoordY = worldCoordY + worldOffsetY;
                        chunkSlots[x, y, z].chunks[type].realCoordZ = worldCoordZ + worldOffsetZ;
                        chunkSlots[x, y, z].chunks[type].Init();
                        yield return new WaitForSeconds(WorldConfigFile.instance.GetConfig().chunkLoadIntervalSeconds);
                    }

                }
            }
        KojeomLogger.DebugLog(string.Format("World name : {0} Chunk 로드를 완료했습니다.", worldName), LOG_TYPE.DEBUG_TEST);
        OnFinishLoadChunks(worldIndex);
    }

    private void UnloadColumn(int x, int z)
    {
		for (int y=0; y< chunkSlots.GetLength(1); y++)
        {
            for(int type = 0; type < (int)ChunkType.COUNT; type++)
            {
                //Object.Destroy(chunkGroup [x, y, z].chunks[type].gameObject);
                chunkSlots[x, y, z].chunks[type].gameObject.SetActive(false);
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
    
}