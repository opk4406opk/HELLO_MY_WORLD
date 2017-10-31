using UnityEngine;
using System.Collections;

/// <summary>
/// 게임내에서 사용자가 사용하고 싶은 블록을 선택하는 벨트를 관리하는 클래스.
/// </summary>
public class BlockSelector : MonoBehaviour {

    [SerializeField]
    private GameObject blockPrefab;
    [SerializeField]
    private GameObject uiGridObj;

    private int maxSelectBlocks = 0;
    private string curSelectBlockName;
    private byte _curSelectBlockType;
    public byte curSelectBlockType
    {
        get { return _curSelectBlockType; }
    }

	public void Init()
    {
        maxSelectBlocks = TileDataFile.instance.tileNameList.Count;
        //default : grass block;
        curSelectBlockName = TileType.TILE_TYPE_GRASS;
        _curSelectBlockType = 1;

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

            Ed_OnSelectBlock = new EventDelegate(this, "OnSelectBlock");
            Ed_OnSelectBlock.parameters[0].value = TileDataFile.instance.tileNameList[idx];
            newBlock.GetComponent<UIButton>().onClick.Add(Ed_OnSelectBlock);

            BlockData block = newBlock.GetComponent<BlockData>();
            block.Init(TileDataFile.instance.tileNameList[idx]);
        }
        uiGridObj.GetComponent<UIGrid>().Reposition();
    }

    private EventDelegate Ed_OnSelectBlock;
    private void OnSelectBlock(string name)
    {
        curSelectBlockName = name;
        CalcBlockType();
    }

    private void CalcBlockType()
    {
       TileInfo tileData = TileDataFile.instance.GetTileData(curSelectBlockName);
        _curSelectBlockType = (byte)tileData.type;    
    }
}
