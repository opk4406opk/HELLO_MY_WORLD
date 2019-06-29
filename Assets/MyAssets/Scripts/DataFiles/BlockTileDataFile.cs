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
    public string name;
    public int posX;
    public int posY;
    public BlockTileType type;
}

/// <summary>
/// 블록 수정 또는 삭제, 월드 생성시에 쓰이는 TileDataFile.
/// </summary>
public class BlockTileDataFile : BaseDataFile
{
    private Dictionary<BlockTileType, BlockTileInfo> tileDatas;

    public static BlockTileDataFile instance = null;
    public override void Init ()
    {
        //
        instance = this;
        //
        tileDatas = new Dictionary<BlockTileType, BlockTileInfo>();
        JsonFile = Resources.Load(ConstFilePath.TXT_BLOCK_TILE_DATAS) as TextAsset;
        JsonObject = new JSONObject(JsonFile.text);
        AccessData(JsonObject);
    }

    public BlockTileInfo GetBlockTileInfo(BlockTileType type)
    {
        BlockTileInfo tileInfo;
        tileDatas.TryGetValue(type, out tileInfo);
        return tileInfo;
    }

    public int GetBlockTileInfoCount()
    {
        return tileDatas.Count;
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
                    tileInfo.name = extractedInfo;
                    tileInfo.type = KojeomUtility.StringToEnum<BlockTileType>(extractedInfo);
                    data.TryGetValue("posX", out extractedInfo);
                    tileInfo.posX = int.Parse(extractedInfo);
                    data.TryGetValue("posY", out extractedInfo);
                    tileInfo.posY = int.Parse(extractedInfo);
                    //
                    tileDatas.Add(tileInfo.type, tileInfo);
                }
                break;
            default:
                break;
        }
    }

}
