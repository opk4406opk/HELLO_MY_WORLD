using UnityEngine;
using System;

[Serializable]
public struct Block {

    public byte type;
    /// <summary>
    /// 블록의 정중앙 포인트.
    /// </summary>
    public Vector3 center;
    public bool isRendered;
    // 복사 생성자.
    public Block(Block b)
    {
        type = b.type;
        center = b.center;
        isRendered = b.isRendered;
    }
}
