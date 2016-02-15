using UnityEngine;
using System.Collections;

public class BlockData : MonoBehaviour {

    [SerializeField]
    private UISprite spr_block;

	public void Init(string _sprName)
    {
        spr_block.spriteName = _sprName;
    }
    
}
