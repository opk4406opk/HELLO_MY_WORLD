using UnityEngine;
using System.Collections;

/// <summary>
/// 게임내 사용자의 블록 belt에 등록되어있는 블록정보.
/// </summary>
public class BlockData : MonoBehaviour
{

    [SerializeField]
    private UISprite spr_block;

	public void Init(string _sprName)
    {
        spr_block.spriteName = _sprName;
    }
    
}
