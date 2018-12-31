using UnityEngine;
using System.Collections;

public struct CustomAABB {
    public float vx;
    public float vy;
    public float vz;
    public float width;
    public float height;
    public float depth;

    public Vector3 minExtent;
    public Vector3 maxExtent;
    /// <summary>
    ///  in world space coordinate
    /// </summary>
    public Vector3 centerPos;
    public bool isEnable { set; get; }

    public Vector3 hitPointWithRay;

    public void MakeAABB(CustomAABB other)
    {
        minExtent = other.minExtent;
        maxExtent = other.maxExtent;
        vx = other.vx;
        vy = other.vy;
        vz = other.vz;
        width = other.width;
        height = other.height;
        depth = other.depth;
    }

    public void MakeAABB(Vector3 vec)
    {
        minExtent += vec;
        maxExtent += vec;
        centerPos = (maxExtent + minExtent) / 2;
        Vector3 diff = (maxExtent - minExtent) / 2;
        width = diff.x;
        height = diff.y;
        depth = diff.z;
    }

    public void MakeAABB(Vector3[] points, float velocityX = 0.0f, float velocityY = 0.0f, float velocityZ = 0.0f)
    {
        vx = velocityX;
        vy = velocityY;
        vz = velocityZ;

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
        Vector3 diff = (maxExtent - minExtent) / 2;
        width = diff.x;
        height = diff.y;
        depth = diff.z;
    }
    public void MakeAABB(Vector3 minExtent, Vector3 maxExtent, float velocityX = 0.0f, float velocityY = 0.0f, float velocityZ = 0.0f)
    {
        vx = velocityX;
        vy = velocityY;
        vz = velocityZ;

        this.minExtent = minExtent;
        this.maxExtent = maxExtent;
        centerPos = (this.maxExtent + this.minExtent) / 2;
        Vector3 diff = (maxExtent - minExtent) /2;
        width = diff.x;
        height = diff.y;
        depth = diff.z;
    }

