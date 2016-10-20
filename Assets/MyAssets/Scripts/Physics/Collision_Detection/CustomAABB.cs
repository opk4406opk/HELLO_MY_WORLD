using UnityEngine;
using System.Collections;

public struct CustomAABB {

    private Vector3 _minExtent;
    public Vector3 minExtent
    {
        set { _minExtent = value; }
    }
    private Vector3 _maxExtent;
    public Vector3 maxExtent
    {
        set { _maxExtent = value; }
    }

    public bool IsInterSectPoint(Vector3 point)
    {
        Debug.Log("click point " + point);
        Debug.Log("minExtent " + _minExtent);
        Debug.Log("maxExtent " + _maxExtent);
        if ((point.x >= _minExtent.x && point.x <= _maxExtent.x ) &&
           (point.y >= _minExtent.y && point.y <= _maxExtent.y) &&
           (point.z >= _minExtent.z && point.z <= _maxExtent.z))
        {
            Debug.Log("Wow, IsInterSectPoint");
            return true;
        }
        return false;
    }

}
