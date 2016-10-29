using UnityEngine;
using System.Collections;

// 각 노드는 정육면체라고 가정.
// -> 모든 변의 길이가 동일.
public class COTNode
{
    public CustomAABB aabb;
    
    private COTNode[] _childs = new COTNode[8];
    public COTNode[] childs
    {
        get { return _childs; }
    }

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
}
public class CustomOctree : MonoBehaviour
{
    private COTNode root;
    // 블록생성시 정중앙을 0,0,0으로 맞추기 위해 x,y,z 에 -0.5f, +0.5f, -0.5f 씩 오프셋이 적용되어 있다.
    // 따라서, Octree 노드에 있는 AABB min, max또한 블록에 적용된 오프셋을 감안해서 별도의 오프셋을 적용한다.
    // 값은 아래와 같다.
    private Vector3 offset = new Vector3(-0.5f, -0.5f, -0.5f);
    void Start()
    {
        Init(new Vector3(0, 0, 0), new Vector3(GameWorldConfig.worldX,
            GameWorldConfig.worldY,
            GameWorldConfig.worldZ));
    }

    void OnDrawGizmos()
    {
        DrawAABB(root);
    }
    
    private void DrawAABB(COTNode node)
    {
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(node.aabb.minExtent, node.aabb.maxExtent);
        Vector3 size = node.aabb.maxExtent - node.aabb.minExtent;
        Gizmos.DrawWireCube(node.center, size);
        for (int i = 0; i < 8; i++)
        {
            COTNode child = node.childs[i];
            if (child == null) continue;
            size = child.aabb.maxExtent - child.aabb.minExtent;
            Gizmos.DrawWireCube(child.center, size);
            DrawAABB(child);
        }
    }

    public void Init(Vector3 _minBound, Vector3 _maxBound)
    {
        // 실제 월드에 존재하는 각 블록들의 렌더링 되는 시작점에서 해당 오프셋을 빼준다.
        _minBound += offset;
        _maxBound += offset;
        Vector3 center = (_maxBound + _minBound) /2;
        root = new COTNode();
        root.center = center;
        root.aabb.MakeAABB(_minBound, _maxBound);

        CreateALLNodes(root, 1);
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
