using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor와 관련된 행동트리 노드들이 정의되어 있는 파일. 
 */

public class MoveForTarget : Node
{
    private readonly float Distance = 0.12f;
    private Stack<PathNode3D> PathList;
    private Vector3 StartPosition, GoalPosition;
    
    public CustomAstar3D PathFinderInstance { get; private set; } = new CustomAstar3D();

    public override bool Invoke()
    {
        if (PathList.Count > 0)
        {
            PathNode3D node = null;
            StartPosition = Controller.GetActorTransform().position;
            GoalPosition = node.GetWorldPosition();
            Vector3 dir = GoalPosition - StartPosition;
            if (Vector3.Distance(StartPosition, GoalPosition) <= Distance)
            {
                node = PathList.Pop();
                Controller.LookAt(dir);
            }
            else Controller.Move(dir, 1.5f);
            return false;
        }
        else return true;
    }
    public void InitPathFinder(PathFinderInitData data)
    {
        PathFinderInstance.Init(data);
    }
    public void PathFinding(Vector3 goalWorldPosition)
    {
        PathList = PathFinderInstance.PathFinding(goalWorldPosition);
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