    /// <summary>
    /// 유니티엔진에서 제공하는 BoxCollider를 이용해 CustomAABB를 생성.
    /// </summary>
    /// <param name="boxColl"></param>
    public void MakeAABB(BoxCollider boxColl, float velocityX = 0.0f, float velocityY = 0.0f, float velocityZ = 0.0f)
    {
        vx = velocityX;
        vy = velocityY;
        vz = velocityZ;
       
        minExtent = boxColl.bounds.min;
        maxExtent = boxColl.bounds.max;
        centerPos = (maxExtent + minExtent) / 2;
        Vector3 diff = (maxExtent - minExtent) / 2;
        width = diff.x;
        height = diff.y;
        depth = diff.z;
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


    public static CustomAABB GetSweptBroadphaseBox(CustomAABB b)
    {
        CustomAABB broadphasebox = new CustomAABB();
        broadphasebox.MakeAABB(b);
        broadphasebox.centerPos.x = b.vx > 0 ? b.centerPos.x : b.centerPos.x + b.vx;
        broadphasebox.centerPos.y = b.vy > 0 ? b.centerPos.y : b.centerPos.y + b.vy;
        broadphasebox.centerPos.y = b.vz > 0 ? b.centerPos.z : b.centerPos.z + b.vz;
        //
        broadphasebox.width = b.vx > 0 ? b.vx + b.width : b.width - b.vx;
        broadphasebox.height = b.vy > 0 ? b.vy + b.height : b.height - b.vy;
        broadphasebox.depth = b.vz > 0 ? b.vz + b.depth : b.depth - b.vz;

        return broadphasebox;
    }
    /// <summary>
    /// https://www.gamedev.net/articles/programming/general-and-gameplay-programming/swept-aabb-collision-detection-and-response-r3084/
    /// </summary>
    /// <param name="moveAABB"></param>
    /// <param name="staticAABB"></param>
    /// <param name="normalFaceX"></param>
    /// <param name="normalFaceY"></param>
    /// <param name="normalFaceZ"></param>
    /// <returns> 0.0 ~ 1.0f</returns>
    public static float SweptAABB(CustomAABB moveAABB, CustomAABB staticAABB, ref float normalFaceX, ref float normalFaceY, ref float normalFaceZ)
    {
        float xInvEntry, yInvEntry, zInvEntry;
        float xInvExit, yInvExit, zInvExit;

        // find the distance between the objects on the near and far sides for both x and y
        if (moveAABB.vx > 0.0f)
        {
            xInvEntry = staticAABB.maxExtent.x - (moveAABB.maxExtent.x + moveAABB.width);
            xInvExit = (staticAABB.maxExtent.x + staticAABB.width) - moveAABB.maxExtent.x;
        }
        else
        {
            xInvEntry = (staticAABB.maxExtent.x + staticAABB.width) - moveAABB.maxExtent.x;
            xInvExit = staticAABB.maxExtent.x - (moveAABB.maxExtent.x + moveAABB.width);
        }

        if (moveAABB.vy > 0.0f)
        {
            yInvEntry = staticAABB.maxExtent.y - (moveAABB.maxExtent.y + moveAABB.height);
            yInvExit = (staticAABB.maxExtent.y + staticAABB.height) - moveAABB.maxExtent.y;
        }
        else
        {
            yInvEntry = (staticAABB.maxExtent.y + staticAABB.height) - moveAABB.maxExtent.y;
            yInvExit = staticAABB.maxExtent.y - (moveAABB.maxExtent.y + moveAABB.height);
        }

        if (moveAABB.vz > 0.0f)
        {
            zInvEntry = staticAABB.maxExtent.z - (moveAABB.maxExtent.z + moveAABB.depth);
            zInvExit = (staticAABB.maxExtent.z + staticAABB.depth) - moveAABB.maxExtent.z;
        }
        else
        {
            zInvEntry = (staticAABB.maxExtent.z + staticAABB.depth) - moveAABB.maxExtent.z;
            zInvExit = staticAABB.maxExtent.z - (moveAABB.maxExtent.z + moveAABB.depth);
        }

        //
        float xEntry, yEntry, zEntry;
        float xExit, yExit, zExit;

        if (moveAABB.vx == 0.0f)
        {
            xEntry = -float.PositiveInfinity;
            xExit = float.PositiveInfinity;
        }
        else
        {
            xEntry = xInvEntry / moveAABB.vx;
            xExit = xInvExit / moveAABB.vx;
        }

        if (moveAABB.vy == 0.0f)
        {
            yEntry = -float.PositiveInfinity;
            yExit = float.PositiveInfinity;
        }
        else
        {
            yEntry = yInvEntry / moveAABB.vy;
            yExit = yInvExit / moveAABB.vy;
        }

        if (moveAABB.vz == 0.0f)
        {
            zEntry = -float.PositiveInfinity;
            zExit = float.PositiveInfinity;
        }
        else
        {
            zEntry = zInvEntry / moveAABB.vz;
            zExit = zInvExit / moveAABB.vz;
        }

        //
        // find the earliest/latest times of collision
        float[] entryCandidates = new float[3];
        entryCandidates[0] = xEntry;
        entryCandidates[1] = yEntry;
        entryCandidates[2] = zEntry;
        float[] exitCandidates = new float[3];
        exitCandidates[0] = xExit;
        exitCandidates[1] = yExit;
        exitCandidates[2] = zExit;

        float entryTime = Mathf.Max(entryCandidates);
        float exitTime = Mathf.Min(exitCandidates);

        // if there was no collision
        if (entryTime > exitTime ||
            xEntry < 0.0f && yEntry < 0.0f && zEntry < 0.0f ||
            xEntry > 1.0f || yEntry > 1.0f || zEntry > 1.0f)
        {
            normalFaceX = 0.0f;
            normalFaceY = 0.0f;
            normalFaceZ = 0.0f;
            return 1.0f;
        }
        else // if there was a collision
        {
            // calculate normal of collided surface
            if (xEntry > yEntry)
            {
                if (xInvEntry < 0.0f)
                {
                    normalFaceX = 1.0f;
                    normalFaceY = 0.0f;
                    normalFaceZ = 0.0f;
                }
                else
                {
                    normalFaceX = -1.0f;
                    normalFaceY = 0.0f;
                    normalFaceZ = 0.0f;
                }
            }
            else if(xEntry < yEntry)
            {
                if (yInvEntry < 0.0f)
                {
                    normalFaceX = 0.0f;
                    normalFaceY = 1.0f;
                    normalFaceZ = 0.0f;
                }
                else
                {
                    normalFaceX = 0.0f;
                    normalFaceY = -1.0f;
                    normalFaceZ = 0.0f;
                }
            }
            else
            {
                if (zInvEntry < 0.0f)
                {
                    normalFaceX = 0.0f;
                    normalFaceY = 0.0f;
                    normalFaceZ = 1.0f;
                }
                else
                {
                    normalFaceX = 0.0f;
                    normalFaceY = 0.0f;
                    normalFaceZ = -1.0f;
                }
            }

            // return the time of collision
            return entryTime;
        }

    }
}
