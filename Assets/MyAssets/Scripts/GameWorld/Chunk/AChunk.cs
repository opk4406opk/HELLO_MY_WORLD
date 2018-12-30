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

	public World world { set; get; }
    protected List<Vector3> newVertices = new List<Vector3>();
    protected List<int> newTriangles = new List<int>();
    protected List<Vector2> newUV = new List<Vector2>();

    /// <summary>
    /// size per tile.
    ///  ex) 256 x 256(pixel) CommonBlockSheet 기준. 1tile(16pixel) 이 차지하는 텍스처 좌표값.  16/256
    /// </summary>
    protected float tileUnit;
    protected Vector2 texturePos;
    protected Mesh mesh;
    protected int faceCount;

    protected int chunkSize = 0;

    protected ChunkType chunkType = ChunkType.NONE;
    public ChunkType GetChunkType() { return chunkType; }

    // 월드 데이터 배열에서 Chunk가 존재하는 인덱스 값( x,y,z).----------
    protected int _worldDataIdxX;
    public int worldDataIdxX
    {
        set { _worldDataIdxX = value; }
    }
    protected int _worldDataIdxY;
    public int worldDataIdxY
    {
        set { _worldDataIdxY = value; }
    }
    protected int _worldDataIdxZ;
    public int worldDataIdxZ
    {
        set { _worldDataIdxZ = value; }
    }
    // 월드 좌표에서 실제로 Chunk가 존재하는 좌표 x,y,z.
    protected float _realCoordX;
    public float realCoordX
    {
        set { _realCoordX = value; }
    }
    protected float _realCoordY;
    public float realCoordY
    {
        set { _realCoordY = value; }
    }
    protected float _realCoordZ;
    public float realCoordZ
    {
        set { _realCoordZ = value; }
    }
    //-------------------------------------------------------
    protected bool _update;
    public bool update
    {
        set { _update = value; }
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
        return (BlockTileType)world.worldBlockData[x, y, z].type;
    }

    protected void CubeTopFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);
    }

    protected void CubeNorthFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    protected void CubeEastFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    protected void CubeSouthFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    protected void CubeWestFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    protected void CubeBottomFace(float x, float y, float z, BlockTileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        BlockTileInfo tileInfo = BlockTileDataFile.instance.GetBlockTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    protected void CreateFace(Vector2 texturePos)
    {
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(tileUnit * texturePos.x + tileUnit, tileUnit * texturePos.y));
        newUV.Add(new Vector2(tileUnit * texturePos.x + tileUnit, tileUnit * texturePos.y + tileUnit));
        newUV.Add(new Vector2(tileUnit * texturePos.x, tileUnit * texturePos.y + tileUnit));
        newUV.Add(new Vector2(tileUnit * texturePos.x, tileUnit * texturePos.y));

        faceCount++; // Add this line
    }

    protected void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = newVertices.ToArray();
        mesh.uv = newUV.ToArray();
        mesh.triangles = newTriangles.ToArray();
        mesh.RecalculateNormals();

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();
        faceCount = 0;
    }
}
