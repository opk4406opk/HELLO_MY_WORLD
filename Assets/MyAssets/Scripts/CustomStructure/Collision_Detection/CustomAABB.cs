using UnityEngine;
using System.Collections;

public struct CustomAABB {

    private Vector3 _minExtent;
    public Vector3 minExtent
    {
        get { return _minExtent; }
    }
    private Vector3 _maxExtent;
    public Vector3 maxExtent
    {
        get { return _maxExtent; }
    }
    // in world space coordinate
    private Vector3 _centerPos;
    public Vector3 centerPos
    {
        get { return _centerPos; }
    }

    private bool _isEnable;
    public bool isEnable
    {
        set { _isEnable = value; }
        get { return _isEnable; }
    }

    public void MakeAABB(Vector3[] points)
    {
        _minExtent = points[0];
        _maxExtent = points[0];
        for(int idx = 1; idx < points.Length; idx++)
        {
            if (points[idx].x < _minExtent.x) _minExtent.x = points[idx].x;
            else if (points[idx].x > _maxExtent.x) _maxExtent.x = points[idx].x;
            if (points[idx].y < _minExtent.y) _minExtent.y = points[idx].y;
            else if (points[idx].y > _maxExtent.y) _maxExtent.y = points[idx].y;
            if (points[idx].z < _minExtent.z) _minExtent.z = points[idx].z;
            else if (points[idx].z > _maxExtent.z) _maxExtent.z = points[idx].z;
        }

        _centerPos = (_maxExtent + _minExtent) / 2;
    }
    public void MakeAABB(Vector3 minExtent, Vector3 maxExtent)
    {
        _minExtent = minExtent;
        _maxExtent = maxExtent;
        _centerPos = (_maxExtent + _minExtent) / 2;
    }

    /// <summary>
    /// 유니티엔진에서 제공하는 BoxCollider를 이용해 CustomAABB를 생성.
    /// </summary>
    /// <param name="boxColl"></param>
    public void MakeAABB(BoxCollider boxColl)
    {
        _minExtent = boxColl.bounds.min;
        _maxExtent = boxColl.bounds.max;
        _centerPos = (_maxExtent + _minExtent) / 2;
    }

    public bool IsInterSectPoint(Vector3 point)
    {
        if ((point.x > _minExtent.x && point.x < _maxExtent.x ) &&
           (point.y > _minExtent.y && point.y < _maxExtent.y) &&
           (point.z > _minExtent.z && point.z < _maxExtent.z))
        {
            return true;
        }
        return false;
    }

    public static bool IsInterSectPoint(Vector3 minExtent, Vector3 maxExtent, Vector3 point)
    {
        if ((point.x > minExtent.x && point.x < maxExtent.x) &&
           (point.y > minExtent.y && point.y < maxExtent.y) &&
           (point.z > minExtent.z && point.z < maxExtent.z))
        {
            return true;
        }
        return false;
    }

    public bool IsInterSectAABB(CustomAABB other)
    {
        if ((_minExtent.x <= other.maxExtent.x && _maxExtent.x >= other.minExtent.x) &&
         (_minExtent.y <= other.maxExtent.y && _maxExtent.y >= other.minExtent.y) &&
         (_minExtent.z <= other.maxExtent.z && _maxExtent.z >= other.minExtent.z))
        {
            return true;
        }
        return false;
    }

}
