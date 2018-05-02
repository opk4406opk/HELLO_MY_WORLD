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
                int stone = PerlinNoise(x, 32, z, 3, Utility.RandomInteger(1, 3), 2);
                stone += param.baseOffset;
                int grass = PerlinNoise(x, 32, z, 1, Utility.RandomInteger(3, 6), 1) + 1;

                KojeomLogger.DebugLog(string.Format("stone : {0}, grass :{1}", stone, grass));

                for (int y = 0; y < gameConfig.sub_world_y_size; y++)
                {
                    if (y <= stone) refWorldBlockData[x, y, z].type = (byte)TileDataFile.instance.
                            GetTileInfo(TileType.STONE_BIG).type;
                    else if (y <= grass + stone) refWorldBlockData[x, y, z].type = (byte)TileDataFile.instance.
                            GetTileInfo(TileType.GRASS).type;
                }
            }
        }
    }

    private static void FloodFill(FloodFillNode node, byte targetType, byte replaceType, Block[,,] worldBlockData)
    {
        FloodFillNode leftNode = new FloodFillNode(node.x - 1, node.y, node.z);
        FloodFillNode rightNode = new FloodFillNode(node.x + 1, node.y, node.z);
        FloodFillNode topNode = new FloodFillNode(node.x, node.y + 1, node.z);
        FloodFillNode bottomNode = new FloodFillNode(node.x, node.y - 1, node.z);

        if(worldBlockData[node.x, node.y, node.z].type == targetType)
        {
            return;
        }
        else
        {
            worldBlockData[node.x, node.y, node.z].type = replaceType;
            if (leftNode.IsInBoundary())
            {
                FloodFill(leftNode, targetType, replaceType, worldBlockData);
            }
            if (rightNode.IsInBoundary())
            {
                FloodFill(rightNode, targetType, replaceType, worldBlockData);
            }
            if (topNode.IsInBoundary())
            {
                FloodFill(topNode, targetType, replaceType, worldBlockData);
            }
            if (bottomNode.IsInBoundary())
            {
                FloodFill(bottomNode, targetType, replaceType, worldBlockData);
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
