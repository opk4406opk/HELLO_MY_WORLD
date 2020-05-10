using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenLib;
using System.Threading.Tasks;

public abstract class AChunk : MonoBehaviour {

	public SubWorld SubWorldInstance { set; get; }
	protected List<Vector3> NewVertices = new List<Vector3>();
	protected List<int> NewTriangles = new List<int>();
	protected List<Vector2> NewUV = new List<Vector2>();

	/// <summary>
	/// size per tile.
	///  ex) 256 x 256(pixel) CommonBlockSheet 기준. 1tile(16pixel) 이 차지하는 텍스처 좌표값.  16/256
	/// </summary>
	protected float TileUnit;
	protected Vector2 TexturePos;
	protected Mesh MeshInstance;
	protected int FaceCount;

	#region Components
	protected MeshCollider MeshColliderComponent;
	#endregion

	protected int ChunkSize = 0;

	protected ChunkType ChunkType = ChunkType.NONE;
	public ChunkType GetChunkType() { return ChunkType; }

	// 월드 데이터 배열에서 Chunk가 존재하는 인덱스 값( x,y,z).----------
	protected int _WorldDataIdxX;
	public int WorldDataIdxX
	{
		set { _WorldDataIdxX = value; }
	}
	protected int _WorldDataIdxY;
	public int WorldDataIdxY
	{
		set { _WorldDataIdxY = value; }
	}
	protected int _WorldDataIdxZ;
	public int WorldDataIdxZ
	{
		set { _WorldDataIdxZ = value; }
	}
	// 월드 좌표에서 실제로 Chunk가 존재하는 좌표 x,y,z.
	protected float _RealCoordX;
	public float RealCoordX
	{
		set { _RealCoordX = value; }
	}
	protected float _RealCoordY;
	public float RealCoordY
	{
		set { _RealCoordY = value; }
	}
	protected float _RealCoordZ;
	public float RealCoordZ
	{
		set { _RealCoordZ = value; }
	}
	//-------------------------------------------------------
	protected bool _Update;
	public bool Update
	{
		set { _Update = value; }
	}

	protected abstract void CreateCube(int blockIdxX, int blockIdxY, int blockIdxZ, float cubeX, float cubeY, float cubeZ, float blockCenterX, float blockCenterY, float blockCenterZ);
	protected abstract void FixedUpdate();
	public abstract void Init();

	protected BlockTileType GetBlockType(int x, int y, int z)
	{
		var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        Const<bool> bOverXrange = new Const<bool>(x >= gameWorldConfig.SubWorldSizeX || x < 0);
        Const<bool> bOverYrange = new Const<bool>(y >= gameWorldConfig.SubWorldSizeY || y < 0);
        Const<bool> bOverZrange = new Const<bool>(z >= gameWorldConfig.SubWorldSizeZ || z < 0);
        if (bOverXrange.Value == true || bOverYrange.Value == true || bOverZrange.Value == true)
		{
			return BlockTileType.EMPTY;
		}
		return (BlockTileType)SubWorldInstance.WorldBlockData[x, y, z].CurrentType;
	}

	protected void CubeTopFace(float x, float y, float z, BlockTileType tileType)
	{
		NewVertices.Add(new Vector3(x, y, z + 1));
		NewVertices.Add(new Vector3(x + 1, y, z + 1));
		NewVertices.Add(new Vector3(x + 1, y, z));
		NewVertices.Add(new Vector3(x, y, z));

		BlockTileInfo tileInfo = BlockTileDataFile.Instance.GetBlockTileInfo(tileType);
		TexturePos.x = tileInfo.PositionX;
		TexturePos.y = tileInfo.PositionY;

		CreateFace(TexturePos);
	}

