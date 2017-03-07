using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomOBB : MonoBehaviour
{
    public Vector3 xAxis { get { return _objTrans.right; } }
    public Vector3 yAxis { get { return _objTrans.up; } }
    public Vector3 zAxis { get { return _objTrans.forward; } }
    public Vector3 center { get { return _objTrans.position; } }
    private Transform _objTrans;
    private Transform _maxExtent;
    public float xRadius { get { return Mathf.Abs(_maxExtent.position.x - _objTrans.position.x); } }
    public float yRadius { get { return Mathf.Abs(_maxExtent.position.y - _objTrans.position.y); } }
    public float zRadius { get { return Mathf.Abs(_maxExtent.position.z - _objTrans.position.z); } }

    public void Init(Transform trans, Transform maxExtent)
    {
        _objTrans = trans;
        _maxExtent = maxExtent;
    }

    public static bool Collide(CustomOBB a, CustomOBB b)
    {
        //factors.
        float r1, r2, T;
        Vector3 betweenOBB = b.center - a.center;
        Matrix4x4 aR = Matrix4x4.TRS(Vector3.zero, a.transform.rotation, Vector3.zero);
        Matrix4x4 bR = Matrix4x4.TRS(Vector3.zero, b.transform.rotation, Vector3.zero);
        Matrix4x4 toAR = aR.inverse * bR;
        //betweenOBB = aR * betweenOBB;
        
        //# A xAxis 
        r1 = a.xRadius;
        r2 = b.xRadius * Mathf.Abs(toAR.m00) + b.yRadius * Mathf.Abs(toAR.m01) +
            b.zRadius * Mathf.Abs(toAR.m02);
        T = Mathf.Abs(betweenOBB.x);
        if (T > r1 + r2) return false;
        //# A yAxis
        r1 = a.yRadius;
        r2 = b.xRadius * Mathf.Abs(toAR.m10) + b.yRadius * Mathf.Abs(toAR.m11) +
            b.zRadius * Mathf.Abs(toAR.m12);
        T = Mathf.Abs(betweenOBB.y);
        if (T > r1 + r2) return false;
        //# A zAxis
        r1 = a.zRadius;
        r2 = b.xRadius * Mathf.Abs(toAR.m20) + b.yRadius * Mathf.Abs(toAR.m21) +
            b.zRadius * Mathf.Abs(toAR.m22);
        T = Mathf.Abs(betweenOBB.z);
        if (T > r1 + r2) return false;
        //# B xAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m00) + a.yRadius * Mathf.Abs(toAR.m10) +
            a.zRadius * Mathf.Abs(toAR.m20);
        r2 = b.xRadius;
        T = Mathf.Abs(betweenOBB.x * toAR.m00 +
            betweenOBB.y * toAR.m10 +
            betweenOBB.z + toAR.m20);
        if (T > r1 + r2) return false;
        // # B yAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m01) + a.yRadius * Mathf.Abs(toAR.m11) +
            a.zRadius * Mathf.Abs(toAR.m21);
        r2 = b.yRadius;
        T = Mathf.Abs(betweenOBB.x * toAR.m01 +
            betweenOBB.y * toAR.m11 +
            betweenOBB.z + toAR.m21);
        if (T > r1 + r2) return false;
        // # B zAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m02) + a.yRadius * Mathf.Abs(toAR.m12) +
            a.zRadius * Mathf.Abs(toAR.m22);
        r2 = b.zRadius;
        T = Mathf.Abs(betweenOBB.x * toAR.m02 +
            betweenOBB.y * toAR.m12 +
            betweenOBB.z + toAR.m22);
        if (T > r1 + r2) return false;

        // A와 B오브젝트의 각 축에 대한 외적으로 만든 축.
        // A xAsis Cross RB xAxis
        r1 = a.yRadius * Mathf.Abs(toAR.m20) +
            a.zRadius * Mathf.Abs(toAR.m10);
        r2 = b.yRadius * Mathf.Abs(toAR.m02) +
            b.zRadius * Mathf.Abs(toAR.m01);
        T = Mathf.Abs(-betweenOBB.y * toAR.m20 + betweenOBB.z * toAR.m10);
        if (T > r1 + r2) return false;
        // A xAsis Cross RB yAxis
        r1 = a.yRadius * Mathf.Abs(toAR.m21) +
            a.zRadius * Mathf.Abs(toAR.m11);
        r2 = b.xRadius * Mathf.Abs(toAR.m02) +
            b.zRadius * Mathf.Abs(toAR.m00);
        T = Mathf.Abs(-betweenOBB.y * toAR.m21 + betweenOBB.z * toAR.m11);
        if (T > r1 + r2) return false;
        // A xAsis Cross RB zAxis
        r1 = a.yRadius * Mathf.Abs(toAR.m22) +
            a.zRadius * Mathf.Abs(toAR.m12);
        r2 = b.xRadius * Mathf.Abs(toAR.m01) +
            b.yRadius * Mathf.Abs(toAR.m00);
        T = Mathf.Abs(-betweenOBB.y * toAR.m22 + betweenOBB.z * toAR.m12);
        if (T > r1 + r2) return false;

        // A yAsis Cross RB xAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m20) +
            a.zRadius * Mathf.Abs(toAR.m00);
        r2 = b.yRadius * Mathf.Abs(toAR.m12) +
            b.zRadius * Mathf.Abs(toAR.m11);
        T = Mathf.Abs(betweenOBB.x * toAR.m20 - betweenOBB.z * toAR.m00);
        if (T > r1 + r2) return false;
        // A yAsis Cross RB yAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m21) +
            a.zRadius * Mathf.Abs(toAR.m01);
        r2 = b.xRadius * Mathf.Abs(toAR.m12) +
            b.zRadius * Mathf.Abs(toAR.m10);
        T = Mathf.Abs(betweenOBB.x * toAR.m21 - betweenOBB.z * toAR.m01);
        if (T > r1 + r2) return false;
        // A yAsis Cross RB zAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m22) +
            a.zRadius * Mathf.Abs(toAR.m02);
        r2 = b.xRadius * Mathf.Abs(toAR.m11) +
            b.yRadius * Mathf.Abs(toAR.m10);
        T = Mathf.Abs(betweenOBB.x * toAR.m22 - betweenOBB.z * toAR.m02);
        if (T > r1 + r2) return false;

        // A zAsis Cross RB xAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m10) +
            a.yRadius * Mathf.Abs(toAR.m00);
        r2 = b.yRadius * Mathf.Abs(toAR.m22) +
            b.zRadius * Mathf.Abs(toAR.m21);
        T = Mathf.Abs(-betweenOBB.x * toAR.m10 + betweenOBB.y * toAR.m00);
        if (T > r1 + r2) return false;
        // A zAsis Cross RB yAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m11) +
            a.yRadius * Mathf.Abs(toAR.m01);
        r2 = b.xRadius * Mathf.Abs(toAR.m22) +
            b.zRadius * Mathf.Abs(toAR.m20);
        T = Mathf.Abs(-betweenOBB.x * toAR.m11 + betweenOBB.y * toAR.m01);
        if (T > r1 + r2) return false;
        // A zAsis Cross RB zAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m12) +
            a.yRadius * Mathf.Abs(toAR.m02);
        r2 = b.xRadius * Mathf.Abs(toAR.m21) +
            b.yRadius * Mathf.Abs(toAR.m20);
        T = Mathf.Abs(-betweenOBB.x * toAR.m12 + betweenOBB.y * toAR.m02);
        if (T > r1 + r2) return false;

        return true;
    }
}
