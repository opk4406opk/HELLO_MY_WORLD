using UnityEngine;
using System.Collections;
 
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
    // 1개의 월드에는 N개의 청크가 있다.
    private Chunk[,,] _chunkGroup;
    public Chunk[,,] chunkGroup
    {
        get { return _chunkGroup; }
    }
    
    // 월드의 모든 블록성질을 저장하는 배열.
	public byte[,,] worldBlockData;

	private int worldX = 0;
    private int worldY = 0;
    private int worldZ = 0;
    private int chunkSize = 0;
    private int _chunkOffsetX = 0;
    public int chunkOffsetX
    {
        get { return _chunkOffsetX; }
    }
    private int _chunkOffsetZ = 0;
    public int chunkOffsetZ
    {
        get { return _chunkOffsetZ; }
    }
    

    private IEnumerator loadProcessRoutine;
    private readonly float INTERVAL_LOAD_TIME = 1.0f;

    public void Init(int offsetX, int offsetZ)
	{
        worldX = GameWorldConfig.worldX;
        worldY = GameWorldConfig.worldY;
        worldZ = GameWorldConfig.worldZ;
        chunkSize = GameWorldConfig.chunkSize;
        _chunkOffsetX = offsetX;
        _chunkOffsetZ = offsetZ;

        InitWorldData();
        InitChunkGroup();
        loadProcessRoutine = LoadProcess();
        StartCoroutine(loadProcessRoutine);
    }

    private IEnumerator LoadProcess()
    {
        while(true)
        {
            LoadChunks(48, 256);
            yield return new WaitForSeconds(INTERVAL_LOAD_TIME);
        }
    }
    
    private void LoadChunks(float distToLoad, float distToUnload)
    {
        for (int x = 0; x < _chunkGroup.GetLength(0); x++)
            for (int z = 0; z < _chunkGroup.GetLength(2); z++)
            {
                float dist = Vector2.Distance(new Vector2((x + _chunkOffsetX) * chunkSize,
                        (z + _chunkOffsetZ) * chunkSize),
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
            GameObject newChunk = Instantiate(_chunkPrefab, new Vector3((x + _chunkOffsetX) * chunkSize,
                                                y * chunkSize, (z + _chunkOffsetZ) * chunkSize),
                                                new Quaternion(0, 0, 0, 0)) as GameObject;

            newChunk.transform.parent = gameObject.transform;
            _chunkGroup[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
            _chunkGroup[x, y, z].world = this;
            _chunkGroup[x, y, z].chunkX = x * chunkSize;
            _chunkGroup[x, y, z].chunkY = y * chunkSize;
            _chunkGroup[x, y, z].chunkZ = z * chunkSize;
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
        worldBlockData = new byte[worldX, worldY, worldZ];

        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
                stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;
                int dirt = PerlinNoise(x, 100, z, 50, 3, 0) + 1;

                for (int y = 0; y < worldY; y++)
                {
                    if (y <= stone) worldBlockData[x, y, z] = 1;
                    else if (y <= dirt + stone) worldBlockData[x, y, z] = 2;
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
  
  
	public byte Block (int x, int y, int z)
	{
   
		if (x >= worldX ||
            x < 0 ||
            y >= worldY ||
            y < 0 ||
            z >= worldZ ||
            z < 0)
        {
			return (byte)1;
		}
   
		return worldBlockData [x, y, z];
	}
}