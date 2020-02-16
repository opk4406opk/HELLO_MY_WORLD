using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor와 관련된 행동트리 노드들이 정의되어 있는 파일. 
 */

public class BTNodeMoveForTarget : Node
{
    public CustomAstar3D PathFinderInstance { get; private set; } = new CustomAstar3D();
    //
    private float ReCalcPathfindingTimeSec = 5.0f;
    public BTNodeMoveForTarget(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
   
    public override bool Invoke(float DeltaTime)
    {
        ElapsedTimeSec += DeltaTime;
        bool bRecalcPathFinding = BehaviorTreeInstance.GetBlackBoard().PathList.Count == 0 && ElapsedTimeSec >= ReCalcPathfindingTimeSec;
        if (bRecalcPathFinding == true)
        {
            ElapsedTimeSec = 0.0f;
            AsyncPathFinding(BehaviorTreeInstance.GetBlackBoard().PathFidningTargetPoint);
        }

        if (BehaviorTreeInstance.GetBlackBoard().PathList.Count > 0)
        {
            // test code.
            PathNode3D node = BehaviorTreeInstance.GetBlackBoard().PathList.Pop();
            Controller.StartRun(node.GetWorldPosition());
        }
        return true;
    }
    public void AsyncPathFinding(Vector3 goalWorldPosition)
    {
        // init
        PathFinderSettings needData = new PathFinderSettings(Controller.GetContainedWorldBlockData(),
                                                             Controller.GetContainedSubWorldOffset(),
                                                             Controller.GetContainedWorldAreaOffset());
        PathFinderInstance.Init(needData, new SimpleVector3(Controller.GetActorTransform().position));
        PathFinderInstance.OnFinishAsyncPathFinding += OnFinishAsyncPathFinding;
        // async start.
        PathFinderInstance.AsyncPathFinding(goalWorldPosition);
    }
    private void OnFinishAsyncPathFinding(Stack<PathNode3D> resultPath)
    {
        BehaviorTreeInstance.GetBlackBoard().PathList = resultPath;
    }
}

// 이곳저곳 배회하는 노드.
public class BTNodeWandering : Node
{
    private readonly float WakeupTimeSec = 3.0f;
    public BTNodeWandering(BehaviorTree behaviorTreeInstance, ActorController actorController)
    {
        Controller = actorController;
        BehaviorTreeInstance = behaviorTreeInstance;
    }
    public override bool Invoke(float DeltaTime)
    {
        ElapsedTimeSec += DeltaTime;
        if(ElapsedTimeSec >= WakeupTimeSec)
        {
            Vector3 targetPos;
            if(AIUtils.GetRandomWorldPositionFromActorPos(out targetPos, Controller) == true)
            {
                if(Controller.GetCurrentState() != ActorStateType.Run)
                {
                    //KojeomLogger.DebugLog(string.Format("Wandering start!"));
                    Controller.StartRun(targetPos);
                }
            }
            ElapsedTimeSec = 0.0f;
        }
        return true;
    }

}

public class BTNodeTimer : Node
{
    private float WakeupTimeSec = 2.0f;
    public delegate void OnAfterTimer();
    private OnAfterTimer CallBack;

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

    public void SetCallbackAfterTimer(OnAfterTimer callback)
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
