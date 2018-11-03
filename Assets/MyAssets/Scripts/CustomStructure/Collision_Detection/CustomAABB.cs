using UnityEngine;
using System.Collections;

public struct CustomAABB {
    public Vector3 minExtent;
    public Vector3 maxExtent;
    // in world space coordinate
    public Vector3 centerPos { get; private set; }
    public bool isEnable { set; get; }

    public void MakeAABB(Vector3[] points)
    {
        minExtent = points[0];
        maxExtent = points[0];
        for(int idx = 1; idx < points.Length; idx++)
        {
            if (points[idx].x < minExtent.x) minExtent.x = points[idx].x;
            else if (points[idx].x > maxExtent.x) maxExtent.x = points[idx].x;
            if (points[idx].y < minExtent.y) minExtent.y = points[idx].y;
            else if (points[idx].y > maxExtent.y) maxExtent.y = points[idx].y;
            if (points[idx].z < minExtent.z) minExtent.z = points[idx].z;
            else if (points[idx].z > maxExtent.z) maxExtent.z = points[idx].z;
        }

        centerPos = (maxExtent + minExtent) / 2;
    }
    public void MakeAABB(Vector3 minExtent, Vector3 maxExtent)
    {
        this.minExtent = minExtent;
        this.maxExtent = maxExtent;
        centerPos = (this.maxExtent + this.minExtent) / 2;
    }

    /// <summary>
    /// 유니티엔진에서 제공하는 BoxCollider를 이용해 CustomAABB를 생성.
    /// </summary>
    /// <param name="boxColl"></param>
    public void MakeAABB(BoxCollider boxColl)
    {
        minExtent = boxColl.bounds.min;
        maxExtent = boxColl.bounds.max;
        centerPos = (maxExtent + minExtent) / 2;
    }

    public bool IsInterSectPoint(Vector3 point)
    {
        if ((point.x > minExtent.x && point.x < maxExtent.x ) &&
           (point.y > minExtent.y && point.y < maxExtent.y) &&
           (point.z > minExtent.z && point.z < maxExtent.z))
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
        if ((minExtent.x <= other.maxExtent.x && maxExtent.x >= other.minExtent.x) &&
         (minExtent.y <= other.maxExtent.y && maxExtent.y >= other.minExtent.y) &&
         (minExtent.z <= other.maxExtent.z && maxExtent.z >= other.minExtent.z))
        {
            return true;
        }
        return false;
    }

}
