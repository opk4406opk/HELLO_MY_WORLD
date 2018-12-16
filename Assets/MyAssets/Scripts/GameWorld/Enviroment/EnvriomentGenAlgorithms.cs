using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TreeType
{
    NORMAL = 0,
    SQAURE = 1,
    COUNT
}
public class EnvriomentGenAlgorithms : MonoBehaviour {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="worldBlockData"></param>
    public static void GenerateDefaultTree(Block[,,] worldBlockData, Vector3 rootPosition)
    {
        int branchDepth = KojeomUtility.RandomInteger(3, 7);
        int treeBodyLength = KojeomUtility.RandomInteger(4, 6);
        for (int idx = 0; idx < treeBodyLength; idx++)
        {
            if (WorldGenAlgorithms.CheckBoundary((int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z) == true)
            {
                worldBlockData[(int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z].type = (byte)BlockTileType.WOOD;
            }
        }
        MakeDefaultBranch(worldBlockData,
            new Vector3(rootPosition.x, rootPosition.y + treeBodyLength, rootPosition.z),
            branchDepth, BlockTileType.NORMAL_TREE_LEAF);
    }

    private static void MakeDefaultBranch(Block[,,] worldBlockData, Vector3 branchPos, int depth, BlockTileType leafType)
    {
        if (depth == 0) return;

        // make leafs.
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                for (int z = -1; z < 2; z++)
                {
                    if (WorldGenAlgorithms.CheckBoundary((int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z) == true)
                    {
                        worldBlockData[(int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z].type = (byte)leafType;
                    }
                }
            }
        }

        // make more branches.
        MakeDefaultBranch(worldBlockData, branchPos, depth - 1, leafType);
        if (WorldGenAlgorithms.CheckBoundary((int)branchPos.x, (int)branchPos.y + 1, (int)branchPos.z) == true) MakeDefaultBranch(worldBlockData, new Vector3(branchPos.x, branchPos.y + 1, branchPos.z), depth - 1, leafType);
        if (WorldGenAlgorithms.CheckBoundary((int)branchPos.x + 1, (int)branchPos.y + 1, (int)branchPos.z) == true) MakeDefaultBranch(worldBlockData, new Vector3(branchPos.x + 1, branchPos.y + 1, branchPos.z), depth - 1, leafType);
        if (WorldGenAlgorithms.CheckBoundary((int)branchPos.x - 1, (int)branchPos.y + 1, (int)branchPos.z) == true) MakeDefaultBranch(worldBlockData, new Vector3(branchPos.x - 1, branchPos.y + 1, branchPos.z), depth - 1, leafType);
        if (WorldGenAlgorithms.CheckBoundary((int)branchPos.x, (int)branchPos.y + 1, (int)branchPos.z + 1) == true) MakeDefaultBranch(worldBlockData, new Vector3(branchPos.x, branchPos.y + 1, branchPos.z + 1), depth - 1, leafType);
        if (WorldGenAlgorithms.CheckBoundary((int)branchPos.x, (int)branchPos.y + 1, (int)branchPos.z - 1) == true) MakeDefaultBranch(worldBlockData, new Vector3(branchPos.x, branchPos.y + 1, branchPos.z - 1), depth - 1, leafType);
    }

    public static void GenerateSqaureTree(Block[,,] worldBlockData, Vector3 rootPosition)
    {
        int treeBodyLength = KojeomUtility.RandomInteger(4, 6);
        for (int idx = 0; idx < treeBodyLength; idx++)
        {
            if (WorldGenAlgorithms.CheckBoundary((int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z) == true)
            {
                worldBlockData[(int)rootPosition.x, (int)rootPosition.y + idx, (int)rootPosition.z].type = (byte)BlockTileType.WOOD;
            }
        }
        MakeSqaureBranch(worldBlockData, new Vector3(rootPosition.x, rootPosition.y + treeBodyLength, rootPosition.z), 2, BlockTileType.SQAURE_TREE_LEAF);
    }

    private static void MakeSqaureBranch(Block[,,] worldBlockData, Vector3 branchPos, int sqaureFactor, BlockTileType leafType)
    {
        // make leafs.
        for (int x = -1; x < sqaureFactor; x++)
        {
            for (int y = -1; y < sqaureFactor; y++)
            {
                for (int z = -1; z < sqaureFactor; z++)
                {
                    if (WorldGenAlgorithms.CheckBoundary((int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z) == true)
                    {
                        worldBlockData[(int)branchPos.x + x, (int)branchPos.y + y, (int)branchPos.z + z].type = (byte)leafType;
                    }
                }
            }
        }
    }

    public static void MakeWaterArea(int seaLevel, Block[,,] worldBlockData)
    {
        for(int x = 0; x < worldBlockData.GetLength(0); x++)
        {
            for(int z = 0; z < worldBlockData.GetLength(2); z++)
            {
                for(int y = seaLevel - KojeomUtility.RandomInteger(1, 3); y >= 0; y--)
                {
                    if(worldBlockData[x, y, z].type == (byte)BlockTileType.EMPTY)
                    {
                        worldBlockData[x, y, z].type = (byte)BlockTileType.WATER;
                    }
                }
            }
        }
    }
}

