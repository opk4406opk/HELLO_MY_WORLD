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
    // in world space coordinate
    public Vector3 centerPos { get; private set; }
    public bool isEnable { set; get; }

    public Vector3 hitPointWithRay;

    public void SettingSpeed(Vector3 speed)
    {
        vx = speed.x;
        vy = speed.y;
        vz = speed.z;
    }

    public void Repositioning(Vector3 vec)
    {
        minExtent += vec;
        maxExtent += vec;
        centerPos = (maxExtent + minExtent) / 2;
    }

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

    public void MakeAABB(Vector3[] points, float velocityX = 0.0f, float velocityY = 0.0f, float velocityZ = 0.0f)
    {
        vx = velocityX;
        vy = velocityY;
        vz = velocityZ;
        width = 0.5f;
        height = 0.5f;
        depth = 0.5f;

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
    public void MakeAABB(Vector3 minExtent, Vector3 maxExtent, float velocityX = 0.0f, float velocityY = 0.0f, float velocityZ = 0.0f)
    {
        vx = velocityX;
        vy = velocityY;
        vz = velocityZ;
        width = 0.5f;
        height = 0.5f;
        depth = 0.5f;

        this.minExtent = minExtent;
        this.maxExtent = maxExtent;
        centerPos = (this.maxExtent + this.minExtent) / 2;
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
        width = 0.5f;
        height = 0.5f;
        depth = 0.5f;

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

    /// <summary>
    /// https://www.gamedev.net/articles/programming/general-and-gameplay-programming/swept-aabb-collision-detection-and-response-r3084/
    /// </summary>
    /// <param name="move"></param>
    /// <param name="fix"></param>
    /// <param name="normalFaceX"></param>
    /// <param name="normalFaceY"></param>
    /// <param name="normalFaceZ"></param>
    /// <returns> 0.0 ~ 1.0f</returns>
    public static float SweptAABB(CustomAABB move, CustomAABB fix, ref float normalFaceX, ref float normalFaceY, ref float normalFaceZ)
    {
        float xInvEntry, yInvEntry, zInvEntry;
        float xInvExit, yInvExit, zInvExit;

        // find the distance between the objects on the near and far sides for both x and y
        if (move.vx > 0.0f)
        {
            xInvEntry = fix.centerPos.x - (move.centerPos.x + move.width);
            xInvExit = (fix.centerPos.x + fix.width) - move.centerPos.x;
        }
        else
        {
            xInvEntry = (fix.centerPos.x + fix.width) - move.centerPos.x;
            xInvExit = fix.centerPos.x - (move.centerPos.x + move.width);
        }

        if (move.vy > 0.0f)
        {
            yInvEntry = fix.centerPos.y - (move.centerPos.y + move.height);
            yInvExit = (fix.centerPos.y + fix.height) - move.centerPos.y;
        }
        else
        {
            yInvEntry = (fix.centerPos.y + fix.height) - move.centerPos.y;
            yInvExit = fix.centerPos.y - (move.centerPos.y + move.height);
        }

        if (move.vz > 0.0f)
        {
            zInvEntry = fix.centerPos.z - (move.centerPos.z + move.depth);
            zInvExit = (fix.centerPos.z + fix.depth) - move.centerPos.z;
        }
        else
        {
            zInvEntry = (fix.centerPos.z + fix.depth) - move.centerPos.z;
            zInvExit = fix.centerPos.z - (move.centerPos.z + move.depth);
        }

        //
        float xEntry, yEntry, zEntry;
        float xExit, yExit, zExit;

        if (move.vx == 0.0f)
        {
            xEntry = -float.PositiveInfinity;
            xExit = float.PositiveInfinity;
        }
        else
        {
            xEntry = xInvEntry / move.vx;
            xExit = xInvExit / move.vx;
        }

        if (move.vy == 0.0f)
        {
            yEntry = -float.PositiveInfinity;
            yExit = float.PositiveInfinity;
        }
        else
        {
            yEntry = yInvEntry / move.vy;
            yExit = yInvExit / move.vy;
        }

        if (move.vz == 0.0f)
        {
            zEntry = -float.PositiveInfinity;
            zExit = float.PositiveInfinity;
        }
        else
        {
            zEntry = zInvEntry / move.vz;
            zExit = zInvExit / move.vz;
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
