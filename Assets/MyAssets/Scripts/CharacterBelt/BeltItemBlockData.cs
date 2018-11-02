using UnityEngine;
using System.Collections;

/// <summary>
/// 게임내 사용자의 블록 belt에 등록되어있는 블록정보.
/// </summary>
public class BeltItemBlockData : ABeltItem
{
    private BlockTileType blockTileType;
    public override void Init(string spriteName)
    {
        uiButton = gameObject.GetComponent<UIButton>();
        uiSprite.spriteName = spriteName;
    }

    public void SetBlockTileType(BlockTileType blockTileType)
    {
        this.blockTileType = blockTileType;
    }

    public BlockTileType GetBlockTileType()
    {
        return blockTileType;
    }
}
