using UnityEngine;
using System.Collections;
using System.Collections.Generic;
 
/// <summary>
/// 게임내 월드를 생성 및 관리하는 클래스.
/// </summary>
public class World : MonoBehaviour
{
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

    private List<Block> _worldBlockList = new List<Block>(); 
    
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

    public void Init(int offsetX, int offsetZ, TileDataFile tileDataFile)
	{
        worldTileDataFile = tileDataFile;
        worldX = GameWorldConfig.worldX;
        worldY = GameWorldConfig.worldY;
        worldZ = GameWorldConfig.worldZ;
        chunkSize = GameWorldConfig.chunkSize;
        _worldOffsetX = offsetX;
        _worldOffsetZ = offsetZ;

        InitWorldData();
        InitChunkGroup();
        if (GameStatus.isLoadGame == false)
        {
            InsertDefaultWorldData();
            _loadProcessRoutine = LoadProcess();
            StartCoroutine(_loadProcessRoutine);
        }
        else
        {
            _loadProcessRoutine = LoadProcess();
        }
    }

    private IEnumerator LoadProcess()
    {
        while(true)
        {
            LoadChunks(48, 96);
            yield return new WaitForSeconds(INTERVAL_LOAD_TIME);
        }
    }
    
    private void LoadChunks(float distToLoad, float distToUnload)
    {
        for (int x = 0; x < _chunkGroup.GetLength(0); x++)
            for (int z = 0; z < _chunkGroup.GetLength(2); z++)
            {
                float dist = Vector2.Distance(new Vector2((x + _worldOffsetX) * chunkSize,
                        (z + _worldOffsetZ) * chunkSize),
                        new Vector2(_playerTrans.position.x, _playerTrans.position.z));

                if (dist < distToLoad)
                {
                    if (_chunkGroup[x, 0, z] == null) GenColumn(x, z);
                }
                else if (dist > distToUnload)
                {
                    if (_chunkGroup[x, 0, z] != null) UnloadColumn(x, z);
                }
            }
    }

    private void GenColumn(int x, int z)
    {
        for (int y = 0; y < _chunkGroup.GetLength(1); y++)
        {
            //subWorld offset 크기만큼 실제 chunk의 world Position에 적용.
            // 0.5f 의 값을 실제 Chunk가 위치하는 xyz값에 더하고 빼는 이유는 아래와 같다.
            // 유니티엔진에서 제공되는 모든 게임오브젝트들의 중점은 정가운데로 되어 있다. Cube로 예를 들면 정육면체의 정가운데 지점이 된다.
            // 따라서, 실제 Chunk에서 N개의 Block을 생성할 때 Block의 중점은 상단면 최하단 왼쪽 부분이다. ( = 유니티엔진과 실제 생성하는 블록과의 중점의 차이가 발생. )
            // 중점에 대한 차이를 없애기 위해 아래 코드처럼 처리한다.
            float coordX = x * chunkSize + 0.5f;
            float coordY = y * chunkSize - 0.5f;
            float coordZ = z * chunkSize + 0.5f;
            GameObject newChunk = Instantiate(_chunkPrefab, new Vector3(0, 0 , 0),
                                                new Quaternion(0, 0, 0, 0)) as GameObject;

            newChunk.transform.parent = gameObject.transform;
            newChunk.transform.name = "Chunk_" + chunkNumber++;
            _chunkGroup[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
            _chunkGroup[x, y, z].world = this;
            _chunkGroup[x, y, z].worldDataIdxX = x * chunkSize;
            _chunkGroup[x, y, z].worldDataIdxY = y * chunkSize;
            _chunkGroup[x, y, z].worldDataIdxZ = z * chunkSize;
            _chunkGroup[x, y, z].worldCoordX = coordX;
            _chunkGroup[x, y, z].worldCoordY = coordY;
            _chunkGroup[x, y, z].worldCoordZ = coordZ;
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
            for (int y = 0; y < worldY; y++)
                for (int z = 0; z < worldZ; z++)
                {
                    _worldBlockData[x, y, z].aabb.isEnable = false;
                }
    }
  
    private void InsertDefaultWorldData()
    {
        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
                stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;
                int dirt = PerlinNoise(x, 100, z, 50, 3, 0) + 1;

                for (int y = 0; y < worldY; y++)
                {
                    if (y <= stone) _worldBlockData[x, y, z].type = (byte)worldTileDataFile.GetTileData("STONE_BIG").type;
                    else if (y <= dirt + stone) _worldBlockData[x, y, z].type = (byte)worldTileDataFile.GetTileData("GRASS").type;
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