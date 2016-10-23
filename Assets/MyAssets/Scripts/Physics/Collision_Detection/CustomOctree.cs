using UnityEngine;
using System.Collections;

public class COTNode
{
    private CustomAABB aabb;
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
        set { _maxExtent = value; }
        get { return _maxExtent; }
    }

    private Vector3 _center;
    public Vector3 center
    {
        set { _center = value; }
        get { return _center; }
    }

    public void MakeAABB()
    {
        aabb.MakeAABB(_minExtent, _maxExtent);
        aabb.isEnable = false;
    }

}
public class CustomOctree : MonoBehaviour
{

    private Vector3 maxBound;
    private Vector3 minBound;
    private COTNode root;

    void Start()
    {
        Init(new Vector3(0, 0, 0), new Vector3(31, 31, 31));
    }

    void OnDrawGizmos()
    {
        DrawAABB(root);
    }
    private void DrawAABB(COTNode node)
    {
        int next = 0;
        while(node != null)
        {
            for(int i = 0; i<8; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(node.minExtent, node.maxExtent);
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
        maxBound = _maxBound;
        minBound = _minBound;
        root = new COTNode();
        root.maxExtent = maxBound;
        root.minExtent = minBound;
        root.center = maxBound / 2;
        root.MakeAABB();

        CreateTree(root);
    }
    private void CreateTree(COTNode root)
    {
        if ((root.maxExtent.x <= 1) ||
            (root.maxExtent.y <= 1) ||
            (root.maxExtent.z <= 1)) return;

        root.childs[0] = new COTNode();
        root.childs[0].minExtent = root.minExtent;
        root.childs[0].maxExtent = root.maxExtent / 2;
        root.childs[0].center = root.center / 2;
        root.childs[0].MakeAABB();
        CreateTree(root.childs[0]);

       
    }

}
