using UnityEngine;
using System.Collections;

/// <summary>
/// 게임내 사용자의 블록 belt에 등록되어있는 블록정보.
/// </summary>
public class BeltItemBlockData : ABeltItem
{
    private TileType blockTileType;
    public override void Init(string spriteName)
    {
        uiButton = gameObject.GetComponent<UIButton>();
        uiSprite.spriteName = spriteName;
    }

    public void SetBlockTileType(TileType blockTileType)
    {
        this.blockTileType = blockTileType;
    }

    public TileType GetBlockTileType()
    {
        return blockTileType;
    }
}
