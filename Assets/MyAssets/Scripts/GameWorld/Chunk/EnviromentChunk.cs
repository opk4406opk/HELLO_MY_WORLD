using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenLib;
public class EnviromentChunk : AChunk
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
        ChunkType = ChunkType.ENVIROMENT;
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
        bool bEnviromentBlock = blockType == BlockTileType.NORMAL_TREE_LEAF || blockType == BlockTileType.SQAURE_TREE_LEAF;
        bool bNotWaterBlock = blockType != BlockTileType.WATER;
        if (bEnviromentBlock == true && bNotWaterBlock == true)
        {
            CubeTopFace(cubeX, cubeY, cubeZ, blockType);
            CubeBottomFace(cubeX, cubeY, cubeZ, blockType);
            CubeNorthFace(cubeX, cubeY, cubeZ, blockType);
            CubeSouthFace(cubeX, cubeY, cubeZ, blockType);
            CubeEastFace(cubeX, cubeY, cubeZ, blockType);
            CubeWestFace(cubeX, cubeY, cubeZ, blockType);
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
            SubWorldInstance.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].OwnerChunkType = ChunkType.ENVIROMENT;
            // 월드맵에 생성된 블록의 중앙점을 이용해 Octree의 노드를 생성합니다.
            SubWorldInstance.CustomOctreeInstance.Add(new Vector3(blockCenterX, blockCenterY, blockCenterZ));
        }
    }
}
