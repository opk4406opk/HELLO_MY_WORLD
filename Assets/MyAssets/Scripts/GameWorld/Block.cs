using UnityEngine;
using System;

[Serializable]
public struct Block {

    public byte type;
    public float centerX;
    public float centerY;
    public float centerZ;
    public bool isRendered;
    // 복사 생성자.
    public Block(Block b)
    {
        type = b.type;
        centerX = b.centerX;
        centerY = b.centerY;
        centerZ = b.centerZ;
        isRendered = b.isRendered;
    }
}
