using UnityEngine;
using System.Collections;

public class COTNode
{
    private CustomAABB _aabb;
    
    private COTNode[] _childs = new COTNode[8];
    public COTNode[] childs
    {
        get { return _childs; }
    }
    private Vector3 _minExtent;
    public Vector3 minExtent
    {
        set { _minExtent = value; }
        get { return _minExtent; }
    }
    private Vector3 _maxExtent;
    public Vector3 maxExtent
    {
        set{ _maxExtent = value; }
        get { return _maxExtent; }
    }

    private Vector3 _center;
    public Vector3 center
    {
        set { _center = value; }
        get { return _center; }
    }

    private float _size;
    public float size
    {
        set { _size = value; }
        get { return _size; }
    }

    public void MakeAABB()
    {
        _aabb.MakeAABB(_minExtent, _maxExtent);
        _aabb.isEnable = false;
    }
    public void EnableAABB()
    {
        _aabb.isEnable = true;
    }

}
public class CustomOctree : MonoBehaviour
{
    private COTNode root;

    void Start()
    {
        Init(new Vector3(0, 0, 0), new Vector3(32, 32, 32));
    }

    void OnDrawGizmos()
    {
        //DrawAABB(root);
    }
    private void DrawAABB(COTNode node)
    {
        Gizmos.color = Color.green;
        int next = 0;
        while(node != null)
        {
            for(int i = 0; i<8; i++)
            {
                if (node.childs[i] != null)
                {
                    Gizmos.DrawLine(node.childs[i].minExtent, node.childs[i].maxExtent);
                }
            }
            if (next == 8) break;
            DrawAABB(node.childs[next++]);
        }
    }

    public void Init(Vector3 _minBound, Vector3 _maxBound)
    {
        root = new COTNode();
        root.maxExtent = _maxBound;
        root.minExtent = _minBound;
        root.center = _maxBound / 2;
        root.MakeAABB();

        CreateTree(root, 1);
    }
    private void CreateTree(COTNode root, int maxDepth)
    {
        if (maxDepth == 0) return;

        Vector3 parentCenter = root.center;
        float parentHalfWidth = root.maxExtent.x / 2;
        float parentHalfHeight = root.maxExtent.y / 2;
        float parentHalfDepth = root.maxExtent.z / 2;

        root.childs[0] = new COTNode();
        root.childs[0].minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentCenter.y - parentHalfHeight, parentCenter.z - parentHalfDepth);
        root.childs[0].maxExtent = new Vector3(parentHalfWidth, parentHalfHeight, parentHalfDepth);
        root.childs[0].center = root.childs[0].maxExtent / 2;
        root.childs[0].MakeAABB();
        CreateTree(root.childs[0], maxDepth -1);

        root.childs[1] = new COTNode();
        root.childs[1].minExtent = new Vector3(parentHalfWidth, parentCenter.y - parentHalfHeight, parentCenter.z - parentHalfDepth);
        root.childs[1].maxExtent = new Vector3(parentHalfWidth * 2, parentHalfHeight, parentHalfDepth);
        root.childs[1].center = root.childs[1].maxExtent / 2;
        root.childs[1].MakeAABB();
        CreateTree(root.childs[1], maxDepth - 1);

        root.childs[2] = new COTNode();
        root.childs[2].minExtent = new Vector3(parentHalfWidth, parentHalfHeight, parentCenter.z - parentHalfDepth);
        root.childs[2].maxExtent = new Vector3(parentHalfWidth * 2, parentHalfHeight * 2, parentHalfDepth);
        root.childs[2].center = root.childs[2].maxExtent / 2;
        root.childs[2].MakeAABB();
        CreateTree(root.childs[2], maxDepth - 1);

        root.childs[3] = new COTNode();
        root.childs[3].minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentHalfHeight, parentCenter.z - parentHalfDepth);
        root.childs[3].maxExtent = new Vector3(parentHalfWidth, parentHalfHeight * 2, parentHalfDepth);
        root.childs[3].center = root.childs[3].maxExtent / 2;
        root.childs[3].MakeAABB();
        CreateTree(root.childs[3], maxDepth - 1);

        root.childs[4] = new COTNode();
        root.childs[4].minExtent = new Vector3(parentHalfWidth, parentCenter.y - parentHalfHeight, parentHalfDepth);
        root.childs[4].maxExtent = new Vector3(parentHalfWidth * 2, parentHalfHeight, parentHalfDepth * 2);
        root.childs[4].center = root.childs[4].maxExtent / 2;
        root.childs[4].MakeAABB();
        CreateTree(root.childs[4], maxDepth - 1);

        root.childs[5] = new COTNode();
        root.childs[5].minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentCenter.y - parentHalfHeight, parentHalfDepth);
        root.childs[5].maxExtent = new Vector3(parentHalfWidth, parentHalfHeight, parentHalfDepth * 2);
        root.childs[5].center = root.childs[5].maxExtent / 2;
        root.childs[5].MakeAABB();
        CreateTree(root.childs[5], maxDepth - 1);

        root.childs[6] = new COTNode();
        root.childs[6].minExtent = new Vector3(parentHalfWidth, parentHalfHeight, parentHalfDepth);
        root.childs[6].maxExtent = new Vector3(parentHalfWidth * 2, parentHalfHeight * 2, parentHalfDepth * 2);
        root.childs[6].center = root.childs[6].maxExtent / 2;
        root.childs[6].MakeAABB();
        CreateTree(root.childs[6], maxDepth - 1);

        root.childs[7] = new COTNode();
        root.childs[7].minExtent = new Vector3(parentCenter.x - parentHalfWidth, parentHalfHeight, parentHalfDepth);
        root.childs[7].maxExtent = new Vector3(parentHalfWidth, parentHalfHeight * 2, parentHalfDepth * 2);
        root.childs[7].center = root.childs[7].maxExtent / 2;
        root.childs[7].MakeAABB();
        CreateTree(root.childs[7], maxDepth - 1);
    }

}
