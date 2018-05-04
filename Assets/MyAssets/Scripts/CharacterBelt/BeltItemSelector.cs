using UnityEngine;
using System.Collections;

public abstract class ABeltItem : MonoBehaviour
{
    protected UIButton uiButton;
    [SerializeField]
    protected UISprite uiSprite;
    public abstract void Init(string spriteName);
}

/// <summary>
/// 게임내에서 사용자가 사용하고 싶은 아이템을 선택하는 벨트를 관리하는 클래스.
/// </summary>
public class BeltItemSelector : MonoBehaviour {

    private static BeltItemSelector _singleton = null;
    public static BeltItemSelector singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("BeltItemSelector 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }
    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private GameObject uiGridObj;

    private int maxSelectBlocks = 0;
    private byte _curSelectBlockType;
    public byte curSelectBlockType
    {
        get { return _curSelectBlockType; }
    }

	public void Init()
    {
        _singleton = this;
        maxSelectBlocks = TileDataFile.instance.GetTileInfoCount();
        //default : grass block;
        _curSelectBlockType = (byte)TileType.GRASS;
        //
        CreateSelectBlock();
    }
	
    private void CreateSelectBlock()
    {
        for (int idx = 0; idx < maxSelectBlocks; ++idx)
        {
            GameObject newBlock = Instantiate(blockPrefab,
               new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0)) as GameObject;

            newBlock.SetActive(true);
            //item parenting
            newBlock.transform.parent = uiGridObj.transform;
            newBlock.transform.localScale = new Vector3(1, 1, 1);
            newBlock.transform.localPosition = new Vector3(0, 0, 0);

            var tileInfo = TileDataFile.instance.GetTileInfo((TileType)idx);
            BeltItemBlockData block = newBlock.GetComponent<BeltItemBlockData>();
            block.Init(tileInfo.name);
        }
        uiGridObj.GetComponent<UIGrid>().Reposition();
    }

    public void OnSelectBlock(GameObject selectedBlock)
    {
    }
}
