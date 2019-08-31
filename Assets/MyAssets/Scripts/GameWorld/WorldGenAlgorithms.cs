using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGenAlgorithms
{
    private static List<Vector3> TreeSpawnCandidates = new List<Vector3>();

    public static void DefaultGenSurfaceSubWorld(Block[,,] subWorldBlockData, MakeWorldParam param)
    {
        Vector3 highestPoint = Vector3.zero;
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        // perlin 알고리즘을 이용해 지형을 생성한다.
        for (int x = 0; x < gameWorldConfig.sub_world_x_size; x++)
        {
            for (int z = 0; z < gameWorldConfig.sub_world_z_size; z++)
            {
                int internalTerrain = WorldGenerateUtils.PerlinNoise(x, 20, z, 3, KojeomUtility.RandomInteger(1, 3), 2);
                internalTerrain += param.BaseOffset;
                int surface = WorldGenerateUtils.PerlinNoise(x, 21, z, 1, KojeomUtility.RandomInteger(1, 2), 1) + 1;

                for (int y = 0; y < gameWorldConfig.sub_world_y_size; y++)
                {
                    if (y <= internalTerrain)
                    {
                        subWorldBlockData[x, y, z].Type = (byte)BlockTileDataFile.Instance.GetBlockTileInfo(BlockTileType.STONE_BIG).Type;
                    }
                    else if (y <= surface + internalTerrain)
                    {
                        subWorldBlockData[x, y, z].Type = (byte)BlockTileDataFile.Instance.GetBlockTileInfo(BlockTileType.GRASS).Type;
                        if (y > highestPoint.y)
                        {
                            highestPoint = new Vector3(x, y, z);
                        }
                    }
                    else if (y >= surface + internalTerrain && subWorldBlockData[x, y - 1, z].Type != (byte)BlockTileType.EMPTY)
                    {
                        TreeSpawnCandidates.Add(new Vector3(x, y, z));
                    }
                    
                }
            }
        }
        // caves
        GenerateSphereCaves(subWorldBlockData);
        // various trees.
        int treeSpawnCount = KojeomUtility.RandomInteger(3, 7);
        for (int spawnCnt = 0; spawnCnt < treeSpawnCount; spawnCnt++)
        {
            TreeType randTreeType = (TreeType)KojeomUtility.RandomInteger(0, (int)TreeType.COUNT);
            switch (randTreeType)
            {
                case TreeType.NORMAL:
                    EnviromentGenAlgorithms.GenerateDefaultTree(subWorldBlockData, TreeSpawnCandidates[KojeomUtility.RandomInteger(0, TreeSpawnCandidates.Count)]);
                    break;
                case TreeType.SQAURE:
                    EnviromentGenAlgorithms.GenerateSqaureTree(subWorldBlockData, TreeSpawnCandidates[KojeomUtility.RandomInteger(0, TreeSpawnCandidates.Count)]);
                    break;
            }
        }
        EnviromentGenAlgorithms.MakeDefaultWaterArea(highestPoint, subWorldBlockData);
    }

    public static void DefaultGenInternalSubWorld(Block[,,] subWorldBlockData)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        // perlin 알고리즘을 이용해 지형을 생성한다.
        for (int x = 0; x < gameWorldConfig.sub_world_x_size; x++)
        {
            for (int z = 0; z < gameWorldConfig.sub_world_z_size; z++)
            {
                for (int y = 0; y < gameWorldConfig.sub_world_y_size; y++)
                {
                    subWorldBlockData[x, y, z].Type = (byte)BlockTileDataFile.Instance.GetBlockTileInfo(BlockTileType.STONE_BIG).Type;
                }
            }
        }
        // caves
        GenerateSphereCaves(subWorldBlockData);
    }

    /// <summary>
    /// flood fill 알고리즘을 이용한 Sphere 모양의 동굴 생성.
    /// </summary>
    /// <param name="subWorldBlockData"></param>
    private static void GenerateSphereCaves(Block[,,] subWorldBlockData)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int startX = KojeomUtility.RandomInteger(3, gameWorldConfig.sub_world_x_size - 16);
        int maxX = KojeomUtility.RandomInteger(startX, gameWorldConfig.sub_world_x_size);
        //
        int startZ = KojeomUtility.RandomInteger(3, gameWorldConfig.sub_world_z_size - 16);
        int maxZ = KojeomUtility.RandomInteger(startX, gameWorldConfig.sub_world_z_size);
        //
        int maxY = KojeomUtility.RandomInteger(10, gameWorldConfig.sub_world_y_size - 10);
        int startY = KojeomUtility.RandomInteger(5, maxY - 1);
        for (int x = startX; x < maxX; x++)
        {
            for (int z = startZ; z < maxZ; z++)
            {
                for (int y = startY; y < maxY; y++)
                {
                    int cave = WorldGenerateUtils.PerlinNoise(x, y * 3, z, 2, 18, 1);
                    if (cave > y)
                    {
                        subWorldBlockData[x, y, z].Type = (byte)BlockTileDataFile.Instance.
                            GetBlockTileInfo(BlockTileType.EMPTY).Type;
                        WorldGenerateUtils.FloodFill(new FloodFillNode(x, y, z), BlockTileType.SAND, BlockTileType.EMPTY,
                                    subWorldBlockData, 4);
                    }
                }
            }
        }
    }
    
}
