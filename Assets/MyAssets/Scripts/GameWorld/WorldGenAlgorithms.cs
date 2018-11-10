using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGenAlgorithms {

    private static List<Vector3> treeSpawnCandidates = new List<Vector3>();

    public static bool CheckBoundary(int x, int y, int z)
    {
        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
        if (x >= gameWorldConfig.sub_world_x_size || x < 0) return false;
        if (y >= gameWorldConfig.sub_world_y_size || y < 0) return false;
        if (z >= gameWorldConfig.sub_world_z_size || z < 0) return false;
        return true;
    }

    private struct FloodFillNode
    {
        public int x, y, z;
        public FloodFillNode(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public bool IsInBoundary()
        {
            return CheckBoundary(x, y, z);
        }
    }
    public static void DefaultGenWorld(Block[,,] worldBlockData, MakeWorldParam param)
    {
        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
        // perlin 알고리즘을 이용해 지형을 생성한다.
        for (int x = 0; x < gameWorldConfig.sub_world_x_size; x++)
        {
            for (int z = 0; z < gameWorldConfig.sub_world_z_size; z++)
            {
                int stone = PerlinNoise(x, 20, z, 3, KojeomUtility.RandomInteger(1, 3), 2);
                stone += param.baseOffset;
                int grass = PerlinNoise(x, 21, z, 1, KojeomUtility.RandomInteger(1, 2), 1) + 1;

                for (int y = 0; y < gameWorldConfig.sub_world_y_size; y++)
                {
                    if (y <= stone)
                    {
                        worldBlockData[x, y, z].type = (byte)BlockTileDataFile.instance.GetBlockTileInfo(BlockTileType.STONE_BIG).type;
                    }
                    else if (y <= grass + stone)
                    {
                        worldBlockData[x, y, z].type = (byte)BlockTileDataFile.instance.GetBlockTileInfo(BlockTileType.GRASS).type;
                    }
                    else if (y >= grass + stone && worldBlockData[x, y - 1, z].type != (byte)BlockTileType.EMPTY)
                    {
                        treeSpawnCandidates.Add(new Vector3(x, y, z));
                    }
                }
            }
        }
        // caves
        GenerateSphereCaves(worldBlockData);
        // various trees.
        int treeSpawnCount = KojeomUtility.RandomInteger(3, 7);
        for (int spawnCnt = 0; spawnCnt < treeSpawnCount; spawnCnt++)
        {
            TreeType randTreeType = (TreeType)KojeomUtility.RandomInteger(0, (int)TreeType.COUNT);
            switch(randTreeType)
            {
                case TreeType.NORMAL:
                    EnvriomentGenAlgorithms.GenerateDefaultTree(worldBlockData, treeSpawnCandidates[KojeomUtility.RandomInteger(0, treeSpawnCandidates.Count)]);
                    break;
                case TreeType.SQAURE:
                    EnvriomentGenAlgorithms.GenerateSqaureTree(worldBlockData, treeSpawnCandidates[KojeomUtility.RandomInteger(0, treeSpawnCandidates.Count)]);
                    break;
            }
        }
    }

    /// <summary>
    /// flood fill 알고리즘을 이용한 Sphere 모양의 동굴 생성.
    /// </summary>
    /// <param name="worldBlockData"></param>
    private static void GenerateSphereCaves(Block[,,] worldBlockData)
    {
        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
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
                    int cave = PerlinNoise(x, y * 3, z, 2, 18, 1);
                    if (cave > y)
                    {
                        worldBlockData[x, y, z].type = (byte)BlockTileDataFile.instance.
                            GetBlockTileInfo(BlockTileType.EMPTY).type;
                        FloodFill(new FloodFillNode(x, y, z), BlockTileType.SAND, BlockTileType.EMPTY,
                                    worldBlockData, 4);
                    }
                }
            }
        }
    }

    private static void FloodFill(FloodFillNode node, BlockTileType targetType,
        BlockTileType replaceType, Block[,,] worldBlockData, int depth)
    {
        if (depth == 0) return;
        depth--;

        FloodFillNode leftNode = new FloodFillNode(node.x - 1, node.y, node.z);
        FloodFillNode rightNode = new FloodFillNode(node.x + 1, node.y, node.z);
        FloodFillNode topNode = new FloodFillNode(node.x, node.y + 1, node.z);
        FloodFillNode bottomNode = new FloodFillNode(node.x, node.y - 1, node.z);
        FloodFillNode frontNode = new FloodFillNode(node.x, node.y, node.z + 1);
        FloodFillNode backNode = new FloodFillNode(node.x, node.y, node.z -1);

        if (worldBlockData[node.x, node.y, node.z].type == (byte)targetType)
        {
            return;
        }
        else
        {
            worldBlockData[node.x, node.y, node.z].type = (byte)replaceType;
            if (leftNode.IsInBoundary())
            {
                FloodFill(leftNode, targetType, replaceType, worldBlockData, depth);
            }
            if (rightNode.IsInBoundary())
            {
                FloodFill(rightNode, targetType, replaceType, worldBlockData, depth);
            }
            if (topNode.IsInBoundary())
            {
                FloodFill(topNode, targetType, replaceType, worldBlockData, depth);
            }
            if (bottomNode.IsInBoundary())
            {
                FloodFill(bottomNode, targetType, replaceType, worldBlockData, depth);
            }
            if (frontNode.IsInBoundary())
            {
                FloodFill(frontNode, targetType, replaceType, worldBlockData, depth);
            }
            if (backNode.IsInBoundary())
            {
                FloodFill(backNode, targetType, replaceType, worldBlockData, depth);
            }
        }
    }

    private static int PerlinNoise(int x, int y, int z, float scale, float height, float power)
    {
        // noise value 0 to 1
        float rValue;
        rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
        rValue *= height;

        if (power != 0) rValue = Mathf.Pow(rValue, power);
        return (int)rValue;
    }
}
