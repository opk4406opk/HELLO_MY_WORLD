﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Actor와 관련된 행동트리 노드들이 정의되어 있는 파일. 
 */

public class BTNodeMoveForTarget : Node
{
    private bool bAlreadyPathFinding = false;
    private readonly float IntervalDistance = 0.12f;
    private Vector3 StartPosition, GoalPosition;
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
            PathNode3D node = BehaviorTreeInstance.GetBlackBoard().PathList.Pop();
            StartPosition = Controller.GetActorTransform().position;
            GoalPosition = node.GetWorldPosition();
            Vector3 dir = GoalPosition - StartPosition;
            if (Vector3.Distance(StartPosition, GoalPosition) <= IntervalDistance)
            {
                bAlreadyPathFinding = false;
                Controller.LookAt(dir);
            }
            else
            {
                Controller.Move(dir, 1.5f);
            }
            return false;
        }
        else
        {
            if(bAlreadyPathFinding == false)
            {
                bAlreadyPathFinding = true;
                AsyncPathFinding(BehaviorTreeInstance.GetBlackBoard().PathFidningTargetPoint);
            }
            return true;
        }
    }
    public void InitPathFinder()
    {
        PathFinderInitData initData;
        initData.WorldBlockData = Controller.GetContainedWorldBlockData();
        initData.OffsetX = (int)Controller.GetSubWorldOffset().x;
        initData.OffsetY = (int)Controller.GetSubWorldOffset().y;
        initData.OffsetZ = (int)Controller.GetSubWorldOffset().z;
        PathFinderInstance.Init(initData, Controller.GetActorTransform());
        PathFinderInstance.OnFinishAsyncPathFinding += OnFinishAsyncPathFinding;
    }
    public void AsyncPathFinding(Vector3 goalWorldPosition)
    {
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
