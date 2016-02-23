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
    private MeshCollider col;

    private int faceCount;

    private int chunkSize = 0;

    private int _chunkX;
    public int chunkX
    {
        set { _chunkX = value; }
    }
    private int _chunkY;
    public int chunkY
    {
        set { _chunkY = value; }
    }
    private int _chunkZ;
    public int chunkZ
    {
        set { _chunkZ = value; }
    }
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
        col = GetComponent<MeshCollider>();
        GenerateMesh();
    }

    private void GenerateMesh()
    {

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    //This code will run for every block in the chunk
                    if (Block(x, y, z) != 0)
                    {
                        //if (Block(x, y + 1, z) == 0) CubeTop(x, y, z, Block(x, y, z));
                        //if (Block(x, y - 1, z) == 0) CubeBot(x, y, z, Block(x, y, z));
                        //if (Block(x + 1, y, z) == 0) CubeEast(x, y, z, Block(x, y, z));
                        //if (Block(x - 1, y, z) == 0) CubeWest(x, y, z, Block(x, y, z));
                        //if (Block(x, y, z + 1) == 0) CubeNorth(x, y, z, Block(x, y, z));
                        //if (Block(x, y, z - 1) == 0) CubeSouth(x, y, z, Block(x, y, z));

                        if (Block(x, y + 1, z) == 0) CubeTop(x, y, z, Block(x, y, z));
                        if (Block(x, y - 1, z) == 0) CubeBot(x, y, z, Block(x, y, z));
                        CubeNorth(x, y, z, Block(x, y, z));
                        CubeSouth(x, y, z, Block(x, y, z));
                        CubeEast(x, y, z, Block(x, y, z));
                        CubeWest(x, y, z, Block(x, y, z));
                    }

                }
            }
        }

        UpdateMesh();
    }

    private byte Block(int x, int y, int z)
    {
        return _world.Block(x + _chunkX, y + _chunkY, z + _chunkZ);
    }

    private void CubeTop(int x, int y, int z, byte block)
    {
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x, y, z));

        string tileName = worldTileData.GetTileName(Block(x, y, z));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        Cube(texturePos);
    }

    private  void CubeNorth(int x, int y, int z, byte block)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        string tileName = worldTileData.GetTileName(Block(x, y, z));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        Cube(texturePos);

    }

    private void CubeEast(int x, int y, int z, byte block)
    {

        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y, z + 1));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));

        string tileName = worldTileData.GetTileName(Block(x, y, z));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        Cube(texturePos);

    }

    private void CubeSouth(int x, int y, int z, byte block)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x + 1, y, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));

        string tileName = worldTileData.GetTileName(Block(x, y, z));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        Cube(texturePos);

    }

    private void CubeWest(int x, int y, int z, byte block)
    {

        newVertices.Add(new Vector3(x, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y, z + 1));
        newVertices.Add(new Vector3(x, y, z));
        newVertices.Add(new Vector3(x, y - 1, z));

        string tileName = worldTileData.GetTileName(Block(x, y, z));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        Cube(texturePos);

    }

    private void CubeBot(int x, int y, int z, byte block)
    {

        newVertices.Add(new Vector3(x, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z));
        newVertices.Add(new Vector3(x + 1, y - 1, z + 1));
        newVertices.Add(new Vector3(x, y - 1, z + 1));

        string tileName = worldTileData.GetTileName(Block(x, y, z));
        TileInfo tileData = worldTileData.GetTileData(tileName);

        texturePos.x = tileData.posX;
        texturePos.y = tileData.posY;

        Cube(texturePos);

    }

    private void Cube(Vector2 texturePos)
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

        col.sharedMesh = null;
        col.sharedMesh = mesh;

        newVertices.Clear();
        newUV.Clear();
        newTriangles.Clear();
        faceCount = 0;

    }
}