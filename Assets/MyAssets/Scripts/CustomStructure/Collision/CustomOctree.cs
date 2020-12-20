using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MapGenLib;
//                      X    Y     Z
//root 노드 -> center(15.5, 15.5, 15.5)

//0번 노드 -> center "(7.5, 7.5, 7.5)"
//1번 노드 -> center "(23.5, 7.5, 7.5)"
//2번 노드 -> center "(23.5, 23.5, 7.5)"
//3번 노드 -> center "(7.5, 23.5, 7.5)"
//4번 노드 -> center "(7.5, 7.5, 23.5)"
//5번 노드 -> center "(23.5, 7.5, 23.5)"
//6번 노드 -> center "(23.5, 23.5, 23.5)"
//7번 노드 -> center "(7.5, 23.5, 23.5)"

//root를 기준으로  중심 값을 비교했을 때

//0번 노드 -> x, y ,z 모두 작다.
//1번 노드 -> x 크고, y, z는 작다.
//2번 노드 -> x ,y는 크고 z는 작다.
//3번 노드 -> x, z는 작고 y는 크다.
//4번 노드 -> x, y는 작고 z는 크다
//5번 노드 -> x, z는 크고 y는 작다.
//6번 노드 -> x, y, z 모두 크다.
//7번 노드 -> x는 작고, y, z는 크다.

public class COTNode
{
    public CustomAABB aabb;
    public COTNode[] Childs { get; } = new COTNode[8];
    // 노드의 중점은 게임 내 존재하는 블록의 중점과 같다. ( aabb의 중점도 마찬가지로 동일.)
    public Vector3 Center { set; get; }
    public Vector3 Size { set; get; }
    public bool bCanDelete { set; get; }

    public COTNode()
    {
        bCanDelete = false;
    }
}
/// <summary>
/// 충돌한 지점에 대한 정보 구조체.
/// </summary>
public struct CollideInfo
{
    public bool bCollide;
    public Vector3 HitBlockCenter;
    public Vector3 CollisionPoint;
    public CustomAABB aabb;
    public Block GetBlock()
    {
        var containWorld = WorldAreaManager.Instance.ContainedSubWorld(HitBlockCenter);
        Vector3 worldCoord = WorldAreaManager.GetRealCoordToWorldDataCoord(HitBlockCenter);
        return containWorld.WorldBlockData[(int)worldCoord.x, (int)worldCoord.y, (int)worldCoord.z];
    }
}

public class CustomOctree
{
    private COTNode Root;
    // 블록생성시 정중앙을 0,0,0으로 맞추기 위해 x,y,z 에 -0.5f, +0.5f, -0.5f 씩 오프셋이 적용되어 있다.
    // 따라서, Octree 노드에 있는 AABB min, max또한 블록에 적용된 오프셋을 감안해서 별도의 오프셋을 적용한다.
    // 값은 아래와 같다.
    private Vector3 Offset = new Vector3(-0.5f, -0.5f, -0.5f);
    // 블록의 최소 단위.
    private Vector3 BlockMinSize = new Vector3(1.0f, 1.0f, 1.0f);
    // root min, max bound 의 경우 Octree가 생성하는 Node들중의
    // 가장 최상위 노드의 extent를 의미한다.
    public Vector3 RootMinBound { get; private set; }
    public Vector3 RootMaxBound { get; private set; }
    /// <summary>
    /// 전체 지형를 감싸는 바운딩박스의 Min, MaxExtent로  Octree의 초기화를 합니다.
    /// </summary>
    /// <param name="_minBound"></param>
    /// <param name="_maxBound"></param>
    public void Init(Vector3 _minBound, Vector3 _maxBound)
    {
        // 실제 월드에 존재하는 각 블록들의 렌더링 되는 시작점에서 해당 오프셋을 적용.
        RootMinBound = _minBound + Offset;
        RootMaxBound = _maxBound + Offset;
        Vector3 center = (RootMaxBound + RootMinBound) /2;
        Root = new COTNode
        {
            Center = center,
            Size = RootMaxBound - RootMinBound
        };
        Root.aabb.MakeAABB(RootMinBound, RootMaxBound);
    }
    /// <summary>
    /// Gizmo를 이용해 Octree의 모든노드를 그려준다.
    /// ((OnDrawGizmos() 에서 호출되어야 한다.))
    /// </summary>
    public void DrawFullTree()
    {
        if(Root != null) DrawAllNodes(ref Root);
    }
    private void DrawAllNodes(ref COTNode node)
    {
        Gizmos.color = Color.yellow;
        if (node.Size == BlockMinSize) Gizmos.DrawWireCube(node.Center, node.Size);
        for (int i = 0; i < 8; i++)
        {
            if (node.Childs[i] == null) continue;
            if (node.Size == BlockMinSize) Gizmos.DrawWireCube(node.Childs[i].Center, node.Childs[i].Size);
            DrawAllNodes(ref node.Childs[i]);
        }
    }
    /// <summary>
    /// Octree에 특정 위치에 노드를 추가합니다.
    /// </summary>
    /// <param name="pos">추가할 위치.</param>
    public void Add(Vector3 pos)
    {
        AddNode(pos, ref Root);
    }
    /// <summary>
    /// Octree에 특정 위치의 노드를 삭제 합니다.
    /// </summary>
    /// <param name="pos">삭제할 위치.</param>
    public void Delete(Vector3 pos)
    {
        DeleteNode(pos, ref Root);
    }
    /// <summary>
    /// Octree가 생성할 수 있는 최대 노드까지 생성합니다. ( 최대 깊이 만큼.)
    /// </summary>
    public void CreateFullTree()
    {
        CreateALLNodes(ref Root, 1);
    }

