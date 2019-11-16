using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUtils : MonoBehaviour
{
    public static bool GetRandomWorldPositionFromActorPos(out Vector3 outResult, ActorController actorController)
    {
        List<Vector3> direcitons = new List<Vector3>();
        direcitons.Add(new Vector3(0, 0, 1));
        direcitons.Add(new Vector3(0, 0, -1));
        direcitons.Add(new Vector3(1, 0, 0));
        direcitons.Add(new Vector3(-1, 0, 0));
        direcitons.Add(new Vector3(1, 0, 1));
        direcitons.Add(new Vector3(1, 0, -1));
        direcitons.Add(new Vector3(-1, 0, 1));
        direcitons.Add(new Vector3(-1, 0, -1));

        Block[,,] blockData = actorController.GetContainedWorldBlockData();
        Vector3 blockCoordinate = WorldAreaManager.GetRealCoordToWorldCoord(actorController.GetActorTransform().position);

        List<Vector3> candidates = new List<Vector3>();
        foreach (Vector3 dir in direcitons)
        {
            Vector3 offsetIdx = blockCoordinate += dir;
            int blockX = (int)offsetIdx.x;
            int blockY = (int)offsetIdx.y;
            int blockZ = (int)offsetIdx.z;

            bool bInBoundX = 0 <= blockX && blockX < WorldConfigFile.Instance.GetConfig().SubWorldSizeX;
            bool bInBoundY = 0 <= blockY && blockY < WorldConfigFile.Instance.GetConfig().SubWorldSizeY;
            bool bInBoundZ = 0 <= blockZ && blockZ < WorldConfigFile.Instance.GetConfig().SubWorldSizeZ;
            if (bInBoundX == true && bInBoundY == true && bInBoundZ == true)
            {
                Block block = blockData[blockX, blockY, blockZ];
                if(block.Type != (byte)BlockTileType.EMPTY)
                {
                    // 중앙 좌표이므로 Y 값은 0.5 오프셋을 더함.
                    Vector3 position = new Vector3(block.CenterX, block.CenterY + 0.5f, block.CenterZ);
                    candidates.Add(position);
                }
            }
        }
        if(candidates.Count > 0)
        {
            outResult = candidates[KojeomUtility.RandomInteger(0, candidates.Count)];
            return true;
        }
        outResult = Vector3.zero;
        return false;
    }
}
