using System;
using System.Collections;
using System.Collections.Generic;
using MapGenLib;

namespace MapGenLib
{
    /// <summary>
    /// 블록 tile의 type 클래스.
    /// </summary>
    public enum BlockTileType
    {
        NONE = 0,
        EMPTY = 1,
        GRASS = 2,
        STONE_BIG = 3,
        STONE_SMALL = 4,
        SAND = 5,
        RED_STONE = 6,
        WOOD = 7,
        STONE_GOLD = 8,
        STONE_IRON = 9,
        STONE_SILVER = 10,
        NORMAL_TREE_LEAF = 11,
        SQAURE_TREE_LEAF = 12,
        WATER = 13
    }
    public enum ChunkType
    {
        TERRAIN = 0, // 지형 ( 동굴, 땅..)
        WATER = 1, // 물.
        ENVIROMENT = 2, // 환경 ( 나무, 풀..)
        NONE = 3,
        COUNT = NONE
    }

    /// <summary>
    /// Block
    /// (1 x 1 x 1(unit))
    /// </summary>
    [Serializable]
    public struct Block
    {
        public byte CurrentType;
        public byte OriginalType;
        public float CenterX;
        public float CenterY;
        public float CenterZ;
        public bool bRendered;
        public int WorldDataIndexX;
        public int WorldDataIndexY;
        public int WorldDataIndexZ;
        public int Durability;
        public ChunkType OwnerChunkType; // 이 블록을 소유한 청크의 타입.
                                         // 복사 생성자.
        public Block(Block b)
        {
            CurrentType = b.CurrentType;
            OriginalType = b.OriginalType;
            CenterX = b.CenterX;
            CenterY = b.CenterY;
            CenterZ = b.CenterZ;
            bRendered = b.bRendered;
            WorldDataIndexX = b.WorldDataIndexX;
            WorldDataIndexY = b.WorldDataIndexY;
            WorldDataIndexZ = b.WorldDataIndexZ;
            Durability = b.Durability;
            OwnerChunkType = b.OwnerChunkType;
        }

        public CustomVector3 GetCenterPosition()
        {
            return new CustomVector3(CenterX, CenterY, CenterZ);
        }
    }
    public class WorldGenAlgorithms
    {
        private static List<CustomVector3> TreeSpawnCandidates = new List<CustomVector3>();

        public struct TerrainValue
        {
            public BlockTileType BlockType;
            public List<int> Layers;
        }
        public struct MakeWorldParam
        {
            public int BaseOffset;
        }

        public static TerrainValue[,] GenerateUndergroundTerrain(int areaSizeX, int areaSizeZ, int subWorldLayerNum, int subWorldSizeY, int randomSeed)
        {
            Utilitys.ChangeSeed(randomSeed);

            TerrainValue[,] terrainValues = new TerrainValue[areaSizeX, areaSizeZ];
            for (int x = 0; x < areaSizeX; x++)
            {
                for (int z = 0; z < areaSizeZ; z++)
                {
                    terrainValues[x, z].BlockType = BlockTileType.STONE_SMALL;
                    terrainValues[x, z].Layers = new List<int>();
                    for (int layer = 0; layer < subWorldLayerNum; layer++)
                    {
                        terrainValues[x, z].Layers.Add(subWorldSizeY);
                    }
                }
            }
            return terrainValues;
        }

