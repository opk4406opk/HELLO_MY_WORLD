using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor와 관련된 행동트리 노드들이 정의되어 있는 파일. 
 */

public class MoveForTarget : Node
{
    private readonly float dist = 0.12f;
    private Stack<PathNode2D> path;
    private Vector2 src, dest;
    private PathNode2D p = null;

    public CustomAstar2D pathFinder { get; } = new CustomAstar2D();

    public override bool Invoke()
    {
        if (path.Count > 0)
        {
            src.x = Controller.GetActorTransform().position.x;
            src.y = Controller.GetActorTransform().position.z;
            dest.x = p.WorldCoordX;
            dest.y = p.WorldCoordZ;
            Vector2 dir = dest - src;
            if (Vector2.Distance(src, dest) <= dist)
            {
                p = path.Pop();
                Controller.LookAt(new Vector3(dir.x, 0.0f, dir.y));
            }
            else Controller.Move(new Vector3(dir.x, 0.0f, dir.y), 1.5f);
            return false;
        }
        else return true;
    }
    public void InitPathFinder(PathFinderInitData data)
    {
        pathFinder.Init(data);
    }
    public void PathFinding()
    {
        path = pathFinder.PathFinding();
        p = path.Pop();
    }

}

public class StartAttack : Node
{
    public override bool Invoke()
    {
        return true;
    }
}
public class StopAttack : Node
{
    public override bool Invoke()
    {
        return true;
    }
}

public class DeadProcess : Node
{
    public override bool Invoke()
    {
        return true;
    }
}

public class CheckDead : Node
{
    public override bool Invoke()
    {
        return false;
    }
}
