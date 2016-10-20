using UnityEngine;
using System.Collections;

public struct Block {

    private byte _type;
    public byte type
    {
        set { _type = value; }
        get { return _type; }
    }

    public CustomAABB aabb;
    
}