        public static TerrainValue[,] GenerateNormalTerrain(int areaSizeX, int areaSizeZ, int subWorldLayerNum, int subWorldSizeY, int randomSeed, int generateNumber = 800)
        {
            Utilitys.ChangeSeed(randomSeed);
            //
            int[,] xzPlane = new int[areaSizeX, areaSizeZ];
            //
            int rangeValue = subWorldLayerNum * subWorldSizeY;
            int rangeHeightMin = -1 * rangeValue;
            int rangeHeightMax = rangeValue;
            CustomVector2[] startPoints = new CustomVector2[4];
            startPoints[0] = new CustomVector2(0, 0);
            startPoints[1] = new CustomVector2(0, areaSizeZ);
            startPoints[2] = new CustomVector2(areaSizeX, areaSizeZ);
            startPoints[3] = new CustomVector2(areaSizeX, 0);
            for (int loop = 0; loop < generateNumber; loop++)
            {
                CustomVector2 point1 = startPoints[Utilitys.RandomInteger(0, 4)];
                CustomVector2 point2 = new CustomVector2(Utilitys.RandomInteger(areaSizeX / 3, areaSizeX), Utilitys.RandomInteger(areaSizeZ / 3, areaSizeZ));
                CustomVector2 lineVector = point2 - point1;
                for (int x = 0; x < areaSizeX; x++)
                {
                    for (int z = 0; z < areaSizeZ; z++)
                    {
                        CustomVector2 point = new CustomVector2(x, z);
                        float dirValue = CustomVector3.Cross(new CustomVector3(point.x, point.y, 0.0f),
                                                             new CustomVector3(lineVector.x, lineVector.y, 0.0f)).z;
                        if (dirValue > 0)
                        {
                            if (Utilitys.RandomBool() == true) xzPlane[x, z]++;
                            else xzPlane[x, z]--;
                            xzPlane[x, z]++;

                        }
                        else if (dirValue <= 0)
                        {
                            if (Utilitys.RandomBool() == true) xzPlane[x, z]--;
                            else xzPlane[x, z]++;
                            xzPlane[x, z]--;
                        }
                        xzPlane[x, z] = CustomMathf.Clamp(xzPlane[x, z], rangeHeightMin, rangeHeightMax);
                    }
                }
            }

            // Normalize Terrain.
            int waterBasisValue = 0;
            int heightBasisValue = rangeHeightMax / 4;
            for (int x = 0; x < areaSizeX; x++)
            {
                for (int z = 0; z < areaSizeZ; z++)
                {
                    // Water 지형이라면, 평준화 시킨다.
                    if (xzPlane[x, z] <= waterBasisValue)
                    {
                        WorldGenerateUtils.NormalizeWaterTerrain(x, z, xzPlane, waterBasisValue, 6);
                    }
                    else if (xzPlane[x, z] >= heightBasisValue)
                    {
                        WorldGenerateUtils.ForceNormalize8Direction(x, z, xzPlane);
                    }
                }
            }

            // Calc Range per Chunk size.
            TerrainValue[,] terrainValues = new TerrainValue[areaSizeX, areaSizeZ];
            for (int x = 0; x < areaSizeX; x++)
            {
                for (int z = 0; z < areaSizeZ; z++)
                {
                    terrainValues[x, z].BlockType = WorldGenerateUtils.CalcTerrainValueToBlockType(xzPlane[x, z], subWorldLayerNum, subWorldSizeY);
                    terrainValues[x, z].Layers = new List<int>();
                    int absTerrainScalaValue = CustomMathf.Abs(xzPlane[x, z]);
                    for (int layer = 0; layer < subWorldLayerNum; layer++)
                    {
                        int rangeY = 0;
                        if (absTerrainScalaValue <= 0)
                        {
                            absTerrainScalaValue = 0;
                            rangeY = absTerrainScalaValue;
                        }
                        else if (absTerrainScalaValue < subWorldSizeY)
                        {
                            rangeY = absTerrainScalaValue;
                            absTerrainScalaValue -= subWorldSizeY;
                        }
                        else if (absTerrainScalaValue >= subWorldSizeY)
                        {
                            rangeY = subWorldSizeY;
                            absTerrainScalaValue -= subWorldSizeY;
                        }
                        //
                        terrainValues[x, z].Layers.Add(CustomMathf.Abs(rangeY));
                    }
                }
            }
            return terrainValues;
        }

