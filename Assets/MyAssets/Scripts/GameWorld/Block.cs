using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Block
/// (1 x 1 x 1(unit))
/// </summary>
[Serializable]
public struct Block {

    public byte Type;
    public float CenterX;
    public float CenterY;
    public float CenterZ;
    public bool bRendered;
    public int WorldDataIndexX;
    public int WorldDataIndexY;
    public int WorldDataIndexZ;
    public int Durability;
    // 복사 생성자.
    public Block(Block b)
    {
        Type = b.Type;
        CenterX = b.CenterX;
        CenterY = b.CenterY;
        CenterZ = b.CenterZ;
        bRendered = b.bRendered;
        WorldDataIndexX = b.WorldDataIndexX;
        WorldDataIndexY = b.WorldDataIndexY;
        WorldDataIndexZ = b.WorldDataIndexZ;
        Durability = b.Durability;
    }

    public Vector3 GetCenterPosition()
    {
        return new Vector3(CenterX, CenterY, CenterZ);
    }
}

//public enum BlockFace
//{
//    TOP = 0x0001,
//    BOTTOM = 0x0010,
//    NORTH = 0x0100,
//    SOUTH = 0x1000,
//    WEST = 0x10000,
//    EAST = 0x100000
//}
//public struct BlockMakeInfo
//{
//    public float x, y, z;
//    public int blockIdxX, blockIdxY, blockIdxZ;
//    public TileType tileType;
//    public int exceptBlockFace;
//}
//public struct BakedBlockInfo
//{
//    public List<Vector3> vertices;
//    public List<int> triangles;
//    public List<Vector2> uv;
//    public int faceCount;
//}
//public class BlockMaker
//{
//    private static List<Vector3> blockVertices = new List<Vector3>();
//    private static List<int> blockTriangles = new List<int>();
//    private static List<Vector2> blockUV = new List<Vector2>();
//    private static int faceCount;

//    private static void Initialize()
//    {
//        blockVertices.Clear();
//        blockTriangles.Clear();
//        blockUV.Clear();
//        faceCount = 0;
//    }
//    public static BakedBlockInfo MakeBlock(BlockMakeInfo blockMakeInfo)
//    {
//        Initialize();
//        //
//        if ((blockMakeInfo.exceptBlockFace & (int)BlockFace.TOP) == 0x0000)
//        {
//            CreateFace(blockMakeInfo, BlockFace.TOP);
//        }
//        if ((blockMakeInfo.exceptBlockFace & (int)BlockFace.BOTTOM) == 0x0010)
//        {
//            CreateFace(blockMakeInfo, BlockFace.BOTTOM);
//        }
//        if ((blockMakeInfo.exceptBlockFace & (int)BlockFace.NORTH) == 0x0100)
//        {
//            CreateFace(blockMakeInfo, BlockFace.NORTH);
//        }
//        if ((blockMakeInfo.exceptBlockFace & (int)BlockFace.SOUTH) == 0x1000)
//        {
//            CreateFace(blockMakeInfo, BlockFace.SOUTH);
//        }
//        if ((blockMakeInfo.exceptBlockFace & (int)BlockFace.WEST) == 0x10000)
//        {
//            CreateFace(blockMakeInfo, BlockFace.WEST);
//        }
//        if ((blockMakeInfo.exceptBlockFace & (int)BlockFace.EAST) == 0x100000)
//        {
//            CreateFace(blockMakeInfo, BlockFace.EAST);
//        }

//        BakedBlockInfo result;
//        result.vertices = blockVertices;
//        result.triangles = blockTriangles;
//        result.uv = blockUV;
//        result.faceCount = faceCount;
//        return result;
//    }

//    private static void CreateFace(BlockMakeInfo blockMakeInfo, BlockFace blockFace)
//    {
//        var gameWorldConfig = WorldConfigFile.instance.GetConfig();
//        float tileUnit = gameWorldConfig.one_tile_unit;
//        switch (blockFace)
//        {
//            case BlockFace.TOP:
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y, blockMakeInfo.z));
//                break;
//            case BlockFace.BOTTOM:
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y - 1, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y - 1, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y - 1, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y - 1, blockMakeInfo.z + 1));
//                break;
//            case BlockFace.NORTH:
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y - 1, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y - 1, blockMakeInfo.z + 1));
//                break;
//            case BlockFace.SOUTH:
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y - 1, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y - 1, blockMakeInfo.z));
//                break;
//            case BlockFace.WEST:
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y - 1, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x, blockMakeInfo.y - 1, blockMakeInfo.z));
//                break;
//            case BlockFace.EAST:
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y - 1, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y, blockMakeInfo.z));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y, blockMakeInfo.z + 1));
//                blockVertices.Add(new Vector3(blockMakeInfo.x + 1, blockMakeInfo.y - 1, blockMakeInfo.z + 1));
//                break;
//        }
//        TileInfo tileInfo = TileDataFile.instance.GetTileInfo(blockMakeInfo.tileType);
//        Vector2 texturePos;
//        texturePos.x = tileInfo.posX;
//        texturePos.y = tileInfo.posY;

//        blockTriangles.Add(faceCount * 4); //1
//        blockTriangles.Add(faceCount * 4 + 1); //2
//        blockTriangles.Add(faceCount * 4 + 2); //3
//        blockTriangles.Add(faceCount * 4); //1
//        blockTriangles.Add(faceCount * 4 + 2); //3
//        blockTriangles.Add(faceCount * 4 + 3); //4

//        blockUV.Add(new Vector2(tileUnit * texturePos.x + tileUnit, tileUnit * texturePos.y));
//        blockUV.Add(new Vector2(tileUnit * texturePos.x + tileUnit, tileUnit * texturePos.y + tileUnit));
//        blockUV.Add(new Vector2(tileUnit * texturePos.x, tileUnit * texturePos.y + tileUnit));
//        blockUV.Add(new Vector2(tileUnit * texturePos.x, tileUnit * texturePos.y));

//        faceCount++; // Add this line
//    }
   
//}