	protected void CubeNorthFace(float x, float y, float z, BlockTileType tileType)
	{

		NewVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		NewVertices.Add(new Vector3(x + 1, y, z + 1));
		NewVertices.Add(new Vector3(x, y, z + 1));
		NewVertices.Add(new Vector3(x, y - 1, z + 1));

		BlockTileInfo tileInfo = BlockTileDataFile.Instance.GetBlockTileInfo(tileType);
		TexturePos.x = tileInfo.PositionX;
		TexturePos.y = tileInfo.PositionY;

		CreateFace(TexturePos);

	}

	protected void CubeEastFace(float x, float y, float z, BlockTileType tileType)
	{

		NewVertices.Add(new Vector3(x + 1, y - 1, z));
		NewVertices.Add(new Vector3(x + 1, y, z));
		NewVertices.Add(new Vector3(x + 1, y, z + 1));
		NewVertices.Add(new Vector3(x + 1, y - 1, z + 1));

		BlockTileInfo tileInfo = BlockTileDataFile.Instance.GetBlockTileInfo(tileType);
		TexturePos.x = tileInfo.PositionX;
		TexturePos.y = tileInfo.PositionY;

		CreateFace(TexturePos);

	}

	protected void CubeSouthFace(float x, float y, float z, BlockTileType tileType)
	{

		NewVertices.Add(new Vector3(x, y - 1, z));
		NewVertices.Add(new Vector3(x, y, z));
		NewVertices.Add(new Vector3(x + 1, y, z));
		NewVertices.Add(new Vector3(x + 1, y - 1, z));

		BlockTileInfo tileInfo = BlockTileDataFile.Instance.GetBlockTileInfo(tileType);
		TexturePos.x = tileInfo.PositionX;
		TexturePos.y = tileInfo.PositionY;

		CreateFace(TexturePos);

	}

	protected void CubeWestFace(float x, float y, float z, BlockTileType tileType)
	{

		NewVertices.Add(new Vector3(x, y - 1, z + 1));
		NewVertices.Add(new Vector3(x, y, z + 1));
		NewVertices.Add(new Vector3(x, y, z));
		NewVertices.Add(new Vector3(x, y - 1, z));

		BlockTileInfo tileInfo = BlockTileDataFile.Instance.GetBlockTileInfo(tileType);
		TexturePos.x = tileInfo.PositionX;
		TexturePos.y = tileInfo.PositionY;

		CreateFace(TexturePos);

	}

	protected void CubeBottomFace(float x, float y, float z, BlockTileType tileType)
	{

		NewVertices.Add(new Vector3(x, y - 1, z));
		NewVertices.Add(new Vector3(x + 1, y - 1, z));
		NewVertices.Add(new Vector3(x + 1, y - 1, z + 1));
		NewVertices.Add(new Vector3(x, y - 1, z + 1));

		BlockTileInfo tileInfo = BlockTileDataFile.Instance.GetBlockTileInfo(tileType);
		TexturePos.x = tileInfo.PositionX;
		TexturePos.y = tileInfo.PositionY;

		CreateFace(TexturePos);

	}

	protected void CreateFace(Vector2 texturePos)
	{
		NewTriangles.Add(FaceCount * 4); //1
		NewTriangles.Add(FaceCount * 4 + 1); //2
		NewTriangles.Add(FaceCount * 4 + 2); //3
		NewTriangles.Add(FaceCount * 4); //1
		NewTriangles.Add(FaceCount * 4 + 2); //3
		NewTriangles.Add(FaceCount * 4 + 3); //4

		NewUV.Add(new Vector2(TileUnit * texturePos.x + TileUnit, TileUnit * texturePos.y));
		NewUV.Add(new Vector2(TileUnit * texturePos.x + TileUnit, TileUnit * texturePos.y + TileUnit));
		NewUV.Add(new Vector2(TileUnit * texturePos.x, TileUnit * texturePos.y + TileUnit));
		NewUV.Add(new Vector2(TileUnit * texturePos.x, TileUnit * texturePos.y));

		FaceCount++; // Add this line
	}