    public CollideInfo Collide(Vector3 point)
    {
        return CollideWithPoint(point, ref Root);
    }
    private CollideInfo CollideWithPoint(Vector3 point, ref COTNode root)
    {
        CollideInfo info;
        info.bCollide = false;
        info.HitBlockCenter = Vector3.zero;
        info.CollisionPoint = Vector3.zero;
        info.aabb = root.aabb;
        if (root.Size == BlockMinSize)
        {
            info.bCollide = true;
            info.HitBlockCenter = root.Center;
            info.CollisionPoint = root.Center;
            return info;
        }
        for (int i = 0; i < 8; i++)
        {
            if ((root.Childs[i] != null) &&
               (root.Childs[i].aabb.IsInterSectPoint(point)))
            {
                return CollideWithPoint(point, ref root.Childs[i]);
            }
        }
        return info;
    }

    public CollideInfo Collide(Ray ray)
    {
        List<CollideInfo> collideCandidate = new List<CollideInfo>();
        CollideNodeWithRay(ray, ref Root, ref collideCandidate);

        // init.
        CollideInfo info;
        info.bCollide = false;
        info.HitBlockCenter = Vector3.zero;
        info.CollisionPoint = Vector3.zero;
        info.aabb = Root.aabb;
        if (collideCandidate.Count > 0)
        {
            float minDist = Vector3.Distance(ray.origin, collideCandidate[0].HitBlockCenter);
            info.bCollide = true;
            info.HitBlockCenter = collideCandidate[0].HitBlockCenter;
            info.CollisionPoint = collideCandidate[0].CollisionPoint;
            for (int i = 1; i < collideCandidate.Count; i++)
            {
                float d = Vector3.Distance(ray.origin, collideCandidate[i].HitBlockCenter);
                if (minDist > d)
                {
                    minDist = d;
                    info.HitBlockCenter = collideCandidate[i].HitBlockCenter;
                    info.CollisionPoint = collideCandidate[i].CollisionPoint;
                }
            }
        }
        return info;
    }
    /// <summary>
    ///  Octree 중에 광선과 충돌하는 노드를 찾습니다.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="root"></param>
    private void CollideNodeWithRay(Ray ray, ref COTNode root, ref List<CollideInfo> collideCandidate)
    {
        if (root == null) return;

        CollideInfo info;
        info.bCollide = false;
        info.HitBlockCenter = Vector3.zero;
        info.CollisionPoint = Vector3.zero;
        info.aabb = root.aabb;
        if (root.Size == BlockMinSize)
        {
            info.bCollide = true;
            info.HitBlockCenter = root.Center;
            info.CollisionPoint = root.aabb.HitPointWithRay;
            collideCandidate.Add(info);
        }
        for (int i = 0; i < 8; i++)
        {
            if ((root.Childs[i] != null) &&
                (CustomRayCast.InterSectWithAABB(ray, ref root.Childs[i].aabb)))
            {
                CollideNodeWithRay(ray, ref root.Childs[i], ref collideCandidate);
            }
        }
    }

