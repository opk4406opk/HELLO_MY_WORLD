using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MapGenLib;
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
        Vector3 blockCoordinate = WorldAreaManager.GetRealCoordToWorldDataCoord(actorController.GetActorTransform().position);

        List<Vector3> candidates = new List<Vector3>();
        foreach (Vector3 dir in direcitons)
        {
            Vector3 offsetIdx = blockCoordinate += dir;
            int blockX = (int)offsetIdx.x;
            int blockY = (int)offsetIdx.y;
            int blockZ = (int)offsetIdx.z;

            WorldConfig config = WorldConfigFile.Instance.GetConfig();
            bool bInBoundX = 0 <= blockX && blockX < config.SubWorldSizeX;
            bool bInBoundY = 0 <= blockY && blockY < config.SubWorldSizeY;
            bool bInBoundZ = 0 <= blockZ && blockZ < config.SubWorldSizeZ;
            if (bInBoundX == true && bInBoundY == true && bInBoundZ == true)
            {
                bool bEmptyUpBlock = true;
                int upStairY = (blockY + 1);
                bool bInBoundUpY = upStairY < config.SubWorldSizeY;
                if(bInBoundUpY == true)
                {
                    Block upBlock = blockData[blockX, upStairY, blockZ];
                    if(upBlock.Type != (byte)BlockTileType.EMPTY) bEmptyUpBlock = false;
                }
                else
                {
                    // 영역을 벗어나는 지점.
                    bEmptyUpBlock = false;
                }
                Block block = blockData[blockX, blockY, blockZ];
                bool bNotEmptyToBlock = block.Type != (byte)BlockTileType.EMPTY;
                // 목표 블록이 Empty가 아니면서 바로 위에 있는 블록이 Empty 라면 갈 수 있음.
                if (bNotEmptyToBlock == true && bEmptyUpBlock == true)
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
