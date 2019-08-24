using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class Node
{
    public abstract bool Invoke(float DeltaTime);
    protected ActorController Controller;
    protected BehaviorTree BehaviorTreeInstance;
}

public class CompositeNode : Node
{
    public override bool Invoke(float DeltaTime)
    {
        throw new NotImplementedException();
    }

    public void AddChild(Node node)
    {
        ChildrenNodes.Push(node);
    }

    public Stack<Node> GetChildrens()
    {
        return ChildrenNodes;
    }
    private Stack<Node> ChildrenNodes = new Stack<Node>();
}

public class Selector : CompositeNode
{
    public override bool Invoke(float DeltaTime)
    {
        foreach (var node in GetChildrens())
        {
            if (node.Invoke(DeltaTime))
            {
                return true;
            }
        }
        return false;
    }
}

public class Sequence : CompositeNode
{
    public override bool Invoke(float DeltaTime)
    {
        foreach (var node in GetChildrens())
        {
            if (!node.Invoke(DeltaTime))
            {
                return false;
            }
        }
        return true;
    }
}

/// <summary>
/// AI 종류.
/// </summary>
public enum AITypes
{
    Common,
    NONE,
    COUNT = NONE
}

public abstract class BehaviorTree
{
    protected Sequence RootNode = new Sequence();
    protected ActorController ActorControllerInstance;
    protected BlackBoard BlackBoardInstance;
    protected IEnumerator BehaviorProcessInstance;
    protected bool bRunningBT = false;

    public abstract void Initialize(ActorController actorController);
    protected IEnumerator BehaviorProcess()
    {
        KojeomLogger.DebugLog("BehaviorProcess Start!!");
        while(bRunningBT == true)
        {
            if(ActorControllerInstance != null && ActorControllerInstance.IsContactGround() == true)
            {
                RootNode.Invoke(Time.deltaTime);
            }
            yield return null;
        }
        KojeomLogger.DebugLog("Behavior process exit");
    }
    public void StartBT()
    {
        bRunningBT = true;
        BehaviorProcessInstance = BehaviorProcess();
        ActorControllerInstance.StartCoroutine(BehaviorProcessInstance);
    }
    public void StopBT()
    {
        bRunningBT = false;
        ActorControllerInstance.StopCoroutine(BehaviorProcessInstance);
    }

    public BlackBoard GetBlackBoard()
    {
        return BlackBoardInstance;
    }

    public bool IsRunning()
    {
        return bRunningBT;
    }
}