    public CollideInfo Collide(ref CustomAABB other)
    {
        return CollideNodeWithAABB(ref other, ref Root);
    }

    /// <summary>
    /// Octree 노드 중에 특정 AABB와 충돌하는 노드를 찾습니다.
    /// </summary>
    /// <param name="other"></param>
    /// <param name="root"></param>
    private CollideInfo CollideNodeWithAABB(ref CustomAABB other, ref COTNode root)
    {
        CollideInfo info;
        info.bCollide = false;
        info.HitBlockCenter = Vector3.zero;
        info.CollisionPoint = Vector3.zero;
        info.aabb = root.aabb;
        if (root.Size == BlockMinSize) 
        {
            info.bCollide = true;
            info.HitBlockCenter = root.Center;
            return info;
        }
        for (int i = 0; i < 8; i++)
        {
            if ((root.Childs[i] != null) &&
               (root.Childs[i].aabb.IsInterSectAABB(other)))
            {
                return CollideNodeWithAABB(ref other, ref root.Childs[i]);
            }
        }
        return info;
    }

    private void DeleteNode(Vector3 pos, ref COTNode root)
    {
        if (root.Center == pos)
        {
            for (int i = 0; i < 8; i++) root.Childs[i] = null;
            root.bCanDelete = true;
            return;
        }
        else if ((root.Center.x > pos.x) && (root.Center.y > pos.y) && (root.Center.z > pos.z))
        {
            // 0번 노드.
            if (root.Childs[0] != null)
            {
                DeleteNode(pos, ref root.Childs[0]);
                if (root.Childs[0].bCanDelete) root.Childs[0] = null;
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y > pos.y) && (root.Center.z > pos.z))
        {
            // 1번 노드.
            if (root.Childs[1] != null)
            {
                DeleteNode(pos, ref root.Childs[1]);
                if (root.Childs[1].bCanDelete) root.Childs[1] = null;
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y < pos.y) && (root.Center.z > pos.z))
        {
            // 2번 노드.
            if (root.Childs[2] != null)
            {
                DeleteNode(pos, ref root.Childs[2]);
                if (root.Childs[2].bCanDelete) root.Childs[2] = null;
            }
        }
        else if ((root.Center.x > pos.x) && (root.Center.y < pos.y) && (root.Center.z > pos.z))
        {
            // 3번 노드.
            if (root.Childs[3] != null)
            {
                DeleteNode(pos, ref root.Childs[3]);
                if (root.Childs[3].bCanDelete) root.Childs[3] = null;
            }
        }
        else if ((root.Center.x > pos.x) && (root.Center.y > pos.y) && (root.Center.z < pos.z))
        {
            // 4번 노드.
            if (root.Childs[4] != null)
            {
                DeleteNode(pos, ref root.Childs[4]);
                if (root.Childs[4].bCanDelete) root.Childs[4] = null;
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y > pos.y) && (root.Center.z < pos.z))
        {
            //5번 노드.
            if (root.Childs[5] != null)
            {
                DeleteNode(pos, ref root.Childs[5]);
                if (root.Childs[5].bCanDelete) root.Childs[5] = null;
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y < pos.y) && (root.Center.z < pos.z))
        {
            // 6번 노드.
            if (root.Childs[6] != null)
            {
                DeleteNode(pos, ref root.Childs[6]);
                if (root.Childs[6].bCanDelete) root.Childs[6] = null;
            }
        }
        else if ((root.Center.x > pos.x) && (root.Center.y < pos.y) && (root.Center.z < pos.z))
        {
            // 7번 노드.
            if (root.Childs[7] != null)
            {
                DeleteNode(pos, ref root.Childs[7]);
                if (root.Childs[7].bCanDelete) root.Childs[7] = null;
            }
        }
    }

