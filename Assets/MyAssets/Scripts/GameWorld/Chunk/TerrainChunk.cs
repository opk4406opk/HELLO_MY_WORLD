﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 게임내 블록 덩어리를 의미하는 Chunk 클래스.
/// -> 1개의 Chunk는 N개의 면으로 구성되어 하나의 메쉬로 생성된다.
/// </summary>
public class TerrainChunk : AChunk
{
    protected override void LateUpdate()
    {
        if (_Update)
        {
            GenerateMesh();
            _Update = false;
        }
    }

    public override void Init()
    {
        ChunkType = ChunkType.TERRAIN;
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        ChunkSize = gameWorldConfig.chunk_size;
        TileUnit = gameWorldConfig.one_tile_unit;
        Mesh = GetComponent<MeshFilter>().mesh;
        MeshColliderComponent = GetComponent<MeshCollider>();
        GenerateMesh();
    }

    protected override void GenerateMesh()
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
                    //This code will run for every block in the chunk
                    var blockType = GetBlockType(blockIdxX, blockIdxY, blockIdxZ);
                    if (blockType != BlockTileType.EMPTY && blockType != BlockTileType.WATER)
                    {
                        float cubeX, cubeY, cubeZ;
                        cubeX = relativeX + _RealCoordX;
                        cubeY = relativeY + _RealCoordY;
                        cubeZ = relativeZ + _RealCoordZ;

                        CubeTopFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        CubeBottomFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        CubeNorthFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        CubeSouthFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        CubeEastFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        CubeWestFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);

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
                        World.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterX = blockCenterX;
                        World.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterY = blockCenterY;
                        World.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].CenterZ = blockCenterZ;
                        World.WorldBlockData[blockIdxX, blockIdxY, blockIdxZ].bRendered = true;
                        // 월드맵에 생성된 블록의 중앙점을 이용해 Octree의 노드를 생성합니다.
                        World.CustomOctreeInstance.Add(new Vector3(blockCenterX, blockCenterY, blockCenterZ));
                    }

                }
            }
        }
        UpdateMesh();
    }
}