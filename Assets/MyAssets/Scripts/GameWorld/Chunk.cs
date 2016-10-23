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
    // 256 x 256 tile 기준. 1tile(16pixel) 이 차지하는 텍스처 좌표값.  16/256
    private float tUnit = 0.0625f;
    private Vector2 texturePos;
    private Mesh mesh;
    private int faceCount;

    private int chunkSize = 0;

    // 월드 데이터 배열에서 Chunk가 존재하는 idx x,y,z.----------
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

    private TileDataFile worldTileData;

    void LateUpdate()
    {
        if (_update)
        {
            GenerateMesh();
            _update = false;
        }
    }

    public void Init(TileDataFile tileDataFile)
    {
        worldTileData = tileDataFile;
        chunkSize = GameWorldConfig.chunkSize;
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
                    if (CheckBlock(blockIdxX, blockIdxY, blockIdxZ) != 0)
                    {
                        // why use these??
                        //if (Block(x, y + 1, z) == 0) CubeTop(x, y, z, Block(x, y, z));
                        //if (Block(x, y - 1, z) == 0) CubeBot(x, y, z, Block(x, y, z));
                        //if (B lock(x + 1, y, z) == 0) CubeEast(x, y, z, Block(x, y, z));
                        //if (Block(x - 1, y, z) == 0) CubeWest(x, y, z, Block(x, y, z));
                        //if (Block(x, y, z + 1) == 0) CubeNorth(x, y, z, Block(x, y, z));
                        //if (Block(x, y, z - 1) == 0) CubeSouth(x, y, z, Block(x, y, z));
                        //test codes.
                        float worldCoordX, worldCoordY, worldCoordZ;
                        worldCoordX = relativeX + _worldCoordX;
                        worldCoordY = relativeY + _worldCoordY;
                        worldCoordZ = relativeZ + _worldCoordZ;

                        //if (CheckBlock(blockIdxX, blockIdxY + 1, blockIdxZ) == 0)
                        CubeTopFace(worldCoordX, worldCoordY, worldCoordZ, CheckBlock(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        //if (CheckBlock(blockIdxX, blockIdxY - 1, blockIdxZ) == 0)
                        CubeBotFace(worldCoordX, worldCoordY, worldCoordZ, CheckBlock(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeNorthFace(worldCoordX, worldCoordY, worldCoordZ, CheckBlock(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeSouthFace(worldCoordX, worldCoordY, worldCoordZ, CheckBlock(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeEastFace(worldCoordX, worldCoordY, worldCoordZ, CheckBlock(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);
                        CubeWestFace(worldCoordX, worldCoordY, worldCoordZ, CheckBlock(blockIdxX, blockIdxY, blockIdxZ), blockIdxX, blockIdxY, blockIdxZ);

                        Vector3[] points = new Vector3[8];
                        points[0] = new Vector3(worldCoordX, worldCoordY, worldCoordZ);
                        points[1] = new Vector3(worldCoordX + 1, worldCoordY, worldCoordZ);
                        points[2] = new Vector3(worldCoordX + 1, worldCoordY, worldCoordZ + 1 );
                        points[3] = new Vector3(worldCoordX, worldCoordY, worldCoordZ + 1);
                        points[4] = new Vector3(worldCoordX, worldCoordY - 1, worldCoordZ);
                        points[5] = new Vector3(worldCoordX + 1, worldCoordY - 1, worldCoordZ);
                        points[6] = new Vector3(worldCoordX + 1, worldCoordY - 1, worldCoordZ + 1);
                        points[7] = new Vector3(worldCoordX, worldCoordY - 1, worldCoordZ + 1);
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].worldPos = new Vector3(worldCoordX, worldCoordY, worldCoordZ);
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].belongWorld = world.worldName;
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].aabb.MakeAABB(points);
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].aabb.centerPos = new Vector3(worldCoordX, worldCoordY, worldCoordZ);
                        _world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].aabb.isEnable = true;
                    }

                }
            }
        }

        UpdateMesh();
    }
    
    private byte CheckBlock(int x, int y, int z)
    {
        if (x >= GameWorldConfig.worldX ||
               x < 0 ||
               y >= GameWorldConfig.worldY ||
               y < 0 ||
               z >= GameWorldConfig.worldZ ||
               z < 0)
        {
            return (byte)1;
        }
        return _world.worldBlockData[x, y, z].type;
    }

    private void CubeTopFace(float x, float y, float z, byte block, int blockIdxX, int blockIdxY, int blockIdxZ)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        string tileName = worldTileData.GetTileName(CheckBlock(blockIdxX, blockIdxY, blockIdxZ));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        CreateFace(texturePos);
    }

    private  void CubeNorthFace(float x, float y, float z, byte block, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        string tileName = worldTileData.GetTileName(CheckBlock(blockIdxX, blockIdxY, blockIdxZ));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        CreateFace(texturePos);

    }

    private void CubeEastFace(float x, float y, float z, byte block, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        string tileName = worldTileData.GetTileName(CheckBlock(blockIdxX, blockIdxY, blockIdxZ));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        CreateFace(texturePos);

    }

    private void CubeSouthFace(float x, float y, float z, byte block, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        string tileName = worldTileData.GetTileName(CheckBlock(blockIdxX, blockIdxY, blockIdxZ));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        CreateFace(texturePos);

    }

    private void CubeWestFace(float x, float y, float z, byte block, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        string tileName = worldTileData.GetTileName(CheckBlock(blockIdxX, blockIdxY, blockIdxZ));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        CreateFace(texturePos);

    }

    private void CubeBotFace(float x, float y, float z, byte block, int blockIdxX, int blockIdxY, int blockIdxZ)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        string tileName = worldTileData.GetTileName(CheckBlock(blockIdxX, blockIdxY, blockIdxZ));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

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
        mesh.Optimize();
        mesh.RecalculateNormals();

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();
        faceCount = 0;
    }
}