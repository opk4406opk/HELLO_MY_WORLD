using UnityEngine;
using System;
using System.Runtime.Serialization;

[Serializable]
public struct Block {

    public byte type;
    /// <summary>
    /// 블록의 정중앙 포인트.
    /// </summary>
    public Vector3 center;

    public int blockDataPosX;
    public int blockDataPosY;
    public int blockDataPosZ;
    public bool isRendered;
    // 복사 생성자.
    public Block(Block b)
    {
        blockDataPosX = b.blockDataPosX;
        blockDataPosY = b.blockDataPosY;
        blockDataPosZ = b.blockDataPosZ;
        type = b.type;
        center = b.center;
        isRendered = b.isRendered;
    }
}
