using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPC_Controller : MonoBehaviour
{
    private CustomAABB aabb;
    [SerializeField]
    private Transform objectMinExtent;
    [SerializeField]
    private Transform objectMaxExtent;

    private Block[,,] worldBlockData;
    private CustomAstar pathFinder;
    private Stack<PathNode> pathTrace = new Stack<PathNode>();
    private IEnumerator pMoveProcess;
    public void Init(Block[,,] _worldBlockData)
    {
        aabb.isEnable = true;
        worldBlockData = _worldBlockData;
        pathFinder = new CustomAstar(worldBlockData, transform);
       
        StartCoroutine(ReMakeAABBProcess());
        StartCoroutine(SimpleGravityForce());
        //StartCoroutine(MoveProcess());
    }

    public void ActivePathFindNPC(int goalX, int goalZ)
    {
        pathFinder.SetGoalPathNode(goalX, goalZ);
        if(pMoveProcess != null) StopCoroutine(pMoveProcess);
        pMoveProcess = MoveProcess();
        StartCoroutine(pMoveProcess);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(aabb.minExtent, aabb.maxExtent);
    }

    // NPC를 주변으로 일정한 공간과의 충돌을 감지한다.
    private bool IsCollisionArounds()
    {
        int worldX = Mathf.RoundToInt(transform.position.x);
        int worldY = Mathf.RoundToInt(transform.position.y);
        int worldZ = Mathf.RoundToInt(transform.position.z);

        int blockX, blockY, blockZ;
        blockX = worldX; blockY = worldY; blockZ = worldZ;
        if ((worldBlockData[blockX+1, blockY + 1, blockZ].aabb.isEnable)
            && (worldBlockData[blockX+1, blockY + 1, blockZ].aabb.IsInterSectAABB(aabb))) return true;
        if ((worldBlockData[blockX, blockY + 1, blockZ+1].aabb.isEnable)
            && (worldBlockData[blockX, blockY + 1, blockZ+1].aabb.IsInterSectAABB(aabb))) return true;
        if ((worldBlockData[blockX-1, blockY + 1, blockZ].aabb.isEnable)
            && (worldBlockData[blockX-1, blockY + 1, blockZ].aabb.IsInterSectAABB(aabb))) return true;
        if ((worldBlockData[blockX, blockY + 1, blockZ -1].aabb.isEnable)
           && (worldBlockData[blockX, blockY + 1, blockZ -1].aabb.IsInterSectAABB(aabb))) return true;

        return false;
    }

    private bool IsCollsionFloor()
    {
        int worldX = Mathf.RoundToInt(transform.position.x);
        int worldY = Mathf.RoundToInt(transform.position.y);
        int worldZ = Mathf.RoundToInt(transform.position.z);

        int blockX, blockY, blockZ;
        blockX = worldX; blockY = worldY; blockZ = worldZ;
        if ((worldBlockData[blockX, blockY, blockZ].aabb.isEnable)
            && (worldBlockData[blockX, blockY, blockZ].aabb.IsInterSectAABB(aabb)))
        {
            return true;
        }
        return false;
    }

    private IEnumerator MoveProcess()
    {
        CalcPathFinding();
        PathNode node = null;
        while (pathTrace.Count > 0){
            yield return new WaitForSeconds(1.0f);
            Debug.Log("pathTrace count : " + pathTrace.Count);
            node = pathTrace.Pop();
            Vector3 newPos = new Vector3(node.pathMapDataX, node.worldCoordY, node.pathMapDataZ);
            int diff = Mathf.RoundToInt(transform.position.y - newPos.y);
            if (Mathf.Abs(diff) >= 1)
            {
                if (node.isJumped){
                    newPos += new Vector3(0, 1, 0);
                    node.isJumped = false;
                }
                transform.position = newPos;
                CalcPathFinding();
            }
            else
            {
                if (node.isJumped){
                    newPos += new Vector3(0, 1, 0);
                    node.isJumped = false;
                }
                transform.position = newPos;
            }
        }

        Debug.Log("End Move Process");
    }

    private void CalcPathFinding()
    {
        Debug.Log("NPC pos :: " + transform.position);
        pathTrace.Clear();
        Stack<PathNode> reversePath = pathFinder.PathFinding();
        if (reversePath != null)
        {
            foreach (PathNode p in reversePath)
            {
                pathTrace.Push(p);
            }
        }
    }

    private IEnumerator SimpleGravityForce()
    {
        while (true)
        {
            
            if (!IsCollsionFloor()) transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator ReMakeAABBProcess()
    {
        while (true)
        {
            ReMakeAABB();
            yield return null;
        }
    }

    private void ReMakeAABB()
    {
        aabb.MakeAABB(objectMinExtent.position, objectMaxExtent.position);
    }
}


