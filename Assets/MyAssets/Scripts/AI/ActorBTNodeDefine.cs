using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor와 관련된 행동트리 노드들이 정의되어 있는 파일. 
 */

public class BTNodeMoveForTarget : Node
{
    private readonly float Distance = 0.12f;
    private Stack<PathNode3D> PathList;
    private Vector3 StartPosition, GoalPosition;
    
    public BTNodeMoveForTarget(BehaviorTree behaviorTreeInstance)
    {
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public CustomAstar3D PathFinderInstance { get; private set; } = new CustomAstar3D();

    public override bool Invoke(float DeltaTime)
    {
        if (BehaviorTreeInstance.BlackBoardInstance.NavigateList.Count > 0)
        {
            PathNode3D node = null;
            StartPosition = Controller.GetActorTransform().position;
            GoalPosition = node.GetWorldPosition();
            Vector3 dir = GoalPosition - StartPosition;
            if (Vector3.Distance(StartPosition, GoalPosition) <= Distance)
            {
                node = BehaviorTreeInstance.BlackBoardInstance.NavigateList.Pop();
                Controller.LookAt(dir);
            }
            else Controller.Move(dir, 1.5f);
            return false;
        }
        else return true;
    }
    public void InitPathFinder(PathFinderInitData data)
    {
        PathFinderInstance.Init(data, Controller.GetActorTransform());
        PathFinderInstance.OnFinishAsyncPathFinding += OnFinishAsyncPathFinding;
    }
    public void PathFinding(Vector3 goalWorldPosition)
    {
        BehaviorTreeInstance.BlackBoardInstance.NavigateList = PathFinderInstance.PathFinding(goalWorldPosition);
    }

    public void AsyncPathFinding(Vector3 goalWorldPosition)
    {
        PathFinderInstance.AsyncPathFinding(goalWorldPosition);
    }

    private void OnFinishAsyncPathFinding(Stack<PathNode3D> resultPath)
    {
        BehaviorTreeInstance.BlackBoardInstance.NavigateList = resultPath;
    }
}

public class BTNodeStartAttack : Node
{
    public BTNodeStartAttack(BehaviorTree behaviorTreeInstance)
    {
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return true;
    }
}
public class BTNodeStopAttack : Node
{
    public BTNodeStopAttack(BehaviorTree behaviorTreeInstance)
    {
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return true;
    }
}

public class BTNodeDeadProcess : Node
{
    public BTNodeDeadProcess(BehaviorTree behaviorTreeInstance)
    {
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return true;
    }
}

public class BTNodeCheckDead : Node
{
    public BTNodeCheckDead(BehaviorTree behaviorTreeInstance)
    {
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return false;
    }
}
