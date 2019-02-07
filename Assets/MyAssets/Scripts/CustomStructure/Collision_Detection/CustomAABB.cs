using UnityEngine;
using System.Collections;

public struct CustomAABB {
    public float vx;
    public float vy;
    public float vz;
    public float Width;
    public float Height;
    public float Depth;

    public Vector3 MinExtent;
    public Vector3 MaxExtent;
    /// <summary>
    ///  in world space coordinate
    /// </summary>
    public Vector3 Position;
    public bool IsEnable { set; get; }

    public Vector3 HitPointWithRay;

    public void MakeAABB(CustomAABB other)
    {
        MinExtent = other.MinExtent;
        MaxExtent = other.MaxExtent;
        vx = other.vx;
        vy = other.vy;
        vz = other.vz;
        Width = other.Width;
        Height = other.Height;
        Depth = other.Depth;
    }

    public void MakeAABB(Vector3 vec)
    {
        MinExtent += vec;
        MaxExtent += vec;
        Position = (MaxExtent + MinExtent) / 2;
        Vector3 diff = MaxExtent - MinExtent;
        Width = diff.x;
        Height = diff.y;
        Depth = diff.z;
    }

    public void MakeAABB(Vector3[] points, float velocityX = 0.0f, float velocityY = 0.0f, float velocityZ = 0.0f)
    {
        vx = velocityX;
        vy = velocityY;
        vz = velocityZ;

        MinExtent = points[0];
        MaxExtent = points[0];
        for(int idx = 1; idx < points.Length; idx++)
        {
            if (points[idx].x < MinExtent.x) MinExtent.x = points[idx].x;
            else if (points[idx].x > MaxExtent.x) MaxExtent.x = points[idx].x;
            if (points[idx].y < MinExtent.y) MinExtent.y = points[idx].y;
            else if (points[idx].y > MaxExtent.y) MaxExtent.y = points[idx].y;
            if (points[idx].z < MinExtent.z) MinExtent.z = points[idx].z;
            else if (points[idx].z > MaxExtent.z) MaxExtent.z = points[idx].z;
        }

        Position = (MaxExtent + MinExtent) / 2;
        Vector3 diff = MaxExtent - MinExtent;
        Width = diff.x;
        Height = diff.y;
        Depth = diff.z;
    }
    public void MakeAABB(Vector3 minExtent, Vector3 maxExtent, float velocityX = 0.0f, float velocityY = 0.0f, float velocityZ = 0.0f)
    {
        vx = velocityX;
        vy = velocityY;
        vz = velocityZ;

        this.MinExtent = minExtent;
        this.MaxExtent = maxExtent;
        Position = (this.MaxExtent + this.MinExtent) / 2;
        Vector3 diff = maxExtent - minExtent;
        Width = diff.x;
        Height = diff.y;
        Depth = diff.z;
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
       
        MinExtent = boxColl.bounds.min;
        MaxExtent = boxColl.bounds.max;
        Position = (MaxExtent + MinExtent) / 2;
        Vector3 diff = MaxExtent - MinExtent;
        Width = diff.x;
        Height = diff.y;
        Depth = diff.z;
    }

    public bool IsInterSectPoint(Vector3 point)
    {
        if ((point.x > MinExtent.x && point.x < MaxExtent.x ) &&
           (point.y > MinExtent.y && point.y < MaxExtent.y) &&
           (point.z > MinExtent.z && point.z < MaxExtent.z))
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
        if ((MinExtent.x <= other.MaxExtent.x && MaxExtent.x >= other.MinExtent.x) &&
         (MinExtent.y <= other.MaxExtent.y && MaxExtent.y >= other.MinExtent.y) &&
         (MinExtent.z <= other.MaxExtent.z && MaxExtent.z >= other.MinExtent.z))
        {
            return true;
        }
        return false;
    }

