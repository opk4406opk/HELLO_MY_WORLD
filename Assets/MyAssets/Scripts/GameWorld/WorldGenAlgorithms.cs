using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WorldGenAlgorithms
{
    private static List<Vector3> TreeSpawnCandidates = new List<Vector3>();

    public struct TerrainValue
    {
        public int ScalaValue;
        public List<int> Layers;
    }

    public static BlockTileType CalcTerrainValueToBlockType(int terrainScalaValue)
    {
        BlockTileType blockType = BlockTileType.EMPTY;
        int subWorldLayerNum = WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldLayer;
        int subWorldSizeY = WorldConfigFile.Instance.GetConfig().SubWorldSizeY;
        int range = subWorldLayerNum * subWorldSizeY;
        int min = -1 * range;
        int max = range;

        if(terrainScalaValue <= min + 10 && terrainScalaValue >= min)
        {
            blockType = BlockTileType.STONE_SILVER;
        }
        else if(terrainScalaValue > min + 10 && terrainScalaValue <= 0)
        {
            blockType = BlockTileType.WATER;
        }
        else if(terrainScalaValue > 0 && terrainScalaValue <= max / 2)
        {
            blockType = BlockTileType.STONE_BIG;
        }
        else if(terrainScalaValue > max / 2 && terrainScalaValue <= max)
        {
            blockType = BlockTileType.RED_STONE;
        }
        
        return blockType;
    }

    public static TerrainValue[,] GenerateNormalTerrain(int areaSizeX, int areaSizeZ, int generateNumber = 800)
    {
        
        int[,] xzPlane = new int[areaSizeX, areaSizeZ];
        //
        int subWorldLayerNum = WorldMapDataFile.Instance.WorldMapDataInstance.SubWorldLayer;
        int subWorldSizeY = WorldConfigFile.Instance.GetConfig().SubWorldSizeY;
        int rangeValue = subWorldLayerNum * subWorldSizeY;
        for (int loop = 0; loop < generateNumber; loop++)
        {
            Vector2 firstPoint = new Vector2(KojeomUtility.RandomInteger(0, areaSizeX), KojeomUtility.RandomInteger(0, areaSizeZ));
            Vector2 secondPoint = new Vector2(KojeomUtility.RandomInteger(0, areaSizeX), KojeomUtility.RandomInteger(0, areaSizeZ));
            Vector2 dirVec = secondPoint - firstPoint;
            for (int x = 0; x < areaSizeX; x++)
            {
                for (int z = 0; z < areaSizeZ; z++)
                {
                    Vector2 point = new Vector2(x, z);
                    Vector3 crossVec = Vector3.Cross(dirVec, point);
                    if (crossVec.z > 0)
                    {
                        xzPlane[x, z]++;

                    }
                    else if(crossVec.z < 0)
                    {
                        xzPlane[x, z]--;
                    }
                    else
                    {
                        if (KojeomUtility.RandomBool() == true) xzPlane[x, z]--;
                        else xzPlane[x, z]++;
                    }
                    //
                    int min = -1 * rangeValue;
                    int max = rangeValue;
                    xzPlane[x, z] = Mathf.Clamp(xzPlane[x, z], min, max);
                }
            }
        }

        // Normalize Terrain.
        for(int x = 0; x < areaSizeX; x++)
        {
            for(int z = 0; z < areaSizeZ; z++)
            {
                // Water 지형이라면, 평준화 시킨다.
                if (xzPlane[x, z] < 0)
                {
                    WorldGenerateUtils.NormalizeCrossDirection(x, z, xzPlane, -1);
                }
            }
        }

        // Calc Range per Chunk size.
        TerrainValue[,] terrainValues = new TerrainValue[areaSizeX, areaSizeZ];
        for(int x = 0; x < areaSizeX; x++)
        {
            for(int z = 0; z < areaSizeZ; z++)
            {
                terrainValues[x, z].ScalaValue = xzPlane[x, z];
                terrainValues[x, z].Layers = new List<int>();
                int absTerrainScalaValue = Mathf.Abs(xzPlane[x, z]);
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
                    else if(absTerrainScalaValue >= subWorldSizeY)
                    {
                        rangeY = subWorldSizeY;
                        absTerrainScalaValue -= subWorldSizeY;
                    }
                    //
                    terrainValues[x, z].Layers.Add(Mathf.Abs(rangeY));
                }
            }
        }
        return terrainValues;
    }

    public static void GenerateSubWorldWithPerlinNoise(Block[,,] subWorldBlockData, MakeWorldParam param)
    {
        Vector3 highestPoint = Vector3.zero;
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        // perlin 알고리즘을 이용해 지형을 생성한다.
        for (int x = 0; x < gameWorldConfig.SubWorldSizeX; x++)
        {
            for (int z = 0; z < gameWorldConfig.SubWorldSizeZ; z++)
            {
                int internalTerrain = WorldGenerateUtils.PerlinNoise(x, 20, z, 3, KojeomUtility.RandomInteger(1, 3), 2);
                internalTerrain += param.BaseOffset;
                int surface = WorldGenerateUtils.PerlinNoise(x, 21, z, 1, KojeomUtility.RandomInteger(1, 2), 1) + 1;

                for (int y = 0; y < gameWorldConfig.SubWorldSizeY; y++)
                {
                    if (y <= internalTerrain)
                    {
                        subWorldBlockData[x, y, z].Type = (byte)BlockTileType.STONE_BIG;
                    }
                    else if (y <= surface + internalTerrain)
                    {
                        subWorldBlockData[x, y, z].Type = (byte)BlockTileType.GRASS;
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

    /// <summary>
    /// flood fill 알고리즘을 이용한 Sphere 모양의 동굴 생성.
    /// </summary>
    /// <param name="subWorldBlockData"></param>
    private static void GenerateSphereCaves(Block[,,] subWorldBlockData)
    {
        var gameWorldConfig = WorldConfigFile.Instance.GetConfig();
        int startX = KojeomUtility.RandomInteger(3, gameWorldConfig.SubWorldSizeX - 16);
        int maxX = KojeomUtility.RandomInteger(startX, gameWorldConfig.SubWorldSizeX);
        //
        int startZ = KojeomUtility.RandomInteger(3, gameWorldConfig.SubWorldSizeZ - 16);
        int maxZ = KojeomUtility.RandomInteger(startX, gameWorldConfig.SubWorldSizeZ);
        //
        int maxY = KojeomUtility.RandomInteger(10, gameWorldConfig.SubWorldSizeY - 10);
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
                        subWorldBlockData[x, y, z].Type = (byte)BlockTileType.EMPTY;
                        WorldGenerateUtils.FloodFillSubWorld(new FloodFill3DNode(x, y, z), BlockTileType.SAND, BlockTileType.EMPTY,
                                    subWorldBlockData, 4);
                    }
                }
            }
        }
    }
    
}
