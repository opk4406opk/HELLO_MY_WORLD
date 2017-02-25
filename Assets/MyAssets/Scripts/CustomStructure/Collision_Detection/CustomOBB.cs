using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomOBB : MonoBehaviour
{

    public Vector3 xAxis { get { return _xAxis; } }
    public Vector3 yAxis { get { return _yAxis; } }
    public Vector3 zAxis { get { return _zAxis; } }
    private Vector3 _xAxis, _yAxis, _zAxis;
    public Vector3 center { get { return _center; } }
    private Vector3 _center;
    public Transform objTrans { get { return _objTrans; } }
    private Transform _objTrans;

    public float xRadius { get { return _xRadius; } }
    public float yRadius { get { return _yRadius; } }
    public float zRadius { get { return _zRadius; } }
    private float _xRadius, _yRadius, _zRadius;
    public void Init(Transform trans, float xRadius, float yRadius, float zRadius)
    {
        _xRadius = xRadius;
        _yRadius = yRadius;
        _zRadius = zRadius;
        _objTrans = trans;
        ResetCoordinate();
    }

    private void ResetCoordinate()
    {
        _xAxis = _objTrans.right;
        _yAxis = _objTrans.up;
        _zAxis = _objTrans.forward;
        _center = _objTrans.position;
    }

    void OnDrawGizmos()
    {
        Vector3 offsetPos = new Vector3(transform.position.x,
            transform.position.y + 0.5f,
            transform.position.z);
        Gizmos.DrawWireCube(offsetPos, new Vector3(0.5f, 1, 0.5f));
    }

    public static bool Collide(CustomOBB a, CustomOBB b)
    {
        a.ResetCoordinate();
        b.ResetCoordinate();
        //factors.
        float r1, r2, T;
        Vector3 betweenOBB = b.center - a.center;
        Matrix4x4 aR = a.transform.worldToLocalMatrix.inverse;
        Matrix4x4 bR = b.transform.worldToLocalMatrix;
        Matrix4x4 toAR = aR * bR;

        //# A xAxis 
        r1 = a.xRadius;
        r2 = b.xRadius * Mathf.Abs(toAR.m11) + b.yRadius * Mathf.Abs(toAR.m12) +
            b.zRadius * Mathf.Abs(toAR.m13);
        T = Mathf.Abs(betweenOBB.x);
        if (T > r1 + r2) return false;
        //# A yAxis
        r1 = a.yRadius;
        r2 = b.xRadius * Mathf.Abs(toAR.m21) + b.yRadius * Mathf.Abs(toAR.m22) +
            b.zRadius * Mathf.Abs(toAR.m23);
        T = Mathf.Abs(betweenOBB.x);
        if (T > r1 + r2) return false;
        //# A zAxis
        r1 = a.zRadius;
        r2 = b.xRadius * Mathf.Abs(toAR.m31) + b.yRadius * Mathf.Abs(toAR.m32) +
            b.zRadius * Mathf.Abs(toAR.m33);
        T = Mathf.Abs(betweenOBB.x);
        if (T > r1 + r2) return false;
        //# B xAxis
        r1 = Mathf.Abs(a.xRadius * toAR.m11) + Mathf.Abs(a.yRadius * toAR.m21) +
            Mathf.Abs(a.zRadius * toAR.m31);
        r2 = b.xRadius;
        T = Mathf.Abs(betweenOBB.x * toAR.m11 + 
            betweenOBB.y * toAR.m21 + 
            betweenOBB.z + toAR.m31);
        if (T > r1 + r2) return false;
        // # B yAxis
        r1 = Mathf.Abs(a.xRadius * toAR.m12) + Mathf.Abs(a.yRadius * toAR.m22) +
            Mathf.Abs(a.zRadius * toAR.m32);
        r2 = b.yRadius;
        T = Mathf.Abs(betweenOBB.x * toAR.m12 +
            betweenOBB.y * toAR.m22 +
            betweenOBB.z + toAR.m32);
        if (T > r1 + r2) return false;
        // # B zAxis
        r1 = Mathf.Abs(a.xRadius * toAR.m13) + Mathf.Abs(a.yRadius * toAR.m23) +
            Mathf.Abs(a.zRadius * toAR.m33);
        r2 = b.zRadius;
        T = Mathf.Abs(betweenOBB.x * toAR.m13 +
            betweenOBB.y * toAR.m23 +
            betweenOBB.z + toAR.m33);
        if (T > r1 + r2) return false;  
        
        // A와 B오브젝트의 각 축에 대한 외적으로 만든 축.
        // A xAsis Cross RB xAxis
        r1 = a.yRadius * Mathf.Abs(toAR.m31) +
            a.zRadius * Mathf.Abs(toAR.m21);
        r2 = b.yRadius * Mathf.Abs(toAR.m13) +
            b.zRadius * Mathf.Abs(toAR.m12);
        T = Mathf.Abs(-betweenOBB.y * toAR.m31 + betweenOBB.z * toAR.m21);
        if (T > r1 + r2) return false;
        // A xAsis Cross RB yAxis
        r1 = a.yRadius * Mathf.Abs(toAR.m32) +
            a.zRadius * Mathf.Abs(toAR.m22);
        r2 = b.xRadius * Mathf.Abs(toAR.m13) +
            b.zRadius * Mathf.Abs(toAR.m11);
        T = Mathf.Abs(-betweenOBB.y * toAR.m32 + betweenOBB.z * toAR.m22);
        if (T > r1 + r2) return false;
        // A xAsis Cross RB zAxis
        r1 = a.yRadius * Mathf.Abs(toAR.m33) +
            a.zRadius * Mathf.Abs(toAR.m23);
        r2 = b.xRadius * Mathf.Abs(toAR.m12) +
            b.yRadius * Mathf.Abs(toAR.m11);
        T = Mathf.Abs(-betweenOBB.y * toAR.m33 + betweenOBB.z * toAR.m23);
        if (T > r1 + r2) return false;
        
        // A yAsis Cross RB xAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m31) +
            a.zRadius * Mathf.Abs(toAR.m11);
        r2 = b.yRadius * Mathf.Abs(toAR.m23) +
            b.zRadius * Mathf.Abs(toAR.m22);
        T = Mathf.Abs(betweenOBB.x * toAR.m31 - betweenOBB.z * toAR.m11);
        if (T > r1 + r2) return false;
        // A yAsis Cross RB yAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m32) +
            a.zRadius * Mathf.Abs(toAR.m12);
        r2 = b.xRadius * Mathf.Abs(toAR.m23) +
            b.zRadius * Mathf.Abs(toAR.m21);
        T = Mathf.Abs(betweenOBB.x * toAR.m32 - betweenOBB.z * toAR.m12);
        if (T > r1 + r2) return false;
        // A yAsis Cross RB zAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m33) +
            a.zRadius * Mathf.Abs(toAR.m13);
        r2 = b.xRadius * Mathf.Abs(toAR.m22) +
            b.yRadius * Mathf.Abs(toAR.m21);
        T = Mathf.Abs(betweenOBB.x * toAR.m33 - betweenOBB.z * toAR.m13);
        if (T > r1 + r2) return false;

        // A zAsis Cross RB xAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m21) +
            a.yRadius * Mathf.Abs(toAR.m11);
        r2 = b.yRadius * Mathf.Abs(toAR.m33) +
            b.zRadius * Mathf.Abs(toAR.m32);
        T = Mathf.Abs(-betweenOBB.x * toAR.m21 + betweenOBB.y * toAR.m11);
        if (T > r1 + r2) return false;
        // A zAsis Cross RB yAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m22) +
            a.yRadius * Mathf.Abs(toAR.m12);
        r2 = b.xRadius * Mathf.Abs(toAR.m33) +
            b.zRadius * Mathf.Abs(toAR.m31);
        T = Mathf.Abs(-betweenOBB.x * toAR.m22 + betweenOBB.y * toAR.m12);
        if (T > r1 + r2) return false;
        // A zAsis Cross RB zAxis
        r1 = a.xRadius * Mathf.Abs(toAR.m23) +
            a.yRadius * Mathf.Abs(toAR.m13);
        r2 = b.xRadius * Mathf.Abs(toAR.m32) +
            b.yRadius * Mathf.Abs(toAR.m31);
        T = Mathf.Abs(-betweenOBB.x * toAR.m23 + betweenOBB.y * toAR.m13);
        if (T > r1 + r2) return false;

        return true;
    }
}
