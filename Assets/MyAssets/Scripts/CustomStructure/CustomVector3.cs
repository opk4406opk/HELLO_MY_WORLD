using UnityEngine;
using System;
/// <summary>
/// 유니티엔진에 내장된 Vector3를 디컴파일해서 만든 커스텀벡터3.
/// </summary>
[Serializable]
public struct CustomVector3
{
    public const float kEpsilon = 1E-05f;
    public float x;
    public float y;
    public float z;
    private static readonly CustomVector3 zeroVector;
    private static readonly CustomVector3 oneVector;
    private static readonly CustomVector3 upVector;
    private static readonly CustomVector3 downVector;
    private static readonly CustomVector3 leftVector;
    private static readonly CustomVector3 rightVector;
    private static readonly CustomVector3 forwardVector;
    private static readonly CustomVector3 backVector;
    private static readonly CustomVector3 positiveInfinityVector;
    private static readonly CustomVector3 negativeInfinityVector;

    public CustomVector3(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public CustomVector3(float x, float y)
    {
        this.x = x;
        this.y = y;
        this.z = 0f;
    }


    public static CustomVector3 Lerp(CustomVector3 a, CustomVector3 b, float t)
    {
        t = Mathf.Clamp01(t);
        return new CustomVector3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));
    }

    public static CustomVector3 LerpUnclamped(CustomVector3 a, CustomVector3 b, float t)
    {
        return new CustomVector3(a.x + ((b.x - a.x) * t), a.y + ((b.y - a.y) * t), a.z + ((b.z - a.z) * t));
    }

    public static CustomVector3 MoveTowards(CustomVector3 current, CustomVector3 target, float maxDistanceDelta)
    {
        CustomVector3 vector = target - current;
        float magnitude = vector.magnitude;
        if ((magnitude <= maxDistanceDelta) || (magnitude < float.Epsilon))
        {
            return target;
        }
        return (current + ((CustomVector3)((vector / magnitude) * maxDistanceDelta)));
    }
   

    public void Set(float newX, float newY, float newZ)
    {
        this.x = newX;
        this.y = newY;
        this.z = newZ;
    }

    public static CustomVector3 Scale(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    public void Scale(CustomVector3 scale)
    {
        this.x *= scale.x;
        this.y *= scale.y;
        this.z *= scale.z;
    }

    public static CustomVector3 Cross(CustomVector3 lhs, CustomVector3 rhs)
    {
        return new CustomVector3((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));
    }

    public override int GetHashCode()
    {
        return ((this.x.GetHashCode() ^ (this.y.GetHashCode() << 2)) ^ (this.z.GetHashCode() >> 2));
    }

    public override bool Equals(object other)
    {
        if (!(other is CustomVector3))
        {
            return false;
        }
        CustomVector3 vector = (CustomVector3)other;
        return ((this.x.Equals(vector.x) && this.y.Equals(vector.y)) && this.z.Equals(vector.z));
    }

    public static CustomVector3 Reflect(CustomVector3 inDirection, CustomVector3 inNormal)
    {
        return (((CustomVector3)((-2f * Dot(inNormal, inDirection)) * inNormal)) + inDirection);
    }

    public static CustomVector3 Normalize(CustomVector3 value)
    {
        float num = Magnitude(value);
        if (num > 1E-05f)
        {
            return (CustomVector3)(value / num);
        }
        return zero;
    }

    public void Normalize()
    {
        float num = Magnitude(this);
        if (num > 1E-05f)
        {
            this = (CustomVector3)(this / num);
        }
        else
        {
            this = zero;
        }
    }

    public CustomVector3 normalized
    {
        get
        {
            return Normalize(this);
        }
    }
    public static float Dot(CustomVector3 lhs, CustomVector3 rhs)
    {
        return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
    }

    public static CustomVector3 Project(CustomVector3 vector, CustomVector3 onNormal)
    {
        float num = Dot(onNormal, onNormal);
        if (num < Mathf.Epsilon)
        {
            return zero;
        }
        return (CustomVector3)((onNormal * Dot(vector, onNormal)) / num);
    }

    public static CustomVector3 ProjectOnPlane(CustomVector3 vector, CustomVector3 planeNormal)
    {
        return (vector - Project(vector, planeNormal));
    }

    public static float Angle(CustomVector3 from, CustomVector3 to)
    {
        return (Mathf.Acos(Mathf.Clamp(Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f);
    }

    public static float SignedAngle(CustomVector3 from, CustomVector3 to, CustomVector3 axis)
    {
        CustomVector3 normalized = from.normalized;
        CustomVector3 rhs = to.normalized;
        float num = Mathf.Acos(Mathf.Clamp(Dot(normalized, rhs), -1f, 1f)) * 57.29578f;
        float num2 = Mathf.Sign(Dot(axis, Cross(normalized, rhs)));
        return (num * num2);
    }

    public static float Distance(CustomVector3 a, CustomVector3 b)
    {
        CustomVector3 vector = new CustomVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        return Mathf.Sqrt(((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
    }

    public static CustomVector3 ClampMagnitude(CustomVector3 vector, float maxLength)
    {
        if (vector.sqrMagnitude > (maxLength * maxLength))
        {
            return (CustomVector3)(vector.normalized * maxLength);
        }
        return vector;
    }

    public static float Magnitude(CustomVector3 vector)
    {
        return Mathf.Sqrt(((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
    }

    public float magnitude
    {
        get
        {
            return Mathf.Sqrt(((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }
    }
    public static float SqrMagnitude(CustomVector3 vector)
    {
        return (((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
    }

    public float sqrMagnitude
    {
        get
        {
            return (((this.x * this.x) + (this.y * this.y)) + (this.z * this.z));
        }
    }
    public static CustomVector3 Min(CustomVector3 lhs, CustomVector3 rhs)
    {
        return new CustomVector3(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y), Mathf.Min(lhs.z, rhs.z));
    }

    public static CustomVector3 Max(CustomVector3 lhs, CustomVector3 rhs)
    {
        return new CustomVector3(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y), Mathf.Max(lhs.z, rhs.z));
    }

    public static CustomVector3 zero
    {
        get
        {
            return zeroVector;
        }
    }
    public static CustomVector3 one
    {
        get
        {
            return oneVector;
        }
    }
    public static CustomVector3 forward
    {
        get
        {
            return forwardVector;
        }
    }
    public static CustomVector3 back
    {
        get
        {
            return backVector;
        }
    }
    public static CustomVector3 up
    {
        get
        {
            return upVector;
        }
    }
    public static CustomVector3 down
    {
        get
        {
            return downVector;
        }
    }
    public static CustomVector3 left
    {
        get
        {
            return leftVector;
        }
    }
    public static CustomVector3 right
    {
        get
        {
            return rightVector;
        }
    }
    public static CustomVector3 positiveInfinity
    {
        get
        {
            return positiveInfinityVector;
        }
    }
    public static CustomVector3 negativeInfinity
    {
        get
        {
            return negativeInfinityVector;
        }
    }
    public static CustomVector3 operator +(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(a.x + b.x, a.y + b.y, a.z + b.z);
    }

    public static CustomVector3 operator -(CustomVector3 a, CustomVector3 b)
    {
        return new CustomVector3(a.x - b.x, a.y - b.y, a.z - b.z);
    }

    public static CustomVector3 operator -(CustomVector3 a)
    {
        return new CustomVector3(-a.x, -a.y, -a.z);
    }

    public static CustomVector3 operator *(CustomVector3 a, float d)
    {
        return new CustomVector3(a.x * d, a.y * d, a.z * d);
    }

    public static CustomVector3 operator *(float d, CustomVector3 a)
    {
        return new CustomVector3(a.x * d, a.y * d, a.z * d);
    }

    public static CustomVector3 operator /(CustomVector3 a, float d)
    {
        return new CustomVector3(a.x / d, a.y / d, a.z / d);
    }

    public static bool operator ==(CustomVector3 lhs, CustomVector3 rhs)
    {
        return (SqrMagnitude(lhs - rhs) < 9.999999E-11f);
    }

    public static bool operator !=(CustomVector3 lhs, CustomVector3 rhs)
    {
        return !(lhs == rhs);
    }

    
    static CustomVector3()
    {
        zeroVector = new CustomVector3(0f, 0f, 0f);
        oneVector = new CustomVector3(1f, 1f, 1f);
        upVector = new CustomVector3(0f, 1f, 0f);
        downVector = new CustomVector3(0f, -1f, 0f);
        leftVector = new CustomVector3(-1f, 0f, 0f);
        rightVector = new CustomVector3(1f, 0f, 0f);
        forwardVector = new CustomVector3(0f, 0f, 1f);
        backVector = new CustomVector3(0f, 0f, -1f);
        positiveInfinityVector = new CustomVector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        negativeInfinityVector = new CustomVector3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
    }
}
