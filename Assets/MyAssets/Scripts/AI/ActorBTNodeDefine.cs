using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor와 관련된 행동트리 노드들이 정의되어 있는 파일. 
 */

public class MoveForTarget : Node
{
    private readonly float dist = 0.12f;
    private Stack<PathNode> path;
    private Vector2 src, dest;
    private PathNode p = null;

    public CustomAstar pathFinder { get; } = new CustomAstar();

    public ActorController controller
    {
        set { _controller = value; }
    }
    private ActorController _controller;
    public override bool Invoke()
    {
        if (path.Count > 0)
        {
            src.x = _controller.GetActorTransform().position.x;
            src.y = _controller.GetActorTransform().position.z;
            dest.x = p.worldCoordX;
            dest.y = p.worldCoordZ;
            Vector2 dir = dest - src;
            if (Vector2.Distance(src, dest) <= dist)
            {
                p = path.Pop();
                _controller.LookAt(new Vector3(dir.x, 0.0f, dir.y));
            }
            else _controller.Move(new Vector3(dir.x, 0.0f, dir.y), 1.5f);
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
    public ActorController controller
    {
        set { _controller = value; }
    }
    private ActorController _controller;
    public override bool Invoke()
    {
        return true;
    }
}
public class StopAttack : Node
{
    public ActorController controller
    {
        set { _controller = value; }
    }
    private ActorController _controller;
    public override bool Invoke()
    {
        return true;
    }
}

public class DeadProcess : Node
{
    public ActorController controller
    {
        set { _controller = value; }
    }
    private ActorController _controller;
    public override bool Invoke()
    {
        return true;
    }
}

public class CheckDead : Node
{
    public ActorController controller
    {
        set { _controller = value; }
    }
    private ActorController _controller;
    public override bool Invoke()
    {
        return false;
    }
}
