using UnityEngine;
using System.Collections;

public struct Block {

    private byte _type;
    public byte type
    {
        set { _type = value; }
        get { return _type; }
    }

    private Vector3 _center;
    /// <summary>
    /// 블록의 정중앙 포인트.
    /// </summary>
    public Vector3 center
    {
        set { _center = value; }
        get { return _center; }
    }

    private int _blockDataPosX;
    public int blockDataPosX
    {
        set { _blockDataPosX = value; }
        get { return _blockDataPosX; }
    }
    private int _blockDataPosY;
    public int blockDataPosY
    {
        set { _blockDataPosY = value; }
        get { return _blockDataPosY; }
    }
    private int _blockDataPosZ;
    public int blockDataPosZ
    {
        set { _blockDataPosZ = value; }
        get { return _blockDataPosZ; }
    }

    private bool _isRendered;
    public bool isRendered
    {
        set { _isRendered = value; }
        get { return _isRendered; }
    }
    
    // 복사 생성자.
    public Block(Block b)
    {
        _blockDataPosX = b.blockDataPosX;
        _blockDataPosY = b.blockDataPosY;
        _blockDataPosZ = b.blockDataPosZ;
        _type = b.type;
        _center = b.center;
        _isRendered = b.isRendered;
    }
}