    private void AddNode(Vector3 pos, ref COTNode root)
    {
        Vector3 parentCenter = root.Center;
        float parentHalfWidth = root.aabb.MaxExtent.x - parentCenter.x;
        float parentHalfHeight = root.aabb.MaxExtent.y - parentCenter.y;
        float parentHalfDepth = root.aabb.MaxExtent.z - parentCenter.z;
        Vector3 minExtent, maxExtent;

        if (root.Center == pos)
        {
            return;
        }
        else if ((root.Center.x > pos.x) && (root.Center.y > pos.y) &&  (root.Center.z > pos.z))
        {
            // 0번 노드.
            if(root.Childs[0] != null)
            {
                AddNode(pos, ref root.Childs[0]);
            }
            else
            {
                root.Childs[0] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentCenter.y - parentHalfHeight, parentCenter.z - parentHalfDepth);
                maxExtent = parentCenter;
                root.Childs[0].Center = (maxExtent + minExtent) / 2;
                root.Childs[0].Size = maxExtent - minExtent;
                root.Childs[0].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[0]);
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y > pos.y) && (root.Center.z > pos.z))
        {
            // 1번 노드.
            if (root.Childs[1] != null)
            {
                AddNode(pos, ref root.Childs[1]);
            }
            else
            {
                root.Childs[1] = new COTNode();
                minExtent = new Vector3(parentCenter.x, parentCenter.y - parentHalfHeight, parentCenter.z - parentHalfDepth);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth, parentCenter.y, parentCenter.z);
                root.Childs[1].Center = (maxExtent + minExtent) / 2;
                root.Childs[1].Size = maxExtent - minExtent;
                root.Childs[1].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[1]);
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y < pos.y) && (root.Center.z > pos.z))
        {
            // 2번 노드.
            if (root.Childs[2] != null)
            {
                AddNode(pos, ref root.Childs[2]);
            }
            else
            {
                root.Childs[2] = new COTNode();
                minExtent = new Vector3(parentCenter.x, parentCenter.y, parentCenter.z - parentHalfDepth);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth, parentCenter.y + parentHalfHeight, parentCenter.z);
                root.Childs[2].Center = (maxExtent + minExtent) / 2;
                root.Childs[2].Size = maxExtent - minExtent;
                root.Childs[2].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[2]);
            }
        }
        else if ((root.Center.x > pos.x) && (root.Center.y < pos.y) && (root.Center.z > pos.z))
        {
            // 3번 노드.
            if (root.Childs[3] != null)
            {
                AddNode(pos, ref root.Childs[3]);
            }
            else
            {
                root.Childs[3] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentCenter.y, parentCenter.z - parentHalfDepth);
                maxExtent = new Vector3(parentCenter.x, parentCenter.y + parentHalfHeight, parentCenter.z);
                root.Childs[3].Center = (maxExtent + minExtent) / 2;
                root.Childs[3].Size = maxExtent - minExtent;
                root.Childs[3].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[3]);
            }
        }
        else if ((root.Center.x > pos.x) && (root.Center.y > pos.y) && (root.Center.z < pos.z))
        {
            // 4번 노드.
            if (root.Childs[4] != null)
            {
                AddNode(pos, ref root.Childs[4]);
            }
            else
            {
                root.Childs[4] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentCenter.y - parentHalfHeight, parentCenter.z);
                maxExtent = new Vector3(parentCenter.x, parentCenter.y, parentCenter.z + parentHalfDepth);
                root.Childs[4].Center = (maxExtent + minExtent) / 2;
                root.Childs[4].Size = maxExtent - minExtent;
                root.Childs[4].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[4]);
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y > pos.y) && (root.Center.z < pos.z))
        {
            //5번 노드.
            if (root.Childs[5] != null)
            {
                AddNode(pos, ref root.Childs[5]);
            }
            else
            {
                root.Childs[5] = new COTNode();
                minExtent = new Vector3(parentCenter.x, parentCenter.y - parentHalfHeight, parentCenter.z);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth, parentCenter.y, parentCenter.z + parentHalfDepth);
                root.Childs[5].Center = (maxExtent + minExtent) / 2;
                root.Childs[5].Size = maxExtent - minExtent;
                root.Childs[5].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[5]);
            }
        }
        else if ((root.Center.x < pos.x) && (root.Center.y < pos.y) && (root.Center.z < pos.z))
        {
            // 6번 노드.
            if (root.Childs[6] != null)
            {
                AddNode(pos, ref root.Childs[6]);
            }
            else
            {
                root.Childs[6] = new COTNode();
                minExtent = new Vector3(parentCenter.x, parentCenter.y, parentCenter.z);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth, parentCenter.y + parentHalfHeight, parentCenter.z + parentHalfDepth);
                root.Childs[6].Center = (maxExtent + minExtent) / 2;
                root.Childs[6].Size = maxExtent - minExtent;
                root.Childs[6].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[6]);
            }
        }
        else if ((root.Center.x > pos.x) && (root.Center.y < pos.y) && (root.Center.z < pos.z))
        {
            // 7번 노드.
            if (root.Childs[7] != null)
            {
                AddNode(pos, ref root.Childs[7]);
            }
            else
            {
                root.Childs[7] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentCenter.y, parentCenter.z);
                maxExtent = new Vector3(parentCenter.x, parentCenter.y + parentHalfHeight, parentCenter.z + parentHalfDepth);
                root.Childs[7].Center = (maxExtent + minExtent) / 2;
                root.Childs[7].Size = maxExtent - minExtent;
                root.Childs[7].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, ref root.Childs[7]);
            }
        }

    }
    private void CreateALLNodes(ref COTNode root, int maxDepth)
    {
        if (maxDepth == 0) return;

        Vector3 parentCenter = root.Center;
        float parentHalfWidth = root.aabb.MaxExtent.x - parentCenter.x;
        float parentHalfHeight = root.aabb.MaxExtent.y- parentCenter.y;
        float parentHalfDepth = root.aabb.MaxExtent.z - parentCenter.z;

        Vector3 minExtent, maxExtent;
        root.Childs[0] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y - parentHalfHeight,
            parentCenter.z - parentHalfDepth);
        maxExtent = parentCenter;
        root.Childs[0].Center = (maxExtent + minExtent) / 2;
        root.Childs[0].Size = maxExtent - minExtent;
        root.Childs[0].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[0], maxDepth -1);

        root.Childs[1] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y - parentHalfHeight,
            parentCenter.z - parentHalfDepth);
        maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
            parentCenter.y,
            parentCenter.z);
        root.Childs[1].Center = (maxExtent + minExtent) / 2;
        root.Childs[1].Size = maxExtent - minExtent;
        root.Childs[1].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[1], maxDepth - 1);

        root.Childs[2] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y,
            parentCenter.z - parentHalfDepth);
        maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
            parentCenter.y + parentHalfHeight,
            parentCenter.z);
        root.Childs[2].Center = (maxExtent + minExtent) / 2;
        root.Childs[2].Size = maxExtent - minExtent;
        root.Childs[2].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[2], maxDepth - 1);

        root.Childs[3] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y,
            parentCenter.z - parentHalfDepth);
        maxExtent = new Vector3(parentCenter.x,
            parentCenter.y + parentHalfHeight,
            parentCenter.z);
        root.Childs[3].Center = (maxExtent + minExtent) / 2;
        root.Childs[3].Size = maxExtent - minExtent;
        root.Childs[3].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[3], maxDepth - 1);

        root.Childs[4] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y - parentHalfHeight,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x,
            parentCenter.y,
            parentCenter.z + parentHalfDepth);
        root.Childs[4].Center = (maxExtent + minExtent) / 2;
        root.Childs[4].Size = maxExtent - minExtent;
        root.Childs[4].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[4], maxDepth - 1);

        root.Childs[5] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y - parentHalfHeight,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
            parentCenter.y,
            parentCenter.z + parentHalfDepth);
        root.Childs[5].Center = (maxExtent + minExtent) / 2;
        root.Childs[5].Size = maxExtent - minExtent;
        root.Childs[5].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[5], maxDepth - 1);

        root.Childs[6] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x +parentHalfWidth,
            parentCenter.y + parentHalfHeight,
            parentCenter.z + parentHalfDepth);
        root.Childs[6].Center = (maxExtent + minExtent) / 2;
        root.Childs[6].Size = maxExtent - minExtent;
        root.Childs[6].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[6], maxDepth - 1);

        root.Childs[7] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x,
            parentCenter.y + parentHalfHeight,
            parentCenter.z + parentHalfDepth);
        root.Childs[7].Center = (maxExtent + minExtent) / 2;
        root.Childs[7].Size = maxExtent - minExtent;
        root.Childs[7].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(ref root.Childs[7], maxDepth - 1);
    }

}
