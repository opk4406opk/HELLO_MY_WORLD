using UnityEngine;
using System.Collections;
 
public class World : MonoBehaviour
{
    [SerializeField]
    private Transform playerTrans;

    [SerializeField]
    private GameObject chunkPrefab;
	private Chunk[,,] _chunks;
    public Chunk[,,] chunks
    {
        get { return _chunks; }
    }
	public int chunkSize = 16;
    
    // 월드의 모든 블록성질을 저장하는 배열.
	public byte[,,] data;

    [SerializeField]
	private int worldX = 16;
    [SerializeField]
    private int worldY = 16;
    [SerializeField]
    private int worldZ = 16;

    private IEnumerator loadProcessRoutine;
    private readonly float INTERVAL_TIME = 1.0f;

    private int chunkOffsetX = 0;
    private int chunkOffsetZ = 0;

    public void Init(int offsetX, int offsetZ)
	{
        chunkOffsetX = offsetX;
        chunkOffsetZ = offsetZ;

        InitWorldData();
        InitChunks();
        loadProcessRoutine = LoadProcess();
        StartCoroutine(loadProcessRoutine);
    }

    private IEnumerator LoadProcess()
    {
        while(true)
        {
            LoadChunks(playerTrans.position, 48, 16);
            yield return new WaitForSeconds(INTERVAL_TIME);
        }
    }

    public void LoadChunks(Vector3 playerPos, float distToLoad, float distToUnload)
    {
        for (int x = 0; x < _chunks.GetLength(0); x++)
        {
            for (int z = 0; z < _chunks.GetLength(2); z++)
            {
                float dist = Vector2.Distance(new Vector2(x * chunkSize, z * chunkSize), new Vector2(playerPos.x, playerPos.z));
                if (dist < distToLoad)
                {
                    if (_chunks[x, 0, z] == null) GenColumn(x, z);
                }
                else if (dist > distToUnload)
                {
                    if (_chunks[x, 0, z] != null) UnloadColumn(x, z);
                }
            }
        }

    }

    private void GenColumn(int x, int z)
    {
        for (int y = 0; y < _chunks.GetLength(1); y++)
        {
            //Create a temporary Gameobject for the new chunk instead of using chunks[x,y,z]
            GameObject newChunk = Instantiate(chunkPrefab, new Vector3((x + chunkOffsetX) * chunkSize,
                                                y * chunkSize, (z + chunkOffsetZ) * chunkSize),
                                                new Quaternion(0, 0, 0, 0)) as GameObject;

            newChunk.transform.parent = gameObject.transform;
            _chunks[x, y, z] = newChunk.GetComponent("Chunk") as Chunk;
            _chunks[x, y, z].world = gameObject.GetComponent("World") as World;
            _chunks[x, y, z].chunkSize = chunkSize;
            _chunks[x, y, z].chunkX = x * chunkSize;
            _chunks[x, y, z].chunkY = y * chunkSize;
            _chunks[x, y, z].chunkZ = z * chunkSize;
        }
	}

    private void UnloadColumn(int x, int z)
    {
		for (int y=0; y< _chunks.GetLength(1); y++)
        {
			Object.Destroy(chunks [x, y, z].gameObject);
			
		}
	}
  
    private void InitWorldData()
    {
        data = new byte[worldX, worldY, worldZ];

        for (int x = 0; x < worldX; x++)
        {
            for (int z = 0; z < worldZ; z++)
            {
                int stone = PerlinNoise(x, 0, z, 10, 3, 1.2f);
                stone += PerlinNoise(x, 300, z, 20, 4, 0) + 10;
                int dirt = PerlinNoise(x, 100, z, 50, 3, 0) + 1;

                for (int y = 0; y < worldY; y++)
                {
                    if (y <= stone) data[x, y, z] = 1;
                    else if (y <= dirt + stone) data[x, y, z] = 2;
                }
            }
        }
    }
    private void InitChunks()
    {
        _chunks = new Chunk[Mathf.FloorToInt(worldX / chunkSize), Mathf.FloorToInt(worldY / chunkSize), Mathf.FloorToInt(worldZ / chunkSize)];
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
   
		return data [x, y, z];
	}
}