using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 테스트용 NPC 컨트롤러 스크립트.
/// </summary>
public class NPC_Controller : MonoBehaviour
{
    private CustomAABB aabb;
    [SerializeField]
    private Transform objectMinExtent;
    [SerializeField]
    private Transform objectMaxExtent;

	private World world;
    private CustomAstar pathFinder;
    private Stack<PathNode> pathTrace = new Stack<PathNode>();
    private IEnumerator pMoveProcess;
	public void Init(World _world)
    {
		world = _world;
        aabb.MakeAABB(objectMinExtent.position, objectMaxExtent.position);
        pathFinder = new CustomAstar();
        pathFinder.Init(new PathFinderInitData(world.worldBlockData,
            transform, world.worldOffsetX,
            world.worldOffsetZ));
        StartCoroutine(ReMakeAABBProcess());
        StartCoroutine(SimpleGravityForce());
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
        Gizmos.DrawWireCube(aabb.centerPos, aabb.maxExtent - aabb.minExtent);
    }
    
    private IEnumerator MoveProcess()
    {
        CalcPathFinding();
        PathNode node = null;
        while (pathTrace.Count > 0){
            yield return new WaitForSeconds(1.0f);
            node = pathTrace.Pop();
			Vector3 newPos = new Vector3(node.worldCoordX,
				node.worldCoordY,
				node.worldCoordZ);
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
        pathTrace.Clear();
        pathTrace = pathFinder.PathFinding();
    }

    private IEnumerator SimpleGravityForce()
    {
        while (true)
        {
            CollideInfo collideInfo = world.customOctree.Collide(aabb);
            if(!collideInfo.isCollide) transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            yield return null;
        }
    }

    private IEnumerator ReMakeAABBProcess()
    {
        while (true)
        {
            aabb.MakeAABB(objectMinExtent.position, objectMaxExtent.position);
            yield return null;
        }
    }
}