        public static void GenerateSubWorldWithPerlinNoise(Block[,,] subWorldBlockData, MakeWorldParam param, SubWorldSize subWorldSize)
        {
            CustomVector3 highestPoint = CustomVector3.zero;
            // perlin 알고리즘을 이용해 지형을 생성한다.
            for (int x = 0; x < subWorldSize.SizeX; x++)
            {
                for (int z = 0; z < subWorldSize.SizeZ; z++)
                {
                    int internalTerrain = WorldGenerateUtils.PerlinNoise(x, 20, z, 3, Utilitys.RandomInteger(1, 3), 2);
                    internalTerrain += param.BaseOffset;
                    int surface = WorldGenerateUtils.PerlinNoise(x, 21, z, 1, Utilitys.RandomInteger(1, 2), 1) + 1;

                    for (int y = 0; y < subWorldSize.SizeY; y++)
                    {
                        if (y <= internalTerrain)
                        {
                            subWorldBlockData[x, y, z].CurrentType = (byte)BlockTileType.STONE_BIG;
                        }
                        else if (y <= surface + internalTerrain)
                        {
                            subWorldBlockData[x, y, z].CurrentType = (byte)BlockTileType.GRASS;
                            if (y > highestPoint.y)
                            {
                                highestPoint = new CustomVector3(x, y, z);
                            }
                        }
                        else if (y >= surface + internalTerrain && subWorldBlockData[x, y - 1, z].CurrentType != (byte)BlockTileType.EMPTY)
                        {
                            TreeSpawnCandidates.Add(new CustomVector3(x, y, z));
                        }

                    }
                }
            }
            // caves
            GenerateSphereCaves(subWorldBlockData, subWorldSize);
            // various trees.
            int treeSpawnCount = Utilitys.RandomInteger(3, 7);
            for (int spawnCnt = 0; spawnCnt < treeSpawnCount; spawnCnt++)
            {
                TreeType randTreeType = (TreeType)Utilitys.RandomInteger(0, (int)TreeType.COUNT);
                switch (randTreeType)
                {
                    case TreeType.NORMAL:
                        EnviromentGenAlgorithms.GenerateDefaultTree(subWorldBlockData, TreeSpawnCandidates[Utilitys.RandomInteger(0, TreeSpawnCandidates.Count)], subWorldSize);
                        break;
                    case TreeType.SQAURE:
                        EnviromentGenAlgorithms.GenerateSqaureTree(subWorldBlockData, TreeSpawnCandidates[Utilitys.RandomInteger(0, TreeSpawnCandidates.Count)], subWorldSize);
                        break;
                }
            }
            EnviromentGenAlgorithms.MakeDefaultWaterArea(highestPoint, subWorldBlockData, subWorldSize);
        }

        /// <summary>
        /// flood fill 알고리즘을 이용한 Sphere 모양의 동굴 생성.
        /// </summary>
        /// <param name="subWorldBlockData"></param>
        private static void GenerateSphereCaves(Block[,,] subWorldBlockData, SubWorldSize subWorldSize)
        {
            int startX = Utilitys.RandomInteger(3, subWorldSize.SizeX - 16);
            int maxX = Utilitys.RandomInteger(startX, subWorldSize.SizeX);
            //
            int startZ = Utilitys.RandomInteger(3, subWorldSize.SizeZ - 16);
            int maxZ = Utilitys.RandomInteger(startX, subWorldSize.SizeZ);
            //
            int maxY = Utilitys.RandomInteger(10, subWorldSize.SizeY - 10);
            int startY = Utilitys.RandomInteger(5, maxY - 1);
            for (int x = startX; x < maxX; x++)
            {
                for (int z = startZ; z < maxZ; z++)
                {
                    for (int y = startY; y < maxY; y++)
                    {
                        int cave = WorldGenerateUtils.PerlinNoise(x, y * 3, z, 2, 18, 1);
                        if (cave > y)
                        {
                            subWorldBlockData[x, y, z].CurrentType = (byte)BlockTileType.EMPTY;
                            WorldGenerateUtils.FloodFillSubWorld(new FloodFill3DNode(x, y, z), BlockTileType.SAND, BlockTileType.EMPTY,
                                        subWorldBlockData, 4, subWorldSize);
                        }
                    }
                }
            }
        }
    }

}
