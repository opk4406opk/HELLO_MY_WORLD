using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ChunkType
{
    COMMON = 0,
    WATER = 1,
    NONE = 2,
    COUNT = NONE
}

public abstract class AChunk : MonoBehaviour {

	public World World { set; get; }
    protected List<Vector3> NewVertices = new List<Vector3>();
    protected List<int> NewTriangles = new List<int>();
    protected List<Vector2> NewUV = new List<Vector2>();

    /// <summary>
    /// size per tile.
    ///  ex) 256 x 256(pixel) CommonBlockSheet 기준. 1tile(16pixel) 이 차지하는 텍스처 좌표값.  16/256
    /// </summary>
    protected float TileUnit;
    protected Vector2 TexturePos;
    protected Mesh Mesh;
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

    protected abstract void GenerateMesh();
    protected abstract void LateUpdate();
    public abstract void Init();

    protected BlockTileType GetBlockType(int x, int y, int z)
    {
        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
        if (x >= gameWorldConfig.sub_world_x_size ||
               x < 0 ||
               y >= gameWorldConfig.sub_world_y_size ||
               y < 0 ||
               z >= gameWorldConfig.sub_world_z_size ||
               z < 0)
        {
            return BlockTileType.EMPTY;
        }
        return (BlockTileType)World.WorldBlockData[x, y, z].type;
    }

    protected void CubeTopFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {
        NewVertices.Add(new Vector3(x, y, z + 1));
        NewVertices.Add(new Vector3(x + 1, y, z + 1));
        NewVertices.Add(new Vector3(x + 1, y, z));
        NewVertices.Add(new Vector3(x, y, z));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        TexturePos.x = tileInfo.posX;
        TexturePos.y = tileInfo.posY;

        CreateFace(TexturePos);
    }

    protected void CubeNorthFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        NewVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        NewVertices.Add(new Vector3(x + 1, y, z + 1));
        NewVertices.Add(new Vector3(x, y, z + 1));
        NewVertices.Add(new Vector3(x, y - 1, z + 1));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        TexturePos.x = tileInfo.posX;
        TexturePos.y = tileInfo.posY;

        CreateFace(TexturePos);

    }

    protected void CubeEastFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        NewVertices.Add(new Vector3(x + 1, y - 1, z));
        NewVertices.Add(new Vector3(x + 1, y, z));
        NewVertices.Add(new Vector3(x + 1, y, z + 1));
        NewVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        TexturePos.x = tileInfo.posX;
        TexturePos.y = tileInfo.posY;

        CreateFace(TexturePos);

    }

    protected void CubeSouthFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        NewVertices.Add(new Vector3(x, y - 1, z));
        NewVertices.Add(new Vector3(x, y, z));
        NewVertices.Add(new Vector3(x + 1, y, z));
        NewVertices.Add(new Vector3(x + 1, y - 1, z));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        TexturePos.x = tileInfo.posX;
        TexturePos.y = tileInfo.posY;

        CreateFace(TexturePos);

    }

    protected void CubeWestFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        NewVertices.Add(new Vector3(x, y - 1, z + 1));
        NewVertices.Add(new Vector3(x, y, z + 1));
        NewVertices.Add(new Vector3(x, y, z));
        NewVertices.Add(new Vector3(x, y - 1, z));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        TexturePos.x = tileInfo.posX;
        TexturePos.y = tileInfo.posY;

        CreateFace(TexturePos);

    }

    protected void CubeBottomFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        NewVertices.Add(new Vector3(x, y - 1, z));
        NewVertices.Add(new Vector3(x + 1, y - 1, z));
        NewVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        NewVertices.Add(new Vector3(x, y - 1, z + 1));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        TexturePos.x = tileInfo.posX;
        TexturePos.y = tileInfo.posY;

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

    protected void UpdateMesh()
    {
        Mesh.Clear();
        Mesh.vertices = NewVertices.ToArray();
        Mesh.uv = NewUV.ToArray();
        Mesh.triangles = NewTriangles.ToArray();
        Mesh.RecalculateNormals();
        //
        switch(ChunkType)
        {
            case ChunkType.COMMON:
                MeshColliderComponent.sharedMesh = Mesh;
                break;
        }
        //
        NewVertices.Clear();
        NewUV.Clear();
        NewTriangles.Clear();
        FaceCount = 0;
    }
}
