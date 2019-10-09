using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
/// <summary>
/// Tile 데이터를 주고받기 위한 내부 통신용 구조체.
/// </summary>
public struct BlockTileInfo
{
    public string Name;
    public int PositionX;
    public int PositionY;
    public BlockTileType Type;
}

/// <summary>
/// 블록 수정 또는 삭제, 월드 생성시에 쓰이는 TileDataFile.
/// </summary>
public class BlockTileDataFile : BaseDataFile
{
    private Dictionary<BlockTileType, BlockTileInfo> TileDatas;

    public static BlockTileDataFile Instance = null;
    public override void Init ()
    {
        //
        Instance = this;
        //
        TileDatas = new Dictionary<BlockTileType, BlockTileInfo>();
        JsonFile = Resources.Load(ConstFilePath.TXT_BLOCK_TILE_DATAS) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);
    }

    public BlockTileInfo GetBlockTileInfo(BlockTileType type)
    {
        BlockTileInfo tileInfo;
        TileDatas.TryGetValue(type, out tileInfo);
        return tileInfo;
    }

    public int GetBlockTileInfoCount()
    {
        return TileDatas.Count;
    }

    protected override void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                var datas = jsonObj.list;
                for(int idx = 0; idx < datas.Count; idx++)
                {
                    BlockTileInfo tileInfo;
                    var data = datas[idx].ToDictionary();
                    string extractedInfo;
                    data.TryGetValue("type_name", out extractedInfo);
                    tileInfo.Name = extractedInfo;
                    tileInfo.Type = KojeomUtility.StringToEnum<BlockTileType>(extractedInfo);
                    data.TryGetValue("posX", out extractedInfo);
                    tileInfo.PositionX = int.Parse(extractedInfo);
                    data.TryGetValue("posY", out extractedInfo);
                    tileInfo.PositionY = int.Parse(extractedInfo);
                    //
                    TileDatas.Add(tileInfo.Type, tileInfo);
                }
                break;
            default:
                break;
        }
    }

}
