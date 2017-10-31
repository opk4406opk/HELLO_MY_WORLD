using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
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
    // 1개의 월드에는 N개의 Chunk가 있으며 각 Chunk는 M개의 Block을 가지고 있다.
    private Chunk[,,] _chunkGroup;
    public Chunk[,,] chunkGroup
    {
        get { return _chunkGroup; }
    }

    private string _worldName;
    public string worldName
    {
        set {
            _worldName = value;
            gameObject.name = _worldName;
        }
        get { return _worldName; }
    }

    ///<summary>
    /// 월드의 모든 블록을 저장하는 배열.
    ///</summary>
    private Block[,,] _worldBlockData;
    public Block[,,] worldBlockData
    {
        get { return _worldBlockData; }
    }
    
    private int worldX = 0;
    private int worldY = 0;
    private int worldZ = 0;
    private int chunkSize = 0;
    private int _worldOffsetX = 0;
    public int worldOffsetX
    {
        get { return _worldOffsetX; }
    }
    private int _worldOffsetZ = 0;
    public int worldOffsetZ
    {
        get { return _worldOffsetZ; }
    }

    private IEnumerator _loadProcessRoutine;
    public IEnumerator loadProcessRoutine
    {
        get { return _loadProcessRoutine; }
    }
    
    private readonly float INTERVAL_LOAD_TIME = 1.0f;
    private TileDataFile worldTileDataFile;
    private int chunkNumber = 0;

    private CustomOctree _customOctree = new CustomOctree();
    public CustomOctree customOctree
    {
        get { return _customOctree; }
    }

    public void Init(int offsetX, int offsetZ, TileDataFile tileDataFile)
	{
        _customOctree.Init(new Vector3(offsetX, 0, offsetZ), 
            new Vector3(GameConfig.subWorldX + offsetX ,GameConfig.worldY,
            GameConfig.subWorldZ + offsetZ));
        worldTileDataFile = tileDataFile;
        worldX = GameConfig.subWorldX;
        worldY = GameConfig.worldY;
        worldZ = GameConfig.subWorldZ;
        chunkSize = GameConfig.subWorldChunkSize;
        _worldOffsetX = offsetX;
        _worldOffsetZ = offsetZ;

        InitWorldData();
        InitChunkGroup();
        if (GameStatus.isLoadGame == false)
        {
            SetDefaultWorldData();
            _loadProcessRoutine = LoadProcess();
            StartCoroutine(_loadProcessRoutine);
        }
        else
        {
            _loadProcessRoutine = LoadProcess();
        }
    }

    void OnDrawGizmos()
    {
       // _customOctree.DrawFullTree();
    }

    private IEnumerator LoadProcess()
    {
        while(true)
        {
            LoadChunks();
            yield return new WaitForSeconds(INTERVAL_LOAD_TIME);
        }
    }
    
    private void LoadChunks()
    {
        for (int x = 0; x < _chunkGroup.GetLength(0); x++)
            for (int z = 0; z < _chunkGroup.GetLength(2); z++)
            {
                float dist = Vector2.Distance(new Vector2(x * chunkSize,
                        z * chunkSize),
                        new Vector2(_playerTrans.position.x, _playerTrans.position.z));

                if (dist < DIST_TO_LOAD)
                {
                    if (_chunkGroup[x, 0, z] == null) GenColumn(x, z);
                }
                else if (dist > DIST_TO_UNLOAD)
                {
                    if (_chunkGroup[x, 0, z] != null) UnloadColumn(x, z);
                }
            }
    }

    private void GenColumn(int x, int z)
    {
        for (int y = 0; y < _chunkGroup.GetLength(1); y++)
        {
            // 유니티엔진에서 제공되는 게임 오브젝트들의 중점(=월드좌표에서의 위치)은
            // 실제 게임 오브젝트의 정중앙이 된다. 
            // 따라서, 유니티엔진에 맞춰서 오브젝트의 중점을 정중앙으로 하려면, 아래와 같은 0.5f(offset)값을 추가한다.
            // p.s. 이 프로젝트에서 1개의 block의 기준점(block을 생성할 때 쓰이는)은 최상단면의 좌측하단의 포인트가 된다.(디폴트)
            float worldCoordX = x * chunkSize - 0.5f;
            float worldCoordY = y * chunkSize + 0.5f;
            float worldCoordZ = z * chunkSize - 0.5f;
            GameObject newChunk = Instantiate(_chunkPrefab, new Vector3(0, 0, 0),
                                                new Quaternion(0, 0, 0, 0)) as GameObject;
            newChunk.transform.parent = gameObject.transform;
            newChunk.transform.name = "Chunk_" + chunkNumber++;
            _chunkGroup[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
            _chunkGroup[x, y, z].world = this;
            _chunkGroup[x, y, z].worldDataIdxX = x * chunkSize;
            _chunkGroup[x, y, z].worldDataIdxY = y * chunkSize;
            _chunkGroup[x, y, z].worldDataIdxZ = z * chunkSize;
            _chunkGroup[x, y, z].worldCoordX = worldCoordX + _worldOffsetX;
            _chunkGroup[x, y, z].worldCoordY = worldCoordY;
            _chunkGroup[x, y, z].worldCoordZ = worldCoordZ + _worldOffsetZ;
            _chunkGroup[x, y, z].Init(worldTileDataFile);
        }
	}

    private void UnloadColumn(int x, int z)
    {
		for (int y=0; y< _chunkGroup.GetLength(1); y++)
        {
			Object.Destroy(chunkGroup [x, y, z].gameObject);
		}
	}

    private void InitWorldData()
    {
        _worldBlockData = new Block[worldX, worldY, worldZ];
        for (int x = 0; x < worldX; x++)
        {
            for (int y = 0; y < worldY; y++)
            {
                for (int z = 0; z < worldZ; z++)
                {
                    _worldBlockData[x, y, z] = new Block();
                    _worldBlockData[x, y, z].isRendered = false;
                }
            }
        }
    }
  
    private void SetDefaultWorldData()
    {
		// 펄린노이즈 알고리즘을 이용해 지형을 생성한다.
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
                stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;
                int grass = PerlinNoise(x, 100, z, 50, 3, 0) + 1;

                for (int y = 0; y < worldY; y++)
                {
                    if (y <= stone) _worldBlockData[x, y, z].type = (byte)worldTileDataFile.GetTileData(TileType.TILE_TYPE_STONE_BIG).type;
                    else if (y <= grass + stone) _worldBlockData[x, y, z].type = (byte)worldTileDataFile.GetTileData(TileType.TILE_TYPE_GRASS).type;
                }
            }
        }
    }

    private void InitChunkGroup()
    {
        _chunkGroup = new Chunk[Mathf.FloorToInt(worldX / chunkSize), Mathf.FloorToInt(worldY / chunkSize), Mathf.FloorToInt(worldZ / chunkSize)];
    }

    private int PerlinNoise (int x, int y, int z, float scale, float height, float power)
	{
		float rValue;
		rValue = Noise.GetNoise (((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
		rValue *= height;
   
		if (power != 0) rValue = Mathf.Pow(rValue, power);
		return (int)rValue;
	}
    
}