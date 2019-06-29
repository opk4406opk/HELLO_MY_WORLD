using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FloodFillDirection
{
    NONE = 0x000000,
    FRONT = 0x000001,
    BACK = 0x000010,
    TOP = 0x000100,
    BOTTOM = 0x001000,
    RIGHT = 0x010000,
    LEFT = 0x100000
}
public struct FloodFillNode
{
    public int X, Y, Z;
    public FloodFillNode(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public FloodFillNode(Vector3 point)
    {
        X = (int)point.x;
        Y = (int)point.y;
        Z = (int)point.z;
    }
    public bool IsInBoundary()
    {
        return WorldGenerateUtils.CheckBoundary(X, Y, Z);
    }
}

/// <summary>
/// 월드, 환경 생성에 사용되는 공용 유틸리티 클래스.
/// </summary>
public class WorldGenerateUtils
{
    public static bool CheckBoundary(int x, int y, int z)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        if (x >= gameWorldConfig.sub_world_x_size || x < 0) return false;
        if (y >= gameWorldConfig.sub_world_y_size || y < 0) return false;
        if (z >= gameWorldConfig.sub_world_z_size || z < 0) return false;
        return true;
    }

    public static void FloodFill(FloodFillNode centerNode, BlockTileType exceptType,
        BlockTileType replaceType, Block[,,] worldBlockData, int depth, FloodFillDirection maskDirection = FloodFillDirection.NONE)
    {
        if (depth == 0) return;
        depth--;

        FloodFillNode leftNode = new FloodFillNode(centerNode.X - 1, centerNode.Y, centerNode.Z);
        FloodFillNode rightNode = new FloodFillNode(centerNode.X + 1, centerNode.Y, centerNode.Z);
        FloodFillNode topNode = new FloodFillNode(centerNode.X, centerNode.Y + 1, centerNode.Z);
        FloodFillNode bottomNode = new FloodFillNode(centerNode.X, centerNode.Y - 1, centerNode.Z);
        FloodFillNode frontNode = new FloodFillNode(centerNode.X, centerNode.Y, centerNode.Z + 1);
        FloodFillNode backNode = new FloodFillNode(centerNode.X, centerNode.Y, centerNode.Z - 1);

        if (worldBlockData[centerNode.X, centerNode.Y, centerNode.Z].Type == (byte)exceptType)
        {
            return;
        }
        else
        {
            worldBlockData[centerNode.X, centerNode.Y, centerNode.Z].Type = (byte)replaceType;
            if (leftNode.IsInBoundary() && ((maskDirection & FloodFillDirection.LEFT) != FloodFillDirection.LEFT))
            {
                FloodFill(leftNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (rightNode.IsInBoundary() && ((maskDirection & FloodFillDirection.RIGHT) != FloodFillDirection.RIGHT))
            {
                FloodFill(rightNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (topNode.IsInBoundary() && ((maskDirection & FloodFillDirection.TOP) != FloodFillDirection.TOP))
            {
                FloodFill(topNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (bottomNode.IsInBoundary() && ((maskDirection & FloodFillDirection.BOTTOM) != FloodFillDirection.BOTTOM))
            {
                FloodFill(bottomNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (frontNode.IsInBoundary() && ((maskDirection & FloodFillDirection.FRONT) != FloodFillDirection.FRONT))
            {
                FloodFill(frontNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (backNode.IsInBoundary() && ((maskDirection & FloodFillDirection.BACK) != FloodFillDirection.BACK))
            {
                FloodFill(backNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
        }
    }

    public static int PerlinNoise(int x, int y, int z, float scale, float height, float power)
    {
        // noise value 0 to 1
        float rValue;
        rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
        rValue *= height;

        if (power != 0) rValue = Mathf.Pow(rValue, power);
        return (int)rValue;
    }
}