    protected void GenerateMesh()
    {
        for (int relativeX = 0; relativeX < ChunkSize; relativeX++)
        {
            for (int relativeY = 0; relativeY < ChunkSize; relativeY++)
            {
                for (int relativeZ = 0; relativeZ < ChunkSize; relativeZ++)
                {
                    int blockIdxX, blockIdxY, blockIdxZ;
                    blockIdxX = relativeX + _WorldDataIdxX;
                    blockIdxY = relativeY + _WorldDataIdxY;
                    blockIdxZ = relativeZ + _WorldDataIdxZ;
                    //
                    float cubeX, cubeY, cubeZ;
                    cubeX = relativeX + _RealCoordX;
                    cubeY = relativeY + _RealCoordY;
                    cubeZ = relativeZ + _RealCoordZ;
                    //
                    // 블록 생성시 6개의 면들의 위치를 조정하기 위해 추가했던 offset 값을 제거한다.
                    // x, z 는 0.5f 씩 더해주고, y는 0.5f 빼준다.
                    float blockCenterX = cubeX + 0.5f;
                    float blockCenterY = cubeY - 0.5f;
                    float blockCenterZ = cubeZ + 0.5f;
                    SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterX = blockCenterX;
                    SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterY = blockCenterY;
                    SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterZ = blockCenterZ;
                    CreateCube(blockIdxX, blockIdxY, blockIdxZ, cubeX, cubeY, cubeZ, blockCenterX, blockCenterY, blockCenterZ);
                }
            }
        }
        UpdateMesh();
    }

    protected async void TestAsyncGenerateMesh()
    {
        bool bSuccess = await AsyncGenerateMesh_Internal();
        if (bSuccess == true) UpdateMesh();
    }

    private async Task<bool> AsyncGenerateMesh_Internal()
    {
        return await Task.Run(() => {
            for (int relativeX = 0; relativeX < ChunkSize; relativeX++)
            {
                for (int relativeY = 0; relativeY < ChunkSize; relativeY++)
                {
                    for (int relativeZ = 0; relativeZ < ChunkSize; relativeZ++)
                    {
                        int blockIdxX, blockIdxY, blockIdxZ;
                        blockIdxX = relativeX + _WorldDataIdxX;
                        blockIdxY = relativeY + _WorldDataIdxY;
                        blockIdxZ = relativeZ + _WorldDataIdxZ;
                        //
                        float cubeX, cubeY, cubeZ;
                        cubeX = relativeX + _RealCoordX;
                        cubeY = relativeY + _RealCoordY;
                        cubeZ = relativeZ + _RealCoordZ;
                        //
                        // 블록 생성시 6개의 면들의 위치를 조정하기 위해 추가했던 offset 값을 제거한다.
                        // x, z 는 0.5f 씩 더해주고, y는 0.5f 빼준다.
                        float blockCenterX = cubeX + 0.5f;
                        float blockCenterY = cubeY - 0.5f;
                        float blockCenterZ = cubeZ + 0.5f;
                        SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterX = blockCenterX;
                        SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterY = blockCenterY;
                        SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterZ = blockCenterZ;
                        CreateCube(blockIdxX, blockIdxY, blockIdxZ, cubeX, cubeY, cubeZ, blockCenterX, blockCenterY, blockCenterZ);
                    }
                }
            }
            return true;
        });
    }

    protected void UpdateMesh()
	{
		MeshInstance.Clear();
		MeshInstance.vertices = NewVertices.ToArray();
		MeshInstance.uv = NewUV.ToArray();
		MeshInstance.triangles = NewTriangles.ToArray();
		MeshInstance.RecalculateNormals();
		//
		switch(ChunkType)
		{
            case ChunkType.WATER:
			case ChunkType.TERRAIN:
				MeshColliderComponent.sharedMesh = MeshInstance;
				break;
		}
		//
		NewVertices.Clear();
		NewUV.Clear();
		NewTriangles.Clear();
		FaceCount = 0;
	}

	public void Release()
	{
		MeshInstance.Clear();
		NewVertices.Clear();
		NewUV.Clear();
		NewTriangles.Clear();
	}
}
