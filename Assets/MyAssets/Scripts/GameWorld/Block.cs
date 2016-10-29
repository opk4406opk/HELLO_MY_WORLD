using UnityEngine;
using System.Collections;

public class Block {

    private byte _type;
    public byte type
    {
        set { _type = value; }
        get { return _type; }
    }

    public CustomAABB aabb;

    private string _belongWorld;
    public string belongWorld
    {
        set { _belongWorld = value; }
        get { return _belongWorld; }
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

    // 디폴트.
    public Block()
    {
        // to do.
    }
    // 복사 생성자.
    public Block(Block b)
    {
        this.aabb = b.aabb;
        this.belongWorld = b.belongWorld;
        this.blockDataPosX = b.blockDataPosX;
        this.blockDataPosY = b.blockDataPosY;
        this.blockDataPosZ = b.blockDataPosZ;
        this.type = b.type;
        this.center = b.center;
    }
}
