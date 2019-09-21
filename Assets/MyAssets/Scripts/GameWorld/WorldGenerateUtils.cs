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
public struct FloodFill3DNode
{
    public int X, Y, Z;
    public FloodFill3DNode(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
    public FloodFill3DNode(Vector3 point)
    {
        X = (int)point.x;
        Y = (int)point.y;
        Z = (int)point.z;
    }
}

public struct FloodFill2DNode
{
    public int X, Y;
    public FloodFill2DNode(int x, int y)
    {
        X = x;
        Y = y;
    }
    public FloodFill2DNode(Vector3 point)
    {
        X = (int)point.x;
        Y = (int)point.y;
    }
}

/// <summary>
/// 월드, 환경 생성에 사용되는 공용 유틸리티 클래스.
/// </summary>
public class WorldGenerateUtils
{
    public static bool CheckSubWorldBoundary(int x, int y, int z)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        if (x >= gameWorldConfig.SubWorldSizeX || x < 0) return false;
        if (y >= gameWorldConfig.SubWorldSizeY || y < 0) return false;
        if (z >= gameWorldConfig.SubWorldSizeZ || z < 0) return false;
        return true;
    }

    public static bool CheckSubWorldBoundary(FloodFill3DNode node)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        if (node.X >= gameWorldConfig.SubWorldSizeX || node.X < 0) return false;
        if (node.Y >= gameWorldConfig.SubWorldSizeY || node.Y < 0) return false;
        if (node.Z >= gameWorldConfig.SubWorldSizeZ || node.Z < 0) return false;
        return true;
    }

    public static bool CheckSubWorldBoundary(Vector3 pos)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        if (pos.x >= gameWorldConfig.SubWorldSizeX || pos.x < 0) return false;
        if (pos.y >= gameWorldConfig.SubWorldSizeY || pos.y < 0) return false;
        if (pos.z >= gameWorldConfig.SubWorldSizeZ || pos.z < 0) return false;
        return true;
    }

    public static bool CheckBoundary(int x, int y, int boundaryX, int boundaryY)
    {
        if (x < 0 || x >= boundaryX) return false;
        if (y < 0 || y >= boundaryY) return false;
        return true;
    }

    public static void NormalizeCrossDirection(int centerX, int centerY, int[,] map, int normalizeBasis)
    {
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);

        int leftX = centerX - 1, leftY = centerY;
        int rightX = centerX + 1, rightY = centerY;
        int upX = centerX, upY = centerY + 1;
        int downX = centerX, downY = centerY - 1;

        bool[] includeDirections = new bool[4];
        includeDirections[0] = CheckBoundary(leftX, leftY, mapX, mapY) == true && map[leftX, leftY] <= normalizeBasis; // left
        includeDirections[1] = CheckBoundary(rightX, rightY, mapX, mapY) == true && map[rightX, rightY] <= normalizeBasis; // right
        includeDirections[2] = CheckBoundary(upX, upY, mapX, mapY) == true && map[upX, upY] <= normalizeBasis; // up
        includeDirections[3] = CheckBoundary(downX, downY, mapX, mapY) == true && map[downX, downY] <= normalizeBasis; // down

        bool isIncludeAnyDirection = includeDirections[0] | includeDirections[1] | includeDirections[2] | includeDirections[3];
        if (isIncludeAnyDirection == false) return;

        int sum = 0;
        sum += includeDirections[0] ? map[leftX, leftY] : 0;
        sum += includeDirections[1] ? map[rightX, rightY] : 0;
        sum += includeDirections[2] ? map[upX, upY] : 0;
        sum += includeDirections[3] ? map[downX, downY] : 0;

        int num = 0;
        foreach(bool include in includeDirections)
        {
            if (include == true) num++; 
        }

        int nomalizeValue = sum / num;
        if (includeDirections[0] == true) map[leftX, leftY] = nomalizeValue;
        if (includeDirections[1] == true) map[rightX, rightY] = nomalizeValue;
        if (includeDirections[2] == true) map[upX, upY] = nomalizeValue;
        if (includeDirections[3] == true) map[downX, downY] = nomalizeValue;
    }

    public static void FloodFillSubWorld(FloodFill3DNode centerNode, BlockTileType exceptType,
        BlockTileType replaceType, Block[,,] worldBlockData, int depth, FloodFillDirection maskDirection = FloodFillDirection.NONE)
    {
        if (depth == 0) return;
        depth--;

        FloodFill3DNode leftNode = new FloodFill3DNode(centerNode.X - 1, centerNode.Y, centerNode.Z);
        FloodFill3DNode rightNode = new FloodFill3DNode(centerNode.X + 1, centerNode.Y, centerNode.Z);
        FloodFill3DNode topNode = new FloodFill3DNode(centerNode.X, centerNode.Y + 1, centerNode.Z);
        FloodFill3DNode bottomNode = new FloodFill3DNode(centerNode.X, centerNode.Y - 1, centerNode.Z);
        FloodFill3DNode frontNode = new FloodFill3DNode(centerNode.X, centerNode.Y, centerNode.Z + 1);
        FloodFill3DNode backNode = new FloodFill3DNode(centerNode.X, centerNode.Y, centerNode.Z - 1);

        if (worldBlockData[centerNode.X, centerNode.Y, centerNode.Z].Type == (byte)exceptType)
        {
            return;
        }
        else
        {
            worldBlockData[centerNode.X, centerNode.Y, centerNode.Z].Type = (byte)replaceType;
            if (CheckSubWorldBoundary(leftNode) == true && ((maskDirection & FloodFillDirection.LEFT) != FloodFillDirection.LEFT))
            {
                FloodFillSubWorld(leftNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (CheckSubWorldBoundary(rightNode) == true && ((maskDirection & FloodFillDirection.RIGHT) != FloodFillDirection.RIGHT))
            {
                FloodFillSubWorld(rightNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (CheckSubWorldBoundary(topNode) == true && ((maskDirection & FloodFillDirection.TOP) != FloodFillDirection.TOP))
            {
                FloodFillSubWorld(topNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (CheckSubWorldBoundary(bottomNode) == true && ((maskDirection & FloodFillDirection.BOTTOM) != FloodFillDirection.BOTTOM))
            {
                FloodFillSubWorld(bottomNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (CheckSubWorldBoundary(frontNode) == true && ((maskDirection & FloodFillDirection.FRONT) != FloodFillDirection.FRONT))
            {
                FloodFillSubWorld(frontNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
            }
            if (CheckSubWorldBoundary(backNode) == true && ((maskDirection & FloodFillDirection.BACK) != FloodFillDirection.BACK))
            {
                FloodFillSubWorld(backNode, exceptType, replaceType, worldBlockData, depth, maskDirection);
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
