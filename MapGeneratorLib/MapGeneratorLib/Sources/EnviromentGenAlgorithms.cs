using System.Collections;
using System.Collections.Generic;
using MapGenLib;
namespace MapGenLib
{
    public enum TreeType
    {
        NORMAL = 0,
        SQAURE = 1,
        COUNT
    }
    public class EnviromentGenAlgorithms
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="worldBlockData"></param>
        public static void GenerateDefaultTree(Block[,,] worldBlockData, CustomVector3 rootPosition, SubWorldSize subWorldSize)
        {
            int branchDepth = Utilitys.RandomInteger(3, 7);
            int treeBodyLength = Utilitys.RandomInteger(4, 6);
            for (int idx = 0; idx < treeBodyLength; idx++)
            {
                if (WorldGenerateUtils.CheckSubWorldBoundary((int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z, subWorldSize) == true)
                {
                    worldBlockData[(int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z].Type = (byte)BlockTileType.WOOD;
                }
            }
            MakeDefaultBranch(worldBlockData,
                new CustomVector3(rootPosition.x, rootPosition.y + treeBodyLength, rootPosition.z),
                branchDepth, subWorldSize, BlockTileType.NORMAL_TREE_LEAF);
        }

        private static void MakeDefaultBranch(Block[,,] worldBlockData, CustomVector3 branchPos, int depth, SubWorldSize subWorldSize, BlockTileType leafType)
        {
            if (depth == 0) return;

            // make leafs.
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    for (int z = -1; z < 2; z++)
                    {
                        if (WorldGenerateUtils.CheckSubWorldBoundary((int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z, subWorldSize) == true)
                        {
                            worldBlockData[(int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z].Type = (byte)leafType;
                        }
                    }
                }
            }

            // make more branches.
            MakeDefaultBranch(worldBlockData, branchPos, depth - 1, subWorldSize, leafType);
            if (WorldGenerateUtils.CheckSubWorldBoundary((int)branchPos.x, (int)branchPos.y + 1, (int)branchPos.z, subWorldSize) == true) MakeDefaultBranch(worldBlockData, new CustomVector3(branchPos.x, branchPos.y + 1, branchPos.z), depth - 1, subWorldSize, leafType);
            if (WorldGenerateUtils.CheckSubWorldBoundary((int)branchPos.x + 1, (int)branchPos.y + 1, (int)branchPos.z, subWorldSize) == true) MakeDefaultBranch(worldBlockData, new CustomVector3(branchPos.x + 1, branchPos.y + 1, branchPos.z), depth - 1, subWorldSize, leafType);
            if (WorldGenerateUtils.CheckSubWorldBoundary((int)branchPos.x - 1, (int)branchPos.y + 1, (int)branchPos.z, subWorldSize) == true) MakeDefaultBranch(worldBlockData, new CustomVector3(branchPos.x - 1, branchPos.y + 1, branchPos.z), depth - 1, subWorldSize, leafType);
            if (WorldGenerateUtils.CheckSubWorldBoundary((int)branchPos.x, (int)branchPos.y + 1, (int)branchPos.z + 1, subWorldSize) == true) MakeDefaultBranch(worldBlockData, new CustomVector3(branchPos.x, branchPos.y + 1, branchPos.z + 1), depth - 1, subWorldSize, leafType);
            if (WorldGenerateUtils.CheckSubWorldBoundary((int)branchPos.x, (int)branchPos.y + 1, (int)branchPos.z - 1, subWorldSize) == true) MakeDefaultBranch(worldBlockData, new CustomVector3(branchPos.x, branchPos.y + 1, branchPos.z - 1), depth - 1, subWorldSize, leafType);
        }

        public static void GenerateSqaureTree(Block[,,] worldBlockData, CustomVector3 rootPosition, SubWorldSize subWorldSize)
        {
            int treeBodyLength = Utilitys.RandomInteger(4, 6);
            for (int idx = 0; idx < treeBodyLength; idx++)
            {
                if (WorldGenerateUtils.CheckSubWorldBoundary((int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z, subWorldSize) == true)
                {
                    worldBlockData[(int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z].Type = (byte)BlockTileType.WOOD;
                }
            }
            MakeSqaureBranch(worldBlockData, new CustomVector3(rootPosition.x, rootPosition.y + treeBodyLength, rootPosition.z), 2, BlockTileType.SQAURE_TREE_LEAF, subWorldSize);
        }

        private static void MakeSqaureBranch(Block[,,] worldBlockData, CustomVector3 branchPos, int sqaureFactor, BlockTileType leafType, SubWorldSize subWorldSize)
        {
            // make leafs.
            for (int x = -1; x < sqaureFactor; x++)
            {
                for (int y = -1; y < sqaureFactor; y++)
                {
                    for (int z = -1; z < sqaureFactor; z++)
                    {
                        if (WorldGenerateUtils.CheckSubWorldBoundary((int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z, subWorldSize) == true)
                        {
                            worldBlockData[(int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z].Type = (byte)leafType;
                        }
                    }
                }
            }
        }

        public static void MakeDefaultWaterArea(CustomVector3 centerPoint, Block[,,] worldBlockData, SubWorldSize subWorldSize)
        {
            WorldGenerateUtils.FloodFillSubWorld(new FloodFill3DNode(centerPoint),
                BlockTileType.NONE,
                BlockTileType.WATER,
                worldBlockData, 10, subWorldSize, FloodFillDirection.TOP);
        }
    }


}
