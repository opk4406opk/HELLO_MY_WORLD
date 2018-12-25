﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterChunk : AChunk
{
    protected override void LateUpdate()
    {
        if (_update)
        {
            GenerateMesh();
            _update = false;
        }
    }
    public override void Init()
    {
        chunkType = ChunkType.WATER;
        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
        chunkSize = gameWorldConfig.chunk_size;
        tileUnit = gameWorldConfig.one_tile_unit;
        mesh = GetComponent<MeshFilter>().mesh;
        GenerateMesh();
    }

    protected override void GenerateMesh()
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
                    var blockType = GetBlockType(blockIdxX, blockIdxY, blockIdxZ);
                    if (blockType == BlockTileType.WATER)
                    {
                        float cubeX, cubeY, cubeZ;
                        cubeX = relativeX + _worldCoordX;
                        cubeY = relativeY + _worldCoordY;
                        cubeZ = relativeZ + _worldCoordZ;

                        // 중첩되는 Face들은 버텍스를 생성하지 않는다.
                        if (GetBlockType(blockIdxX, blockIdxY + 1, blockIdxZ) == BlockTileType.EMPTY) CubeTopFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        if (GetBlockType(blockIdxX, blockIdxY - 1, blockIdxZ) == BlockTileType.EMPTY) CubeBottomFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        if (GetBlockType(blockIdxX, blockIdxY, blockIdxZ + 1) == BlockTileType.EMPTY) CubeNorthFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        if (GetBlockType(blockIdxX, blockIdxY, blockIdxZ - 1) == BlockTileType.EMPTY) CubeSouthFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        if (GetBlockType(blockIdxX + 1, blockIdxY, blockIdxZ) == BlockTileType.EMPTY) CubeEastFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);
                        if (GetBlockType(blockIdxX - 1, blockIdxY, blockIdxZ) == BlockTileType.EMPTY) CubeWestFace(cubeX, cubeY, cubeZ, blockType, blockIdxX, blockIdxY, blockIdxZ);

                        // points 배열은 실제 블록을 생성할 때 쓰이는 8개의 포인트로 실제 월드 좌표값이다.
                        // 따라서, 이를 이용해 블록의 AABB의 Min, Max Extent 값을 정한다.
                        Vector3[] points = new Vector3[8];
                        points[0] = new Vector3(cubeX, cubeY, cubeZ);
                        points[1] = new Vector3(cubeX + 1, cubeY, cubeZ);
                        points[2] = new Vector3(cubeX + 1, cubeY, cubeZ + 1);
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
                        world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].centerX = blockCenterX;
                        world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].centerY = blockCenterY;
                        world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].centerZ = blockCenterZ;
                        world.worldBlockData[blockIdxX, blockIdxY, blockIdxZ].isRendered = true;
                    }

                }
            }
        }
        UpdateMesh();
    }

}
