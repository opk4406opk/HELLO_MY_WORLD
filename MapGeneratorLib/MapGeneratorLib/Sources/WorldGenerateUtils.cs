using System.Collections;
using System.Collections.Generic;

namespace MapGenLib
{
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
        public FloodFill3DNode(CustomVector3 point)
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
        public FloodFill2DNode(CustomVector3 point)
        {
            X = (int)point.x;
            Y = (int)point.y;
        }
    }

    public struct SubWorldSize
    {
        public int SizeX;
        public int SizeY;
        public int SizeZ;

        public SubWorldSize(int x, int y, int z)
        {
            SizeX = x;
            SizeY = y;
            SizeZ = z;
        }
    }

    /// <summary>
    /// 월드, 환경 생성에 사용되는 공용 유틸리티 클래스.
    /// </summary>
    public class WorldGenerateUtils
    {
        public static bool CheckSubWorldBoundary(int x, int y, int z, SubWorldSize subWorldSize)
        {
            if (x >= subWorldSize.SizeX || x < 0) return false;
            if (y >= subWorldSize.SizeY || y < 0) return false;
            if (z >= subWorldSize.SizeZ || z < 0) return false;
            return true;
        }

        public static bool CheckSubWorldBoundary(FloodFill3DNode node, SubWorldSize subWorldSize)
        {
            if (node.X >= subWorldSize.SizeX || node.X < 0) return false;
            if (node.Y >= subWorldSize.SizeY || node.Y < 0) return false;
            if (node.Z >= subWorldSize.SizeZ || node.Z < 0) return false;
            return true;
        }

        public static bool CheckSubWorldBoundary(CustomVector3 pos, SubWorldSize subWorldSize)
        {
            if (pos.x >= subWorldSize.SizeX || pos.x < 0) return false;
            if (pos.y >= subWorldSize.SizeY || pos.y < 0) return false;
            if (pos.z >= subWorldSize.SizeZ || pos.z < 0) return false;
            return true;
        }

        public static bool CheckBoundary(int x, int y, int boundaryX, int boundaryY)
        {
            if (x < 0 || x >= boundaryX) return false;
            if (y < 0 || y >= boundaryY) return false;
            return true;
        }

        public static bool CheckBoundary(CustomVector2 location, int boundaryX, int boundaryY)
        {
            if (location.x < 0 || location.x >= boundaryX) return false;
            if (location.y < 0 || location.y >= boundaryY) return false;
            return true;
        }

        public static BlockTileType CalcTerrainValueToBlockType(int terrainScalaValue, int subWorldLayerNum, int subWorldSizeY)
        {
            BlockTileType blockType = BlockTileType.NONE;
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
            else
            {
                blockType = BlockTileType.WOOD;
            }

            return blockType;
        }

        public static void ForceNormalize8Direction(int centerX, int centerY, int[,] map)
        {
            int mapX = map.GetLength(0);
            int mapY = map.GetLength(1);

            CustomVector2 west = new CustomVector2(centerX - 1, centerY);
            CustomVector2 east = new CustomVector2(centerX + 1, centerY);
            CustomVector2 north = new CustomVector2(centerX, centerY + 1);
            CustomVector2 south = new CustomVector2(centerX, centerY - 1);
            CustomVector2 northEast = new CustomVector2(centerX + 1, centerY + 1);
            CustomVector2 northWest = new CustomVector2(centerX - 1, centerY + 1);
            CustomVector2 southEast = new CustomVector2(centerX + 1, centerY - 1);
            CustomVector2 southWest = new CustomVector2(centerX - 1, centerY - 1);

            List<CustomVector2> directions = new List<CustomVector2>();
            directions.Add(west);
            directions.Add(east);
            directions.Add(north);
            directions.Add(south);
            directions.Add(northEast);
            directions.Add(northWest);
            directions.Add(southEast);
            directions.Add(southWest);

            int sum = 0;
            List<CustomVector2> candidates = new List<CustomVector2>();
            foreach (var dir in directions)
            {
                if (CheckBoundary(dir, mapX, mapY) == true)
                {
                    sum += map[(int)dir.x, (int)dir.y];
                    candidates.Add(dir);
                }
            }

            if (candidates.Count > 0)
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

            CustomVector2 west = new CustomVector2(centerX - 1, centerY);
            CustomVector2 east = new CustomVector2(centerX + 1, centerY);
            CustomVector2 north = new CustomVector2(centerX, centerY + 1);
            CustomVector2 south = new CustomVector2(centerX, centerY - 1);
            CustomVector2 northEast = new CustomVector2(centerX + 1, centerY + 1);
            CustomVector2 northWest = new CustomVector2(centerX - 1, centerY + 1);
            CustomVector2 southEast = new CustomVector2(centerX + 1, centerY - 1);
            CustomVector2 southWest = new CustomVector2(centerX - 1, centerY - 1);

            List<CustomVector2> directions = new List<CustomVector2>();
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
                bool condtion = CheckBoundary(dir, mapX, mapY) == true && map[(int)dir.x, (int)dir.y] <= CustomMathf.Abs(waterBasisValue);
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

            CustomVector2 west = new CustomVector2(centerX - 1, centerY);
            CustomVector2 east = new CustomVector2(centerX + 1, centerY);
            CustomVector2 north = new CustomVector2(centerX, centerY + 1);
            CustomVector2 south = new CustomVector2(centerX, centerY - 1);

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
            foreach (bool include in includeDirections)
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
            BlockTileType replaceType, Block[,,] worldBlockData, int depth, SubWorldSize subWorldSize, FloodFillDirection maskDirection = FloodFillDirection.NONE)
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
                if (CheckSubWorldBoundary(leftNode, subWorldSize) == true && ((maskDirection & FloodFillDirection.LEFT) != FloodFillDirection.LEFT))
                {
                    FloodFillSubWorld(leftNode, exceptType, replaceType, worldBlockData, depth, subWorldSize, maskDirection);
                }
                if (CheckSubWorldBoundary(rightNode, subWorldSize) == true && ((maskDirection & FloodFillDirection.RIGHT) != FloodFillDirection.RIGHT))
                {
                    FloodFillSubWorld(rightNode, exceptType, replaceType, worldBlockData, depth, subWorldSize, maskDirection);
                }
                if (CheckSubWorldBoundary(topNode, subWorldSize) == true && ((maskDirection & FloodFillDirection.TOP) != FloodFillDirection.TOP))
                {
                    FloodFillSubWorld(topNode, exceptType, replaceType, worldBlockData, depth, subWorldSize, maskDirection);
                }
                if (CheckSubWorldBoundary(bottomNode, subWorldSize) == true && ((maskDirection & FloodFillDirection.BOTTOM) != FloodFillDirection.BOTTOM))
                {
                    FloodFillSubWorld(bottomNode, exceptType, replaceType, worldBlockData, depth, subWorldSize, maskDirection);
                }
                if (CheckSubWorldBoundary(frontNode, subWorldSize) == true && ((maskDirection & FloodFillDirection.FRONT) != FloodFillDirection.FRONT))
                {
                    FloodFillSubWorld(frontNode, exceptType, replaceType, worldBlockData, depth, subWorldSize, maskDirection);
                }
                if (CheckSubWorldBoundary(backNode, subWorldSize) == true && ((maskDirection & FloodFillDirection.BACK) != FloodFillDirection.BACK))
                {
                    FloodFillSubWorld(backNode, exceptType, replaceType, worldBlockData, depth, subWorldSize, maskDirection);
                }
            }
        }

        public static int PerlinNoise(int x, int y, int z, float scale, float height, float power)
        {
            // noise value 0 to 1
            float rValue;
            rValue = Noise.GetNoise(((double)x) / scale, ((double)y) / scale, ((double)z) / scale);
            rValue *= height;

            if (power != 0) rValue = CustomMathf.Pow(rValue, power);
            return (int)rValue;
        }

        // ref : https://epochabuse.com/gaussian-blur/
        public static float[,] GaussianBlur(int lenght, float weight)
        {
            float[,] kernel = new float[lenght, lenght];
            float kernelSum = 0;
            int foff = (lenght - 1) / 2;
            float distance = 0;
            float constant = 1f / (2 * CustomMathf.PI * weight * weight);
            for (int y = -foff; y <= foff; y++)
            {
                for (int x = -foff; x <= foff; x++)
                {
                    distance = ((y * y) + (x * x)) / (2 * weight * weight);
                    kernel[y + foff, x + foff] = constant * CustomMathf.Exp(-distance);
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

}
