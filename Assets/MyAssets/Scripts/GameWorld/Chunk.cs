using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임내 블록 덩어리를 의미하는 Chunk 클래스.
/// -> 1개의 Chunk는 N개의 면으로 구성되어 하나의 메쉬로 생성된다.
/// </summary>
public class Chunk : MonoBehaviour
{
    private World _world;
    public World world
    {
        set { _world = value; }
        get { return _world; }
    }

    private List<Vector3> newVertices = new List<Vector3>();
    private List<int> newTriangles = new List<int>();
    private List<Vector2> newUV = new List<Vector2>();
    /// <summary>
    /// size per tile.
    ///  ex) 256 x 256(pixel) tileSheet 기준. 1tile(16pixel) 이 차지하는 텍스처 좌표값.  16/256
    /// </summary>
    private float tUnit;
    private Vector2 texturePos;
    private Mesh mesh;
    private int faceCount;

    private int chunkSize = 0;

    // 월드 데이터 배열에서 Chunk가 존재하는 인덱스 값( x,y,z).----------
    private int _worldDataIdxX;
    public int worldDataIdxX
    {
        set { _worldDataIdxX = value; }
    }
    private int _worldDataIdxY;
    public int worldDataIdxY
    {
        set { _worldDataIdxY = value; }
    }
    private int _worldDataIdxZ;
    public int worldDataIdxZ
    {
        set { _worldDataIdxZ = value; }
    }
    // 월드 좌표에서 실제로 Chunk가 존재하는 좌표 x,y,z.
    private float _worldCoordX;
    public float worldCoordX
    {
        set { _worldCoordX = value; }
    }
    private float _worldCoordY;
    public float worldCoordY
    {
        set { _worldCoordY = value; }
    }
    private float _worldCoordZ;
    public float worldCoordZ
    {
        set { _worldCoordZ = value; }
    }
    //-------------------------------------------------------
    private bool _update;
    public bool update
    {
        set { _update = value; }
    }

    void LateUpdate()
    {
        if (_update)
        {
            GenerateMesh();
            _update = false;
        }
    }

