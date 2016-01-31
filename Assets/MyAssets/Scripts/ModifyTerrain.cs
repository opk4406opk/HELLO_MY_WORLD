using UnityEngine;
using System.Collections;

public class ModifyTerrain : MonoBehaviour
{
    [SerializeField]
    private World world;

    public void ReplaceBlockCursor(byte block)
    {
        //Replaces the block specified where the mouse cursor is pointing
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) ReplaceBlockAt(hit, block);
    }

    public void AddBlockCursor(byte block)
    {
        //Adds the block specified where the mouse cursor is pointing
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) AddBlockAt(hit, block);
    }

    public void ReplaceBlockAt(RaycastHit hit, byte block)
    {
        //removes a block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        Vector3 position = hit.point;
        position += (hit.normal * -0.5f);

        SetBlockAt(position, block);
    }

    public void AddBlockAt(RaycastHit hit, byte block)
    {
        //adds the specified block at these impact coordinates, you can raycast against the terrain and call this with the hit.point
        Vector3 position = hit.point;
        position += (hit.normal * 0.5f);

        SetBlockAt(position, block);

    }

    public void SetBlockAt(Vector3 position, byte block)
    {
        //sets the specified block at these coordinates

        int x = Mathf.RoundToInt(position.x);
        int y = Mathf.RoundToInt(position.y);
        int z = Mathf.RoundToInt(position.z);

        SetBlockAt(x, y, z, block);
    }

    public void SetBlockAt(int x, int y, int z, byte block)
    {
        //adds the specified block at these coordinates
        print("Adding: " + x + ", " + y + ", " + z);

        if (world.data[x + 1, y, z] == 254) world.data[x + 1, y, z] = 255;
        if (world.data[x - 1, y, z] == 254) world.data[x - 1, y, z] = 255;
        if (world.data[x, y, z + 1] == 254) world.data[x, y, z + 1] = 255;
        if (world.data[x, y, z - 1] == 254) world.data[x, y, z - 1] = 255;
        if (world.data[x, y + 1, z] == 254) world.data[x, y + 1, z] = 255;
        world.data[x, y, z] = block;

        UpdateChunkAt(x, y, z, block);
    }

    public void UpdateChunkAt(int x, int y, int z, byte block)
    {
        int updateX = Mathf.FloorToInt(x / world.chunkSize);
        int updateY = Mathf.FloorToInt(y / world.chunkSize);
        int updateZ = Mathf.FloorToInt(z / world.chunkSize);

        print("Updating: " + updateX + ", " + updateY + ", " + updateZ);

        world.chunks[updateX, updateY, updateZ].update = true;

        if (x - (world.chunkSize * updateX) == 0 && updateX != 0)
        {
            world.chunks[updateX - 1, updateY, updateZ].update = true;
        }

        if (x - (world.chunkSize * updateX) == 15 && updateX != world.chunks.GetLength(0) - 1)
        {
            world.chunks[updateX + 1, updateY, updateZ].update = true;
        }

        if (y - (world.chunkSize * updateY) == 0 && updateY != 0)
        {
            world.chunks[updateX, updateY - 1, updateZ].update = true;
        }

        if (y - (world.chunkSize * updateY) == 15 && updateY != world.chunks.GetLength(1) - 1)
        {
            world.chunks[updateX, updateY + 1, updateZ].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 0 && updateZ != 0)
        {
            world.chunks[updateX, updateY, updateZ - 1].update = true;
        }

        if (z - (world.chunkSize * updateZ) == 15 && updateZ != world.chunks.GetLength(2) - 1)
        {
            world.chunks[updateX, updateY, updateZ + 1].update = true;
        }

    }

}
