using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenLib;
public class WaterChunk : AChunk
{
    protected override void FixedUpdate()
    {
        if (_Update)
        {
            //GenerateMesh();
            _Update = false;
            TestAsyncGenerateMesh();
        }
    }
    public override void Init()
    {
        ChunkType = ChunkType.WATER;
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        ChunkSize = gameWorldConfig.ChunkSize;
        TileUnit = gameWorldConfig.OneTileUnit;
        MeshInstance = GetComponent<MeshFilter>().mesh;
        MeshColliderComponent = GetComponent<MeshCollider>();
        //GenerateMesh();
        TestAsyncGenerateMesh();
    }

    protected override void CreateCube(int blockIdxX, int blockIdxY, int blockIdxZ, float cubeX, float cubeY, float cubeZ, float blockCenterX, float blockCenterY, float blockCenterZ)
    {
        //This code will run for every block in the chunk
        var blockType = GetBlockType(blockIdxX, blockIdxY, blockIdxZ);
        if (blockType == BlockTileType.WATER)
        {
            // 중첩되는 Face들은 버텍스를 생성하지 않는다.
            if (GetBlockType(blockIdxX, blockIdxY + 1, blockIdxZ) == BlockTileType.EMPTY) CubeTopFace(cubeX, cubeY, cubeZ, blockType);
            if (GetBlockType(blockIdxX, blockIdxY - 1, blockIdxZ) == BlockTileType.EMPTY) CubeBottomFace(cubeX, cubeY, cubeZ, blockType);
            if (GetBlockType(blockIdxX, blockIdxY, blockIdxZ + 1) == BlockTileType.EMPTY) CubeFrontFace(cubeX, cubeY, cubeZ, blockType);
            if (GetBlockType(blockIdxX, blockIdxY, blockIdxZ - 1) == BlockTileType.EMPTY) CubeBackFace(cubeX, cubeY, cubeZ, blockType);
            if (GetBlockType(blockIdxX + 1, blockIdxY, blockIdxZ) == BlockTileType.EMPTY) CubeRightFace(cubeX, cubeY, cubeZ, blockType);
            if (GetBlockType(blockIdxX - 1, blockIdxY, blockIdxZ) == BlockTileType.EMPTY) CubeLeftFace(cubeX, cubeY, cubeZ, blockType);
            // points 배열은 실제 블록을 생성할 때 쓰이는 8개의 포인트로 실제 월드 좌표값이다.
            // 따라서, 이를 이용해 블록의 AABB의 Min, Max Extent 값을 정한다.
            //Vector3[] points = new Vector3[8];
            //points[0] = new Vector3(cubeX, cubeY, cubeZ);
            //points[1] = new Vector3(cubeX + 1, cubeY, cubeZ);
            //points[2] = new Vector3(cubeX + 1, cubeY, cubeZ + 1);
            //points[3] = new Vector3(cubeX, cubeY, cubeZ + 1);
            //points[4] = new Vector3(cubeX, cubeY - 1, cubeZ);
            //points[5] = new Vector3(cubeX + 1, cubeY - 1, cubeZ);
            //points[6] = new Vector3(cubeX + 1, cubeY - 1, cubeZ + 1);
            //points[7] = new Vector3(cubeX, cubeY - 1, cubeZ + 1);
            //                       
            SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].bRendered = true;
            SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].OwnerChunkType = ChunkType.WATER;
        }
    }

}

