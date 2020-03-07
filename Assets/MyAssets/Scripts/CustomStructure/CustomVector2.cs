using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public struct CustomVector2
{
    public const float kEpsilon = 1E-05f;
    /// <summary>
    ///   <para>X component of the vector.</para>
    /// </summary>
    public float x;
    /// <summary>
    ///   <para>Y component of the vector.</para>
    /// </summary>
    public float y;

    public float this[int index]
    {
        get
        {
            switch (index)
            {
                case 0:
                    return this.x;
                case 1:
                    return this.y;
                default:
                    throw new IndexOutOfRangeException("Invalid CustomVector2 index!");
            }
        }
        set
        {
            switch (index)
            {
                case 0:
                    this.x = value;
                    break;
                case 1:
                    this.y = value;
                    break;
                default:
                    throw new IndexOutOfRangeException("Invalid CustomVector2 index!");
            }
        }
    }

    /// <summary>
    ///   <para>Returns this vector with a magnitude of 1 (Read Only).</para>
    /// </summary>
    public CustomVector2 normalized
    {
        get
        {
            CustomVector2 CustomVector2 = new CustomVector2(this.x, this.y);
            CustomVector2.Normalize();
            return CustomVector2;
        }
    }

    /// <summary>
    ///   <para>Returns the length of this vector (Read Only).</para>
    /// </summary>
    public float magnitude
    {
        get
        {
            return CustomMathf.Sqrt((float)((double)this.x * (double)this.x + (double)this.y * (double)this.y));
        }
    }

    /// <summary>
    ///   <para>Returns the squared length of this vector (Read Only).</para>
    /// </summary>
    public float sqrMagnitude
    {
        get
        {
            return (float)((double)this.x * (double)this.x + (double)this.y * (double)this.y);
        }
    }

    /// <summary>
    ///   <para>Shorthand for writing CustomVector2(0, 0).</para>
    /// </summary>
    public static CustomVector2 zero
    {
        get
        {
            return new CustomVector2(0.0f, 0.0f);
        }
    }

    /// <summary>
    ///   <para>Shorthand for writing CustomVector2(1, 1).</para>
    /// </summary>
    public static CustomVector2 one
    {
        get
        {
            return new CustomVector2(1f, 1f);
        }
    }

    /// <summary>
    ///   <para>Shorthand for writing CustomVector2(0, 1).</para>
    /// </summary>
    public static CustomVector2 up
    {
        get
        {
            return new CustomVector2(0.0f, 1f);
        }
    }

    /// <summary>
    ///   <para>Shorthand for writing CustomVector2(0, -1).</para>
    /// </summary>
    public static CustomVector2 down
    {
        get
        {
            return new CustomVector2(0.0f, -1f);
        }
    }

    /// <summary>
    ///   <para>Shorthand for writing CustomVector2(-1, 0).</para>
    /// </summary>
    public static CustomVector2 left
    {
        get
        {
            return new CustomVector2(-1f, 0.0f);
        }
    }

    /// <summary>
    ///   <para>Shorthand for writing CustomVector2(1, 0).</para>
    /// </summary>
    public static CustomVector2 right
    {
        get
        {
            return new CustomVector2(1f, 0.0f);
        }
    }

    /// <summary>
    ///   <para>Constructs a new vector with given x, y components.</para>
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public CustomVector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static implicit operator CustomVector2(CustomVector3 v)
    {
        return new CustomVector2(v.x, v.y);
    }

    public static implicit operator CustomVector3(CustomVector2 v)
    {
        return new CustomVector3(v.x, v.y, 0.0f);
    }

    public static CustomVector2 operator +(CustomVector2 a, CustomVector2 b)
    {
        return new CustomVector2(a.x + b.x, a.y + b.y);
    }

    public static CustomVector2 operator -(CustomVector2 a, CustomVector2 b)
    {
        return new CustomVector2(a.x - b.x, a.y - b.y);
    }

    public static CustomVector2 operator -(CustomVector2 a)
    {
        return new CustomVector2(-a.x, -a.y);
    }

    public static CustomVector2 operator *(CustomVector2 a, float d)
    {
        return new CustomVector2(a.x * d, a.y * d);
    }

    public static CustomVector2 operator *(float d, CustomVector2 a)
    {
        return new CustomVector2(a.x * d, a.y * d);
    }

    public static CustomVector2 operator /(CustomVector2 a, float d)
    {
        return new CustomVector2(a.x / d, a.y / d);
    }

    public static bool operator ==(CustomVector2 lhs, CustomVector2 rhs)
    {
        return (double)CustomVector2.SqrMagnitude(lhs - rhs) < 9.99999943962493E-11;
    }

    public static bool operator !=(CustomVector2 lhs, CustomVector2 rhs)
    {
        return (double)CustomVector2.SqrMagnitude(lhs - rhs) >= 9.99999943962493E-11;
    }

    /// <summary>
    ///   <para>Set x and y components of an existing CustomVector2.</para>
    /// </summary>
    /// <param name="new_x"></param>
    /// <param name="new_y"></param>
    public void Set(float new_x, float new_y)
    {
        this.x = new_x;
        this.y = new_y;
    }

    /// <summary>
    ///   <para>Linearly interpolates between vectors a and b by t.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static CustomVector2 Lerp(CustomVector2 a, CustomVector2 b, float t)
    {
        t = CustomMathf.Clamp01(t);
        return new CustomVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
    }

    /// <summary>
    ///   <para>Linearly interpolates between vectors a and b by t.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="t"></param>
    public static CustomVector2 LerpUnclamped(CustomVector2 a, CustomVector2 b, float t)
    {
        return new CustomVector2(a.x + (b.x - a.x) * t, a.y + (b.y - a.y) * t);
    }

    /// <summary>
    ///   <para>Moves a point current towards target.</para>
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    /// <param name="maxDistanceDelta"></param>
    public static CustomVector2 MoveTowards(CustomVector2 current, CustomVector2 target, float maxDistanceDelta)
    {
        CustomVector2 CustomVector2 = target - current;
        float magnitude = CustomVector2.magnitude;
        if ((double)magnitude <= (double)maxDistanceDelta || (double)magnitude == 0.0)
            return target;
        return current + CustomVector2 / magnitude * maxDistanceDelta;
    }

    /// <summary>
    ///   <para>Multiplies two vectors component-wise.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static CustomVector2 Scale(CustomVector2 a, CustomVector2 b)
    {
        return new CustomVector2(a.x * b.x, a.y * b.y);
    }

    /// <summary>
    ///   <para>Multiplies every component of this vector by the same component of scale.</para>
    /// </summary>
    /// <param name="scale"></param>
    public void Scale(CustomVector2 scale)
    {
        this.x *= scale.x;
        this.y *= scale.y;
    }

    /// <summary>
    ///   <para>Makes this vector have a magnitude of 1.</para>
    /// </summary>
    public void Normalize()
    {
        float magnitude = this.magnitude;
        if ((double)magnitude > 9.99999974737875E-06)
            this = (CustomVector2)(this / magnitude);
        else
            this = CustomVector2.zero;
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public override string ToString()
    {
        return string.Format("({0:F1}, {1:F1})", (object)this.x, (object)this.y);
    }

    /// <summary>
    ///   <para>Returns a nicely formatted string for this vector.</para>
    /// </summary>
    /// <param name="format"></param>
    public string ToString(string format)
    {
        return string.Format("({0}, {1})", (object)this.x.ToString(format), (object)this.y.ToString(format));
    }

    public override int GetHashCode()
    {
        return this.x.GetHashCode() ^ this.y.GetHashCode() << 2;
    }

    public override bool Equals(object other)
    {
        if (!(other is CustomVector2))
            return false;
        CustomVector2 CustomVector2 = (CustomVector2)other;
        if (this.x.Equals(CustomVector2.x))
            return this.y.Equals(CustomVector2.y);
        return false;
    }

    /// <summary>
    ///   <para>Reflects a vector off the vector defined by a normal.</para>
    /// </summary>
    /// <param name="inDirection"></param>
    /// <param name="inNormal"></param>
    public static CustomVector2 Reflect(CustomVector2 inDirection, CustomVector2 inNormal)
    {
        return -2f * CustomVector2.Dot(inNormal, inDirection) * inNormal + inDirection;
    }

    /// <summary>
    ///   <para>Dot Product of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static float Dot(CustomVector2 lhs, CustomVector2 rhs)
    {
        return (float)((double)lhs.x * (double)rhs.x + (double)lhs.y * (double)rhs.y);
    }

    /// <summary>
    ///   <para>Returns the angle in degrees between from and to.</para>
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public static float Angle(CustomVector2 from, CustomVector2 to)
    {
        return CustomMathf.Acos(CustomMathf.Clamp(CustomVector2.Dot(from.normalized, to.normalized), -1f, 1f)) * 57.29578f;
    }

    /// <summary>
    ///   <para>Returns the distance between a and b.</para>
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    public static float Distance(CustomVector2 a, CustomVector2 b)
    {
        return (a - b).magnitude;
    }

    /// <summary>
    ///   <para>Returns a copy of vector with its magnitude clamped to maxLength.</para>
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="maxLength"></param>
    public static CustomVector2 ClampMagnitude(CustomVector2 vector, float maxLength)
    {
        if ((double)vector.sqrMagnitude > (double)maxLength * (double)maxLength)
            return vector.normalized * maxLength;
        return vector;
    }

    public static float SqrMagnitude(CustomVector2 a)
    {
        return (float)((double)a.x * (double)a.x + (double)a.y * (double)a.y);
    }

    public float SqrMagnitude()
    {
        return (float)((double)this.x * (double)this.x + (double)this.y * (double)this.y);
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the smallest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static CustomVector2 Min(CustomVector2 lhs, CustomVector2 rhs)
    {
        return new CustomVector2(CustomMathf.Min(lhs.x, rhs.x), CustomMathf.Min(lhs.y, rhs.y));
    }

    /// <summary>
    ///   <para>Returns a vector that is made from the largest components of two vectors.</para>
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    public static CustomVector2 Max(CustomVector2 lhs, CustomVector2 rhs)
    {
        return new CustomVector2(CustomMathf.Max(lhs.x, rhs.x), CustomMathf.Max(lhs.y, rhs.y));
    }


    public static CustomVector2 SmoothDamp(CustomVector2 current, CustomVector2 target, ref CustomVector2 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
    {
        smoothTime = CustomMathf.Max(0.0001f, smoothTime);
        float num1 = 2f / smoothTime;
        float num2 = num1 * deltaTime;
        float num3 = (float)(1.0 / (1.0 + (double)num2 + 0.479999989271164 * (double)num2 * (double)num2 + 0.234999999403954 * (double)num2 * (double)num2 * (double)num2));
        CustomVector2 vector = current - target;
        CustomVector2 CustomVector2_1 = target;
        float maxLength = maxSpeed * smoothTime;
        CustomVector2 CustomVector2_2 = CustomVector2.ClampMagnitude(vector, maxLength);
        target = current - CustomVector2_2;
        CustomVector2 CustomVector2_3 = (currentVelocity + num1 * CustomVector2_2) * deltaTime;
        currentVelocity = (currentVelocity - num1 * CustomVector2_3) * num3;
        CustomVector2 CustomVector2_4 = target + (CustomVector2_2 + CustomVector2_3) * num3;
        if ((double)CustomVector2.Dot(CustomVector2_1 - current, CustomVector2_4 - CustomVector2_1) > 0.0)
        {
            CustomVector2_4 = CustomVector2_1;
            currentVelocity = (CustomVector2_4 - CustomVector2_1) / deltaTime;
        }
        return CustomVector2_4;
    }
}