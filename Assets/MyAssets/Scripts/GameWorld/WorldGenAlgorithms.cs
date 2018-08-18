using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenAlgorithms {
    
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
            var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
            if (x >= gameConfig.sub_world_x_size || x < 0) return false;
            if (y >= gameConfig.sub_world_y_size || y < 0) return false;
            if (z >= gameConfig.sub_world_z_size || z < 0) return false;
            return true;
        }
    }
    public static void DefaultGenWorld(Block[,,] refWorldBlockData, MakeWorldParam param)
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        // perlin 알고리즘을 이용해 지형을 생성한다.
        for (int x = 0; x < gameConfig.sub_world_x_size; x++)
        {
            for (int z = 0; z < gameConfig.sub_world_z_size; z++)
            {
                int stone = PerlinNoise(x, 20, z, 3, KojeomUtility.RandomInteger(1, 3), 2);
                stone += param.baseOffset;
                int grass = PerlinNoise(x, 21, z, 1, KojeomUtility.RandomInteger(1, 2), 1) + 1;

                for (int y = 0; y < gameConfig.sub_world_y_size; y++)
                {
                    if (y <= stone) refWorldBlockData[x, y, z].type = (byte)TileDataFile.instance.
                            GetTileInfo(TileType.STONE_BIG).type;
                    else if (y <= grass + stone) refWorldBlockData[x, y, z].type = (byte)TileDataFile.instance.
                            GetTileInfo(TileType.GRASS).type;
                }
            }
        }
        SimpleGenSphereCave(refWorldBlockData);
    }

    /// <summary>
    /// flood fill 알고리즘을 이용한 Sphere 모양의 동굴 생성.
    /// </summary>
    /// <param name="refWorldBlockData"></param>
    private static void SimpleGenSphereCave(Block[,,] refWorldBlockData)
    {
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        int startX = KojeomUtility.RandomInteger(3, gameConfig.sub_world_x_size - 16);
        int maxX = KojeomUtility.RandomInteger(startX, gameConfig.sub_world_x_size);
        //
        int startZ = KojeomUtility.RandomInteger(3, gameConfig.sub_world_z_size - 16);
        int maxZ = KojeomUtility.RandomInteger(startX, gameConfig.sub_world_z_size);
        //
        int maxY = KojeomUtility.RandomInteger(10, gameConfig.sub_world_y_size - 10);
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
                        refWorldBlockData[x, y, z].type = (byte)TileDataFile.instance.
                            GetTileInfo(TileType.NONE).type;
                        FloodFill(new FloodFillNode(x, y, z), TileType.SAND, TileType.NONE,
                                    refWorldBlockData, 4);
                    }
                }
            }
        }
    }

    private static void FloodFill(FloodFillNode node, TileType targetType,
        TileType replaceType, Block[,,] worldBlockData, int depth)
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
