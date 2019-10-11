using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor와 관련된 행동트리 노드들이 정의되어 있는 파일. 
 */

public class BTNodeMoveForTarget : Node
{
    public CustomAstar3D PathFinderInstance { get; private set; } = new CustomAstar3D();
    public BTNodeMoveForTarget(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
   
    public override bool Invoke(float DeltaTime)
    {
        if (BehaviorTreeInstance.GetBlackBoard().PathList.Count > 0)
        {
            // test code.
            PathNode3D node = BehaviorTreeInstance.GetBlackBoard().PathList.Pop();
            Controller.Teleport(node.GetWorldPosition());
        }
        else
        {
            AsyncPathFinding(BehaviorTreeInstance.GetBlackBoard().PathFidningTargetPoint);
        }
        return true;
    }
    public void AsyncPathFinding(Vector3 goalWorldPosition)
    {
        // init
        PathFinderInitData initData = new PathFinderInitData(Controller.GetContainedWorldBlockData(),
                                                             Controller.GetContainedSubWorldOffset(),
                                                             Controller.GetContainedWorldAreaOffset());
        PathFinderInstance.Init(initData, new SimpleVector3(Controller.GetActorTransform().position));
        PathFinderInstance.OnFinishAsyncPathFinding += OnFinishAsyncPathFinding;
        // async start.
        PathFinderInstance.AsyncPathFinding(goalWorldPosition);
    }
    private void OnFinishAsyncPathFinding(Stack<PathNode3D> resultPath)
    {
        BehaviorTreeInstance.GetBlackBoard().PathList = resultPath;
    }
}

public class BTNodeTimer : Node
{
    private float ElapsedTimeSec = 0.0f;
    private float WakeupTimeSec = 2.0f;
    public delegate void CallBackAfterTimer();
    private CallBackAfterTimer CallBack;

    public BTNodeTimer(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        if(ElapsedTimeSec <= WakeupTimeSec)
        {
            ElapsedTimeSec += DeltaTime;
        }
        else
        {
            ElapsedTimeSec = 0.0f;
            CallBack();
        }
        return true;
    }

    public void SetCallbackAfterTimer(CallBackAfterTimer callback)
    {
        CallBack = callback;
    }

    public void SetWakeupTime(float sec)
    {
        WakeupTimeSec = sec;
    }
}

public class BTNodeStartAttack : Node
{
    public BTNodeStartAttack(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return true;
    }
}
public class BTNodeStopAttack : Node
{
    public BTNodeStopAttack(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return true;
    }
}

public class BTNodeDeadProcess : Node
{
    public BTNodeDeadProcess(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return true;
    }
}

public class BTNodeCheckDead : Node
{
    public BTNodeCheckDead(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        return false;
    }
}