    bool GetCollideFaceNormal(CustomAABB other, ref Vector3 normal)
    {
        Vector3[] faces =
        {
            new Vector3(-1, 0, 0), // 'left' face normal (-x direction)
            new Vector3(1, 0, 0), // 'right' face normal (+x direction)
            new Vector3(0, -1, 0), // 'bottom' face normal (-y direction)
            new Vector3(0, 1, 0), // 'top' face normal (+y direction)
            new Vector3(0, 0, -1), // 'far' face normal (-z direction)
            new Vector3(0, 0, 1) // 'near' face normal (+z direction)
        };
        float[] distances =
        {
            (Position.x - other.Position.x), 
            (other.Position.x - Position.x), 
            (Position.y - other.Position.y),
            (other.Position.y - Position.y),
            (Position.z - other.Position.z), 
            (other.Position.z - Position.z), 
        };
        return false;
    }


    public static CustomAABB GetSweptBroadphaseBox(CustomAABB b)
    {
        CustomAABB broadphasebox = new CustomAABB();
        broadphasebox.MakeAABB(b);
        broadphasebox.MinExtent.x = b.vx > 0 ? b.MinExtent.x : b.MinExtent.x + b.vx;
        broadphasebox.MinExtent.y = b.vy > 0 ? b.MinExtent.y : b.MinExtent.y + b.vy;
        broadphasebox.MinExtent.y = b.vz > 0 ? b.MinExtent.z : b.MinExtent.z + b.vz;
        //
        broadphasebox.Width = b.vx > 0 ? b.vx + b.Width : b.Width - b.vx;
        broadphasebox.Height = b.vy > 0 ? b.vy + b.Height : b.Height - b.vy;
        broadphasebox.Depth = b.vz > 0 ? b.vz + b.Depth : b.Depth - b.vz;

        return broadphasebox;
    }


    public static bool BroadphaseAABBCheck(CustomAABB broadphase, CustomAABB b2)
    {
        return !(broadphase.MinExtent.x + broadphase.Width < b2.MinExtent.x ||
            broadphase.MinExtent.x > b2.MinExtent.x + b2.Width ||
            broadphase.MinExtent.y + broadphase.Height < b2.MinExtent.y ||
            broadphase.MinExtent.y > b2.MinExtent.y + b2.Height ||
            broadphase.MinExtent.z > b2.MinExtent.z + b2.Depth ||
            broadphase.MinExtent.z + broadphase.Depth < b2.MinExtent.z);
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
            xInvEntry = staticAABB.Position.x - (moveAABB.Position.x + moveAABB.Width);
            xInvExit = (staticAABB.Position.x + staticAABB.Width) - moveAABB.Position.x;
        }
        else
        {
            xInvEntry = (staticAABB.Position.x + staticAABB.Width) - moveAABB.Position.x;
            xInvExit = staticAABB.Position.x - (moveAABB.Position.x + moveAABB.Width);
        }

        if (moveAABB.vy > 0.0f)
        {
            yInvEntry = staticAABB.Position.y - (moveAABB.Position.y + moveAABB.Height);
            yInvExit = (staticAABB.Position.y + staticAABB.Height) - moveAABB.Position.y;
        }
        else
        {
            yInvEntry = (staticAABB.Position.y + staticAABB.Height) - moveAABB.Position.y;
            yInvExit = staticAABB.Position.y - (moveAABB.Position.y + moveAABB.Height);
        }

        if (moveAABB.vz > 0.0f)
        {
            zInvEntry = staticAABB.Position.z - (moveAABB.Position.z + moveAABB.Depth);
            zInvExit = (staticAABB.Position.z + staticAABB.Depth) - moveAABB.Position.z;
        }
        else
        {
            zInvEntry = (staticAABB.Position.z + staticAABB.Depth) - moveAABB.Position.z;
            zInvExit = staticAABB.Position.z - (moveAABB.Position.z + moveAABB.Depth);
        }

        //
        float xEntry, yEntry, zEntry;
        float xExit, yExit, zExit;

        if (moveAABB.vx == 0.0f)
        {
            xEntry = float.NegativeInfinity;
            xExit = float.PositiveInfinity;
        }
        else
        {
            xEntry = xInvEntry / moveAABB.vx;
            xExit = xInvExit / moveAABB.vx;
        }

        if (moveAABB.vy == 0.0f)
        {
            yEntry = float.NegativeInfinity;
            yExit = float.PositiveInfinity;
        }
        else
        {
            yEntry = yInvEntry / moveAABB.vy;
            yExit = yInvExit / moveAABB.vy;
        }

        if (moveAABB.vz == 0.0f)
        {
            zEntry = float.NegativeInfinity;
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
