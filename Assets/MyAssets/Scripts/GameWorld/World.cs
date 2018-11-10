using UnityEngine;
using System.Collections;

public struct MakeWorldParam
{
    public int baseOffset;
}
/// <summary>
/// 게임내 월드를 생성 및 관리하는 클래스.
/// </summary>
public class World : MonoBehaviour
{
    private readonly float DIST_TO_LOAD = 48.0f;
    private readonly float DIST_TO_UNLOAD = 96.0f;

    private Transform _playerTrans;
    public Transform playerTrans
    {
        set { _playerTrans = value; }
    }
    private GameObject _chunkPrefab;
    public GameObject chunkPrefab
    {
        set { _chunkPrefab = value; }
    }
    public Chunk[,,] chunkGroup { get; private set; }

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
    /// 월드 idx 넘버.
    /// </summary>
    public int idx;
    public Block[,,] worldBlockData { get; private set; }

    private int worldX = 0;
    private int worldY = 0;
    private int worldZ = 0;
    private int chunkSize = 0;
    public int worldOffsetX { get; private set; } = 0;
    public int worldOffsetZ { get; private set; } = 0;
    private int chunkNumber = 0;
    public CustomOctree customOctree { get; } = new CustomOctree();

    public void Init(int offsetX, int offsetZ)
	{
        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
        //
        customOctree.Init(new Vector3(offsetX, 0, offsetZ), 
            new Vector3(gameWorldConfig.sub_world_x_size + offsetX , gameWorldConfig.sub_world_y_size,
            gameWorldConfig.sub_world_z_size + offsetZ));
        worldX = gameWorldConfig.sub_world_x_size;
        worldY = gameWorldConfig.sub_world_y_size;
        worldZ = gameWorldConfig.sub_world_z_size;
        chunkSize = gameWorldConfig.chunk_size;
        worldOffsetX = offsetX;
        worldOffsetZ = offsetZ;

        InitWorldData();
        InitChunkGroup();
        if (GameStatus.isLoadGame == false)
        {
            MakeWorldParam param;
            param.baseOffset = KojeomUtility.RandomInteger(2, 29);
            SetDefaultWorldData(param);
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
    //   // _customOctree.DrawFullTree();
    //}

    public void LoadProcess()
    {
        StartCoroutine(LoadChunks());
    }
    
    private IEnumerator LoadChunks()
    {
        for (int x = 0; x < chunkGroup.GetLength(0); x++)
            for (int z = 0; z < chunkGroup.GetLength(2); z++)
            {
                //float dist = Vector2.Distance(new Vector2(x * chunkSize,
                //        z * chunkSize),
                //        new Vector2(_playerTrans.position.x, _playerTrans.position.z));

                //if (dist < DIST_TO_LOAD)
                //{
                //    if (_chunkGroup[x, 0, z] == null) GenColumn(x, z);
                //}
                //else if (dist > DIST_TO_UNLOAD)
                //{
                //    if (_chunkGroup[x, 0, z] != null) UnloadColumn(x, z);
                //}
                if (chunkGroup[x, 0, z] == null)
                {
                    for (int y = 0; y < chunkGroup.GetLength(1); y++)
                    {
                        if ((chunkGroup[x, y, z] != null) &&
                            (chunkGroup[x, y, z].gameObject.activeSelf == true))
                        {
                            chunkGroup[x, y, z].gameObject.SetActive(true);
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
                        GameObject newChunk = Instantiate(_chunkPrefab, new Vector3(0, 0, 0),
                                                            new Quaternion(0, 0, 0, 0)) as GameObject;
                        newChunk.transform.parent = gameObject.transform;
                        newChunk.transform.name = "Chunk_" + chunkNumber++;
                        chunkGroup[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
                        chunkGroup[x, y, z].world = this;
                        chunkGroup[x, y, z].worldDataIdxX = x * chunkSize;
                        chunkGroup[x, y, z].worldDataIdxY = y * chunkSize;
                        chunkGroup[x, y, z].worldDataIdxZ = z * chunkSize;
                        chunkGroup[x, y, z].worldCoordX = worldCoordX + worldOffsetX;
                        chunkGroup[x, y, z].worldCoordY = worldCoordY;
                        chunkGroup[x, y, z].worldCoordZ = worldCoordZ + worldOffsetZ;
                        chunkGroup[x, y, z].Init();
                        yield return new WaitForSeconds(WorldConfigFile.instance.GetConfig().chunkLoadIntervalSeconds);
                    }
                }
            }
    }

    private void UnloadColumn(int x, int z)
    {
		for (int y=0; y< chunkGroup.GetLength(1); y++)
        {
            //Object.Destroy(chunkGroup [x, y, z].gameObject);
            chunkGroup[x, y, z].gameObject.SetActive(false);
        }
	}

    private void InitWorldData()
    {
        worldBlockData = new Block[worldX, worldY, worldZ];
        for (int x = 0; x < worldX; x++)
        {
            for (int y = 0; y < worldY; y++)
            {
                for (int z = 0; z < worldZ; z++)
                {
                    worldBlockData[x, y, z] = new Block();
                    worldBlockData[x, y, z].isRendered = false;
                }
            }
        }
    }
  
    private void SetDefaultWorldData(MakeWorldParam param)
    {
        WorldGenAlgorithms.DefaultGenWorld(worldBlockData, param);
    }

    private void InitChunkGroup()
    {
        chunkGroup = new Chunk[Mathf.FloorToInt(worldX / chunkSize), Mathf.FloorToInt(worldY / chunkSize), Mathf.FloorToInt(worldZ / chunkSize)];
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