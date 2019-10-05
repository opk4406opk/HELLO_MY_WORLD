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

    public static bool CheckBoundary(Vector2 location, int boundaryX, int boundaryY)
    {
        if (location.x < 0 || location.x >= boundaryX) return false;
        if (location.y < 0 || location.y >= boundaryY) return false;
        return true;
    }

    public static BlockTileType CalcTerrainValueToBlockType(int terrainScalaValue)
    {
        BlockTileType blockType = BlockTileType.STONE_SILVER;
        int subWorldLayerNum = WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldLayer;
        int subWorldSizeY = WorldConfigFile.Instance.GetConfig().SubWorldSizeY;
        int range = subWorldLayerNum * subWorldSizeY;
        int min = -1 * range;
        int max = range;
        int half = max / 2;
        int underHalf = -1 * half;

        bool bWater = terrainScalaValue == 6;
        bool bStone = terrainScalaValue < half && terrainScalaValue >= 0;
        bool bGrass = terrainScalaValue < max && terrainScalaValue >= half;

        if (bWater == true)
        {
            blockType = BlockTileType.WATER;
        }
        else if (bStone == true)
        {
            blockType = BlockTileType.STONE_SMALL;
        }
        else if (bGrass == true)
        {
            blockType = BlockTileType.GRASS;
        }
        return blockType;
    }

    public static void ForceNormalize8Direction(int centerX, int centerY, int[,] map)
    {
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);

        Vector2 west = new Vector2(centerX - 1, centerY);
        Vector2 east = new Vector2(centerX + 1, centerY);
        Vector2 north = new Vector2(centerX, centerY + 1);
        Vector2 south = new Vector2(centerX, centerY - 1);
        Vector2 northEast = new Vector2(centerX + 1, centerY + 1);
        Vector2 northWest = new Vector2(centerX - 1, centerY + 1);
        Vector2 southEast = new Vector2(centerX + 1, centerY - 1);
        Vector2 southWest = new Vector2(centerX - 1, centerY - 1);

        List<Vector2> directions = new List<Vector2>();
        directions.Add(west);
        directions.Add(east);
        directions.Add(north);
        directions.Add(south);
        directions.Add(northEast);
        directions.Add(northWest);
        directions.Add(southEast);
        directions.Add(southWest);

        int sum = 0;
        List<Vector2> candidates = new List<Vector2>();
        foreach(var dir in directions)
        {
            if(CheckBoundary(dir, mapX, mapY) == true)
            {
                sum += map[(int)dir.x, (int)dir.y];
                candidates.Add(dir);
            }
        }

        if(candidates.Count > 0)
        {
            int averageTerrainValue = sum / candidates.Count;
            foreach (var candidate in candidates)
            {
                map[(int)candidate.x, (int)candidate.y] = averageTerrainValue;
            }
        }
    }

    public static void NormalizeWaterTerrain(int centerX, int centerY, int[,] map, int waterBasisValue, int blockHeight)
    {
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);

        Vector2 west = new Vector2(centerX - 1, centerY);
        Vector2 east = new Vector2(centerX + 1, centerY);
        Vector2 north = new Vector2(centerX, centerY + 1);
        Vector2 south = new Vector2(centerX, centerY - 1);
        Vector2 northEast = new Vector2(centerX + 1, centerY + 1);
        Vector2 northWest = new Vector2(centerX - 1, centerY + 1);
        Vector2 southEast = new Vector2(centerX + 1, centerY - 1);
        Vector2 southWest = new Vector2(centerX - 1, centerY - 1);

        List<Vector2> directions = new List<Vector2>();
        directions.Add(west);
        directions.Add(east);
        directions.Add(north);
        directions.Add(south);
        directions.Add(northEast);
        directions.Add(northWest);
        directions.Add(southEast);
        directions.Add(southWest);

        // 주변
        foreach (var dir in directions)
        {
            bool condtion = CheckBoundary(dir, mapX, mapY) == true && map[(int)dir.x, (int)dir.y] <= Mathf.Abs(waterBasisValue);
            if (condtion == true)
            {
                map[(int)dir.x, (int)dir.y] = blockHeight;
            }
        }
        // 중심.
        map[centerX, centerY] = blockHeight;
    }

    public static void NormalizeCrossDirection(int centerX, int centerY, int[,] map, int normalizeBasisValue, int normalizeDivideFactor = 1)
    {
        int mapX = map.GetLength(0);
        int mapY = map.GetLength(1);

        Vector2 west = new Vector2(centerX - 1, centerY);
        Vector2 east = new Vector2(centerX + 1, centerY);
        Vector2 north = new Vector2(centerX, centerY + 1);
        Vector2 south = new Vector2(centerX , centerY - 1);

        bool[] includeDirections = new bool[4];
        includeDirections[0] = CheckBoundary(west, mapX, mapY) == true && map[(int)west.x, (int)west.y] <= normalizeBasisValue; // west
        includeDirections[1] = CheckBoundary(east, mapX, mapY) == true && map[(int)east.x, (int)east.y] <= normalizeBasisValue; // east
        includeDirections[2] = CheckBoundary(north, mapX, mapY) == true && map[(int)north.x, (int)north.y] <= normalizeBasisValue; // north
        includeDirections[3] = CheckBoundary(south, mapX, mapY) == true && map[(int)south.x, (int)south.y] <= normalizeBasisValue; // south

        bool isIncludeAnyDirection = includeDirections[0] | includeDirections[1] | includeDirections[2] | includeDirections[3];
        if (isIncludeAnyDirection == false) return;

        int sum = 0;
        sum += includeDirections[0] ? map[(int)west.x, (int)west.y] : 0;
        sum += includeDirections[1] ? map[(int)east.x, (int)east.y] : 0;
        sum += includeDirections[2] ? map[(int)north.x, (int)north.y] : 0;
        sum += includeDirections[3] ? map[(int)south.x, (int)south.y] : 0;

        int num = 0;
        foreach(bool include in includeDirections)
        {
            if (include == true) num++; 
        }

        int normalizeValue = (sum / num) / normalizeDivideFactor;
        if (includeDirections[0] == true) map[(int)west.x, (int)west.y] = normalizeValue;
        if (includeDirections[1] == true) map[(int)east.x, (int)east.y] = normalizeValue;
        if (includeDirections[2] == true) map[(int)north.x, (int)north.y] = normalizeValue;
        if (includeDirections[3] == true) map[(int)south.x, (int)south.y] = normalizeValue;
        map[centerX, centerY] = normalizeValue;
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

    // ref : https://epochabuse.com/gaussian-blur/
    public static float[,] GaussianBlur(int lenght, float weight)
    {
        float[,] kernel = new float[lenght, lenght];
        float kernelSum = 0;
        int foff = (lenght - 1) / 2;
        float distance = 0;
        float constant = 1f / (2 * Mathf.PI * weight * weight);
        for (int y = -foff; y <= foff; y++)
        {
            for (int x = -foff; x <= foff; x++)
            {
                distance = ((y * y) + (x * x)) / (2 * weight * weight);
                kernel[y + foff, x + foff] = constant * Mathf.Exp(-distance);
                kernelSum += kernel[y + foff, x + foff];
            }
        }
        for (int y = 0; y < lenght; y++)
        {
            for (int x = 0; x < lenght; x++)
            {
                kernel[y, x] = kernel[y, x] * 1f / kernelSum;
            }
        }
        return kernel;
    }
}
