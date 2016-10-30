using UnityEngine;
using System.Collections;

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
    
    private COTNode[] _childs = new COTNode[8];
    public COTNode[] childs
    {
        get { return _childs; }
    }

    // 노드의 중점은 게임 내 존재하는 블록의 중점과 같다. ( aabb의 중점도 마찬가지로 동일.)
    private Vector3 _center;
    public Vector3 center
    {
        set { _center = value; }
        get { return _center; }
    }
    //// 각 성분은 x, y, z의 길이를 나타낸다. ( 정육면체 ).
    //private Vector3 _size;
    //public Vector3 size
    //{
    //    set { _size = value; }
    //    get { return _size; }
    //}
    private bool _isCanDelete;
    public bool isCanDelete
    {
        set { _isCanDelete = value; }
        get { return _isCanDelete; }
    }

    public COTNode()
    {
        _isCanDelete = false;
    }
}
public class CustomOctree : MonoBehaviour
{
    private COTNode root;
    // 블록생성시 정중앙을 0,0,0으로 맞추기 위해 x,y,z 에 -0.5f, +0.5f, -0.5f 씩 오프셋이 적용되어 있다.
    // 따라서, Octree 노드에 있는 AABB min, max또한 블록에 적용된 오프셋을 감안해서 별도의 오프셋을 적용한다.
    // 값은 아래와 같다.
    private Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);

    /// <summary>
    /// 전체 지형를 감싸는 바운딩박스의 Min, MaxExtent로  Octree의 초기화를 합니다.
    /// </summary>
    /// <param name="_minBound"></param>
    /// <param name="_maxBound"></param>
    public void Init(Vector3 _minBound, Vector3 _maxBound)
    {
        // 실제 월드에 존재하는 각 블록들의 렌더링 되는 시작점에서 해당 오프셋을 빼준다.
        _minBound += offset;
        _maxBound += offset;
        Vector3 center = (_maxBound + _minBound) /2;
        root = new COTNode();
        root.center = center;
        root.aabb.MakeAABB(_minBound, _maxBound);
    }
    /// <summary>
    /// Gizmo를 이용해 Octree의 모든노드를 그려준다.
    /// ((OnDrawGizmos() 에서 호출되어야 한다.))
    /// </summary>
    public void DrawFullTree()
    {
        if(root != null) DrawAllNodes(root);
    }
    private void DrawAllNodes(COTNode node)
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(node.aabb.minExtent, node.aabb.maxExtent);
        Vector3 size = node.aabb.maxExtent - node.aabb.minExtent;
        Gizmos.DrawWireCube(node.center, size);
        for (int i = 0; i < 8; i++)
        {
            if (node.childs[i] == null) continue;
            size = node.childs[i].aabb.maxExtent - node.childs[i].aabb.minExtent;
            Gizmos.DrawWireCube(node.childs[i].center, size);
            DrawAllNodes(node.childs[i]);
        }
    }
    /// <summary>
    /// Octree에 특정 위치에 노드를 추가합니다.
    /// </summary>
    /// <param name="pos">추가할 위치.</param>
    public void Add(Vector3 pos)
    {
        AddNode(pos, root);
    }
    /// <summary>
    /// Octree에 특정 위치의 노드를 삭제 합니다.
    /// </summary>
    /// <param name="pos">삭제할 위치.</param>
    public void Delete(Vector3 pos)
    {
        DeleteNode(pos, root);
    }
    /// <summary>
    /// Octree가 생성할 수 있는 최대 노드까지 생성합니다. ( 최대 깊이 만큼.)
    /// </summary>
    public void CreateFullTree()
    {
        CreateALLNodes(root, 1);
    }

    private void DeleteNode(Vector3 pos, COTNode root)
    {
        if (root.center == pos)
        {
            for (int i = 0; i < 8; i++) root.childs[i] = null;
            root.isCanDelete = true;
            return;
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y > pos.y) &&
            (root.center.z > pos.z))
        {
            // 0번 노드.
            if (root.childs[0] != null)
            {
                DeleteNode(pos, root.childs[0]);
                if (root.childs[0].isCanDelete) root.childs[0] = null;
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y > pos.y) &&
            (root.center.z > pos.z))
        {
            // 1번 노드.
            if (root.childs[1] != null)
            {
                DeleteNode(pos, root.childs[1]);
                if (root.childs[1].isCanDelete) root.childs[1] = null;
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z > pos.z))
        {
            // 2번 노드.
            if (root.childs[2] != null)
            {
                DeleteNode(pos, root.childs[2]);
                if (root.childs[2].isCanDelete) root.childs[2] = null;
            }
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z > pos.z))
        {
            // 3번 노드.
            if (root.childs[3] != null)
            {
                DeleteNode(pos, root.childs[3]);
                if (root.childs[3].isCanDelete) root.childs[3] = null;
            }
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y > pos.y) &&
            (root.center.z < pos.z))
        {
            // 4번 노드.
            if (root.childs[4] != null)
            {
                DeleteNode(pos, root.childs[4]);
                if (root.childs[4].isCanDelete) root.childs[4] = null;
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y > pos.y) &&
            (root.center.z < pos.z))
        {
            //5번 노드.
            if (root.childs[5] != null)
            {
                DeleteNode(pos, root.childs[5]);
                if (root.childs[5].isCanDelete) root.childs[5] = null;
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z < pos.z))
        {
            // 6번 노드.
            if (root.childs[6] != null)
            {
                DeleteNode(pos, root.childs[6]);
                if (root.childs[6].isCanDelete) root.childs[6] = null;
            }
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z < pos.z))
        {
            // 7번 노드.
            if (root.childs[7] != null)
            {
                DeleteNode(pos, root.childs[7]);
                if (root.childs[7].isCanDelete) root.childs[7] = null;
            }
        }
    }

    private void AddNode(Vector3 pos, COTNode root)
    {
        Vector3 parentCenter = root.center;
        float parentHalfWidth = root.aabb.maxExtent.x - parentCenter.x;
        float parentHalfHeight = root.aabb.maxExtent.y - parentCenter.y;
        float parentHalfDepth = root.aabb.maxExtent.z - parentCenter.z;
        Vector3 minExtent, maxExtent;

        if (root.center == pos)
        {
            return;
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y > pos.y) && 
            (root.center.z > pos.z))
        {
            // 0번 노드.
            if(root.childs[0] != null)
            {
                AddNode(pos, root.childs[0]);
            }else
            {
                root.childs[0] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth,
                    parentCenter.y - parentHalfHeight,
                    parentCenter.z - parentHalfDepth);
                maxExtent = parentCenter;
                root.childs[0].center = (maxExtent + minExtent) / 2;
                root.childs[0].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[0]);
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y > pos.y) &&
            (root.center.z > pos.z))
        {
            // 1번 노드.
            if (root.childs[1] != null)
            {
                AddNode(pos, root.childs[1]);
            }
            else
            {
                root.childs[1] = new COTNode();
                minExtent = new Vector3(parentCenter.x,
                    parentCenter.y - parentHalfHeight,
                    parentCenter.z - parentHalfDepth);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
                    parentCenter.y,
                    parentCenter.z);
                root.childs[1].center = (maxExtent + minExtent) / 2;
                root.childs[1].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[1]);
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z > pos.z))
        {
            // 2번 노드.
            if (root.childs[2] != null)
            {
                AddNode(pos, root.childs[2]);
            }
            else
            {
                root.childs[2] = new COTNode();
                minExtent = new Vector3(parentCenter.x,
                    parentCenter.y,
                    parentCenter.z - parentHalfDepth);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
                    parentCenter.y + parentHalfHeight,
                    parentCenter.z);
                root.childs[2].center = (maxExtent + minExtent) / 2;
                root.childs[2].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[2]);
            }
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z > pos.z))
        {
            // 3번 노드.
            if (root.childs[3] != null)
            {
                AddNode(pos, root.childs[3]);
            }
            else
            {
                root.childs[3] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth,
                    parentCenter.y,
                    parentCenter.z - parentHalfDepth);
                maxExtent = new Vector3(parentCenter.x,
                    parentCenter.y + parentHalfHeight,
                    parentCenter.z);
                root.childs[3].center = (maxExtent + minExtent) / 2;
                root.childs[3].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[3]);
            }
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y > pos.y) &&
            (root.center.z < pos.z))
        {
            // 4번 노드.
            if (root.childs[4] != null)
            {
                AddNode(pos, root.childs[4]);
            }
            else
            {
                root.childs[4] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth,
                    parentCenter.y - parentHalfHeight,
                    parentCenter.z);
                maxExtent = new Vector3(parentCenter.x,
                    parentCenter.y,
                    parentCenter.z + parentHalfDepth);
                root.childs[4].center = (maxExtent + minExtent) / 2;
                root.childs[4].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[4]);
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y > pos.y) &&
            (root.center.z < pos.z))
        {
            //5번 노드.
            if (root.childs[5] != null)
            {
                AddNode(pos, root.childs[5]);
            }
            else
            {
                root.childs[5] = new COTNode();
                minExtent = new Vector3(parentCenter.x,
                    parentCenter.y - parentHalfHeight,
                    parentCenter.z);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
                    parentCenter.y,
                    parentCenter.z + parentHalfDepth);
                root.childs[5].center = (maxExtent + minExtent) / 2;
                root.childs[5].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[5]);
            }
        }
        else if ((root.center.x < pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z < pos.z))
        {
            // 6번 노드.
            if (root.childs[6] != null)
            {
                AddNode(pos, root.childs[6]);
            }
            else
            {
                root.childs[6] = new COTNode();
                minExtent = new Vector3(parentCenter.x,
                    parentCenter.y,
                    parentCenter.z);
                maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
                    parentCenter.y + parentHalfHeight,
                    parentCenter.z + parentHalfDepth);
                root.childs[6].center = (maxExtent + minExtent) / 2;
                root.childs[6].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[6]);
            }
        }
        else if ((root.center.x > pos.x) &&
            (root.center.y < pos.y) &&
            (root.center.z < pos.z))
        {
            // 7번 노드.
            if (root.childs[7] != null)
            {
                AddNode(pos, root.childs[7]);
            }
            else
            {
                root.childs[7] = new COTNode();
                minExtent = new Vector3(parentCenter.x - parentHalfWidth,
                    parentCenter.y,
                    parentCenter.z);
                maxExtent = new Vector3(parentCenter.x,
                    parentCenter.y + parentHalfHeight,
                    parentCenter.z + parentHalfDepth);
                root.childs[7].center = (maxExtent + minExtent) / 2;
                root.childs[7].aabb.MakeAABB(minExtent, maxExtent);
                AddNode(pos, root.childs[7]);
            }
        }

    }
    private void CreateALLNodes(COTNode root, int maxDepth)
    {
        if (maxDepth == 0) return;

        Vector3 parentCenter = root.center;
        float parentHalfWidth = root.aabb.maxExtent.x - parentCenter.x;
        float parentHalfHeight = root.aabb.maxExtent.y- parentCenter.y;
        float parentHalfDepth = root.aabb.maxExtent.z - parentCenter.z;

        Vector3 minExtent, maxExtent;
        root.childs[0] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y - parentHalfHeight,
            parentCenter.z - parentHalfDepth);
        maxExtent = parentCenter;
        root.childs[0].center = (maxExtent + minExtent) / 2;
        root.childs[0].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[0], maxDepth -1);

        root.childs[1] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y - parentHalfHeight,
            parentCenter.z - parentHalfDepth);
        maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
            parentCenter.y,
            parentCenter.z);
        root.childs[1].center = (maxExtent + minExtent) / 2;
        root.childs[1].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[1], maxDepth - 1);

        root.childs[2] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y,
            parentCenter.z - parentHalfDepth);
        maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
            parentCenter.y + parentHalfHeight,
            parentCenter.z);
        root.childs[2].center = (maxExtent + minExtent) / 2;
        root.childs[2].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[2], maxDepth - 1);

        root.childs[3] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y,
            parentCenter.z - parentHalfDepth);
        maxExtent = new Vector3(parentCenter.x,
            parentCenter.y + parentHalfHeight,
            parentCenter.z);
        root.childs[3].center = (maxExtent + minExtent) / 2;
        root.childs[3].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[3], maxDepth - 1);

        root.childs[4] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y - parentHalfHeight,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x,
            parentCenter.y,
            parentCenter.z + parentHalfDepth);
        root.childs[4].center = (maxExtent + minExtent) / 2;
        root.childs[4].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[4], maxDepth - 1);

        root.childs[5] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y - parentHalfHeight,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x + parentHalfWidth,
            parentCenter.y,
            parentCenter.z + parentHalfDepth);
        root.childs[5].center = (maxExtent + minExtent) / 2;
        root.childs[5].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[5], maxDepth - 1);

        root.childs[6] = new COTNode();
        minExtent = new Vector3(parentCenter.x,
            parentCenter.y,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x +parentHalfWidth,
            parentCenter.y + parentHalfHeight,
            parentCenter.z + parentHalfDepth);
        root.childs[6].center = (maxExtent + minExtent) / 2;
        root.childs[6].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[6], maxDepth - 1);

        root.childs[7] = new COTNode();
        minExtent = new Vector3(parentCenter.x - parentHalfWidth,
            parentCenter.y,
            parentCenter.z);
        maxExtent = new Vector3(parentCenter.x,
            parentCenter.y + parentHalfHeight,
            parentCenter.z + parentHalfDepth);
        root.childs[7].center = (maxExtent + minExtent) / 2;
        root.childs[7].aabb.MakeAABB(minExtent, maxExtent);
        CreateALLNodes(root.childs[7], maxDepth - 1);
    }

}
