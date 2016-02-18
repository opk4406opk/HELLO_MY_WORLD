using UnityEngine;
using System.Collections;

public class ModifyTerrain : MonoBehaviour
{
    private World _world;
    public World world
    {
        set { _world = value; }
    }
    
    private int chunkSize = 0;

    void Start()
    {
        chunkSize = GameWorldConfig.chunkSize;
    }

    public void ReplaceBlockCursor(RaycastHit hit, byte block)
    {
        ReplaceBlockAt(hit, block);
    }

    public void AddBlockCursor(RaycastHit hit, byte block)
    {
        AddBlockAt(hit, block);
    }

    private void ReplaceBlockAt(RaycastHit hit, byte block)
    {
        //removes a block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        Vector3 position = hit.point;
        position += (hit.normal * -0.5f);

        SetBlockAt(position, block);
    }

    private void AddBlockAt(RaycastHit hit, byte block)
    {
        //adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        Vector3 position = hit.point;
        position += (hit.normal * 0.5f);

        SetBlockAt(position, block);
    }

    private void SetBlockAt(Vector3 position, byte block)
    {
        // 기본 청크 위치 : x' = x * chunkSize;
        // 오프셋 적용한 청크 위치 : x'' = (x + offset) * chunkSize;
        // 따라서, 기본 Chunk 위치와 오프셋이 적용된 Chunk의 차이는
        //  x'' = x' - (offset * chunkSize)
        int x, y, z;
        x = Mathf.RoundToInt(position.x - (_world.chunkOffsetX * chunkSize));
        y = Mathf.RoundToInt(position.y);
        z = Mathf.RoundToInt(position.z - (_world.chunkOffsetZ * chunkSize));

        SetBlockAt(x, y, z, block);
    }

    private void SetBlockAt(int x, int y, int z, byte block)
    {
        //adds the specified block at these coordinates
        print("Adding: " + x + ", " + y + ", " + z);
        
        _world.worldBlockData[x, y, z] = block;

        UpdateChunkAt(x, y, z, block);
    }

    private void UpdateChunkAt(int x, int y, int z, byte block)
    {
        // world data 인덱스를 chunkGroup 인덱스로 변환한다. 
        int updateX, updateY, updateZ;
        updateX = Mathf.FloorToInt(x / chunkSize);
        updateY = Mathf.FloorToInt(y / chunkSize);
        updateZ = Mathf.FloorToInt(z / chunkSize);
       
        print("Updating: " + updateX + ", " + updateY + ", " + updateZ);

        _world.chunkGroup[updateX, updateY, updateZ].update = true;

        if (x - (chunkSize * updateX) == 0 && updateX != 0)
        {
            _world.chunkGroup[updateX - 1, updateY, updateZ].update = true;
        }

        if (x - (chunkSize * updateX) == GameWorldConfig.chunkSize && updateX != _world.chunkGroup.GetLength(0) - 1)
        {
            _world.chunkGroup[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == 0 && updateY != 0)
        {
            _world.chunkGroup[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (chunkSize * updateY) == GameWorldConfig.chunkSize && updateY != _world.chunkGroup.GetLength(1) - 1)
        {
            _world.chunkGroup[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (chunkSize * updateZ) == 0 && updateZ != 0)
        {
            _world.chunkGroup[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (chunkSize * updateZ) == GameWorldConfig.chunkSize && updateZ != _world.chunkGroup.GetLength(2) - 1)
        {
            _world.chunkGroup[updateX, updateY, updateZ + 1].update = true;
        }

    }

}
