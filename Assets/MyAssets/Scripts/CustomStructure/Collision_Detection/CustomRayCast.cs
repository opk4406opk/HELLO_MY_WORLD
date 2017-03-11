using UnityEngine;
using System.Collections;

// ref #1 : http://www.scratchapixel.com/lessons/3d-basic-rendering/minimal-ray-tracer-rendering-simple-shapes/ray-box-intersection
// ref #2 : http://www.opengl-tutorial.org/miscellaneous/clicking-on-objects/picking-with-custom-ray-obb-function/
public class CustomRayCast : MonoBehaviour
{
    /// <summary>
    /// min, maxExtent를 이용한 광선과의 충돌 판정 메소드.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="obb"></param>
    /// <returns></returns>
    public static bool InterSectWithBOX(Ray ray, Vector3 minExtent, Vector3 maxExtent)
    {
        Vector3 dirfrac;
        float t;
        // r.direction is unit direction vector of ray
        dirfrac.x = 1.0f / ray.direction.x;
        dirfrac.y = 1.0f / ray.direction.y;
        dirfrac.z = 1.0f / ray.direction.z;
        // aabb.minExtent is the corner of AABB with minimal coordinates - left bottom, aabb.maxExtent is maximal corner
        // r.origin is origin of ray
        float t1 = (minExtent.x - ray.origin.x) * dirfrac.x;
        float t2 = (maxExtent.x - ray.origin.x) * dirfrac.x;
        float t3 = (minExtent.y - ray.origin.y) * dirfrac.y;
        float t4 = (maxExtent.y - ray.origin.y) * dirfrac.y;
        float t5 = (minExtent.z - ray.origin.z) * dirfrac.z;
        float t6 = (maxExtent.z - ray.origin.z) * dirfrac.z;

        float tmin = Mathf.Max(Mathf.Max(Mathf.Min(t1, t2), Mathf.Min(t3, t4)), Mathf.Min(t5, t6));
        float tmax = Mathf.Min(Mathf.Min(Mathf.Max(t1, t2), Mathf.Max(t3, t4)), Mathf.Max(t5, t6));

        // if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
        if (tmax < 0)
        {
            t = tmax;
            return false;
        }

        // if tmin > tmax, ray doesn't intersect AABB
        if (tmin > tmax)
        {
            t = tmax;
            return false;
        }

        t = tmin;
        return true;
    }

    /// <summary>
    /// AABB 경계상자와 광선과의 충돌 판정 메소드.
    /// </summary>
    /// <param name="ray"></param>
    /// <param name="aabb"></param>
    /// <returns></returns>
    public static bool InterSectWithAABB(Ray ray, CustomAABB aabb)
    {
        float tmin = (aabb.minExtent.x - ray.origin.x) / ray.direction.normalized.x;
        float tmax = (aabb.maxExtent.x - ray.origin.x) / ray.direction.normalized.x;

        if (tmin > tmax)
        {
            float tmp = tmin;
            tmin = tmax;
            tmax = tmp;
        }

        float tymin = (aabb.minExtent.y - ray.origin.y) / ray.direction.normalized.y;
        float tymax = (aabb.maxExtent.y - ray.origin.y) / ray.direction.normalized.y;

        if (tymin > tymax)
        {
            float tmp = tymin;
            tymin = tymax;
            tymax = tmp;
        }

        if ((tmin > tymax) || (tymin > tmax))
            return false;

        if (tymin > tmin)
            tmin = tymin;

        if (tymax < tmax)
            tmax = tymax;

        float tzmin = (aabb.minExtent.z - ray.origin.z) / ray.direction.normalized.z;
        float tzmax = (aabb.maxExtent.z - ray.origin.z) / ray.direction.normalized.z;

        if (tzmin > tzmax)
        {
            float tmp = tzmin;
            tzmin = tzmax;
            tzmax = tmp;
        }

        if ((tmin > tzmax) || (tzmin > tmax))
            return false;
        
        if (tzmin > tmin)
            tmin = tzmin;

        if (tzmax < tmax)
            tmax = tzmax;

        return true;
    }

    /// <summary>
    /// 테스트버전.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="aabb"></param>
    /// <returns></returns>
    public static bool InterSectWithAABB_TEST_Version(Ray r, CustomAABB aabb)
    {
        if (!aabb.isEnable) return false;

        Vector3 dirfrac;
        float t;
        // r.direction is unit direction vector of ray
        dirfrac.x = 1.0f / r.direction.x;
        dirfrac.y = 1.0f / r.direction.y;
        dirfrac.z = 1.0f / r.direction.z;
        // aabb.minExtent is the corner of AABB with minimal coordinates - left bottom, aabb.maxExtent is maximal corner
        // r.origin is origin of ray
        float t1 = (aabb.minExtent.x - r.origin.x) * dirfrac.x;
        float t2 = (aabb.maxExtent.x - r.origin.x) * dirfrac.x;
        float t3 = (aabb.minExtent.y - r.origin.y) * dirfrac.y;
        float t4 = (aabb.maxExtent.y - r.origin.y) * dirfrac.y;
        float t5 = (aabb.minExtent.z - r.origin.z) * dirfrac.z;
        float t6 = (aabb.maxExtent.z - r.origin.z) * dirfrac.z;

        float tmin = Mathf.Max(Mathf.Max(Mathf.Min(t1, t2), Mathf.Min(t3, t4)), Mathf.Min(t5, t6));
        float tmax = Mathf.Min(Mathf.Min(Mathf.Max(t1, t2), Mathf.Max(t3, t4)), Mathf.Max(t5, t6));

        // if tmax < 0, ray (line) is intersecting AABB, but whole AABB is behing us
        if (tmax < 0)
        {
            t = tmax;
            return false;
        }

        // if tmin > tmax, ray doesn't intersect AABB
        if (tmin > tmax)
        {
            t = tmax;
            return false;
        }

        t = tmin;
        return true;
    }
}