    public void Init()
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        chunkSize = gameConfig.chunk_size;
        tUnit = gameConfig.one_tile_unit;
        mesh = GetComponent<MeshFilter>().mesh;
        GenerateMesh();
    }

    private void GenerateMesh()
    {
        for (int relativeX = 0; relativeX < chunkSize; relativeX++)
        {
            for (int relativeY = 0; relativeY < chunkSize; relativeY++)
            {
                for (int relativeZ = 0; relativeZ < chunkSize; relativeZ++)
                {
                    int blockIdxX, blockIdxY, blockIdxZ;
                    blockIdxX = relativeX + _worldDataIdxX;
                    blockIdxY = relativeY + _worldDataIdxY;
                    blockIdxZ = relativeZ + _worldDataIdxZ;
                    //This code will run for every block in the chunk
                    if (GetBlockType(blockIdxX, blockIdxY, blockIdxZ) != TileType.NONE)
                    {
                        //if (Block(x, y + 1, z) == 0) CubeTop(x, y, z, Block(x, y, z));
                        //if (Block(x, y - 1, z) == 0) CubeBot(x, y, z, Block(x, y, z));
                        //if (B lock(x + 1, y, z) == 0) CubeEast(x, y, z, Block(x, y, z));
                        //if (Block(x - 1, y, z) == 0) CubeWest(x, y, z, Block(x, y, z));
                        //if (Block(x, y, z + 1) == 0) CubeNorth(x, y, z, Block(x, y, z));
                        //if (Block(x, y, z - 1) == 0) CubeSouth(x, y, z, Block(x, y, z));
                        //test codes.
                        float cubeX, cubeY, cubeZ;
                        cubeX = relativeX + _worldCoordX;
                        cubeY = relativeY + _worldCoordY;
                        cubeZ = relativeZ + _worldCoordZ;

                        CubeTopFace(cubeX, cubeY, cubeZ, GetBlockType(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeBotFace(cubeX, cubeY, cubeZ, GetBlockType(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeNorthFace(cubeX, cubeY, cubeZ, GetBlockType(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeSouthFace(cubeX, cubeY, cubeZ, GetBlockType(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeEastFace(cubeX, cubeY, cubeZ, GetBlockType(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeWestFace(cubeX, cubeY, cubeZ, GetBlockType(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);

                        // points 배열은 실제 블록을 생성할 때 쓰이는 8개의 포인트로 실제 월드 좌표값이다.
                        // 따라서, 이를 이용해 블록의 AABB의 Min, Max Extent 값을 정한다.
                        Vector3[] points = new Vector3[8];
                        points[0] = new Vector3(cubeX, cubeY, cubeZ);
                        points[1] = new Vector3(cubeX + 1, cubeY, cubeZ);
                        points[2] = new Vector3(cubeX + 1, cubeY, cubeZ + 1 );
                        points[3] = new Vector3(cubeX, cubeY, cubeZ + 1);
                        points[4] = new Vector3(cubeX, cubeY - 1, cubeZ);
                        points[5] = new Vector3(cubeX + 1, cubeY - 1, cubeZ);
                        points[6] = new Vector3(cubeX + 1, cubeY - 1, cubeZ + 1);
                        points[7] = new Vector3(cubeX, cubeY - 1, cubeZ + 1);
                        // 블록 생성시 6개의 면들의 위치를 조정하기 위해 추가했던 offset 값을 제거한다.
                        // x, z 는 0.5f 씩 더해주고, y는 0.5f 빼준다.
                        float blockCenterX = cubeX + 0.5f;
                        float blockCenterY = cubeY - 0.5f;
                        float blockCenterZ = cubeZ + 0.5f;
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].centerX = blockCenterX;
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].centerY = blockCenterY;
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].centerZ = blockCenterZ;
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].isRendered = true;
                        // 월드맵에 생성된 블록의 중앙점을 이용해 Octree의 노드를 생성합니다.
                        _world.customOctree.Add(new Vector3(blockCenterX, blockCenterY, blockCenterZ));
                    }

                }
            }
        }

        UpdateMesh();
    }
    
    private TileType GetBlockType(int x, int y, int z)
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        if (x >= gameConfig.sub_world_x_size ||
               x < 0 ||
               y >= gameConfig.sub_world_y_size ||
               y < 0 ||
               z >= gameConfig.sub_world_z_size ||
               z < 0)
        {
            return TileType.NONE;
        }
        return (TileType)_world.worldBlockData[x, y, z].type;
    }

    private void CubeTopFace(float x, float y, float z, TileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        TileInfo tileInfo = TileDataFile.instance.GetTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);
    }

    private  void CubeNorthFace(float x, float y, float z, TileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        TileInfo tileInfo = TileDataFile.instance.GetTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    private void CubeEastFace(float x, float y, float z, TileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        TileInfo tileInfo = TileDataFile.instance.GetTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    private void CubeSouthFace(float x, float y, float z, TileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        TileInfo tileInfo = TileDataFile.instance.GetTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    private void CubeWestFace(float x, float y, float z, TileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        TileInfo tileInfo = TileDataFile.instance.GetTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    private void CubeBotFace(float x, float y, float z, TileType tileType, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        TileInfo tileInfo = TileDataFile.instance.GetTileInfo(tileType);
        texturePos.x = tileInfo.posX;
        texturePos.y = tileInfo.posY;

        CreateFace(texturePos);

    }

    private void CreateFace(Vector2 texturePos)
    {
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 1); //2
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4); //1
        newTriangles.Add(faceCount * 4 + 2); //3
        newTriangles.Add(faceCount * 4 + 3); //4

        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y));
        newUV.Add(new Vector2(tUnit * texturePos.x + tUnit, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y + tUnit));
        newUV.Add(new Vector2(tUnit * texturePos.x, tUnit * texturePos.y));

        faceCount++; // Add this line
    }

    void UpdateMesh()
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