using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 블록 tile의 type 클래스.
/// </summary>
public enum BlockTileType
{
    EMPTY = 0,
    GRASS = 1,
    STONE_BIG = 2,
    STONE_SMALL = 3,
    SAND = 4,
    RED_STONE = 5,
    WOOD = 6,
    STONE_GOLD = 7,
    STONE_IRON = 8,
    STONE_SILVER = 9,
    NORMAL_TREE_LEAF = 10,
    SQAURE_TREE_LEAF = 11,
    WATER = 12,
    TILE_TOTAL_COUNT
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
public class BlockTileDataFile {

    private JSONObject tileDataJsonObj;
    private TextAsset jsonFile;
    private Dictionary<BlockTileType, BlockTileInfo> tileDatas;

    public static BlockTileDataFile instance = null;
    public void Init ()
    {
        //
        instance = this;
        //
        tileDatas = new Dictionary<BlockTileType, BlockTileInfo>();
        jsonFile = Resources.Load(ConstFilePath.TXT_BLOCK_TILE_DATAS) as TextAsset;
        tileDataJsonObj = new JSONObject(jsonFile.text);
        AccessData(tileDataJsonObj);
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

    private void AccessData(JSONObject jsonObj)
    {
        switch (jsonObj.type)
        {
            case JSONObject.Type.OBJECT:
                var datas = jsonObj.list;
                for(int idx = 0; idx < datas.Count; idx++)
                {
                    BlockTileInfo tileInfo;
                    tileInfo.type = (BlockTileType)idx;
                    var data = datas[idx].ToDictionary();
                    string extractedInfo;
                    data.TryGetValue("type_name", out extractedInfo);
                    tileInfo.name = extractedInfo;
                    data.TryGetValue("posX", out extractedInfo);
                    tileInfo.posX = int.Parse(extractedInfo);
                    data.TryGetValue("posY", out extractedInfo);
                    tileInfo.posY = int.Parse(extractedInfo);
                    //
                    tileDatas.Add((BlockTileType)idx, tileInfo);
                }
                break;
            default:
                break;
        }
    }

}
