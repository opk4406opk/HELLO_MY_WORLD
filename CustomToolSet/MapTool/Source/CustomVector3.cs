using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapTool.Source
{
    class CustomVector3
    {
        public float x, y, z;
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
        public static float Dot(CustomVector3 lhs, CustomVector3 rhs)
        {
            return (((lhs.x * rhs.x) + (lhs.y * rhs.y)) + (lhs.z * rhs.z));
        }
        public static CustomVector3 Cross(CustomVector3 lhs, CustomVector3 rhs)
        {
            return new CustomVector3((lhs.y * rhs.z) - (lhs.z * rhs.y), (lhs.z * rhs.x) - (lhs.x * rhs.z), (lhs.x * rhs.y) - (lhs.y * rhs.x));
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

        public static float SqrMagnitude(CustomVector3 vector)
        {
            return (((vector.x * vector.x) + (vector.y * vector.y)) + (vector.z * vector.z));
        }
    }
}
