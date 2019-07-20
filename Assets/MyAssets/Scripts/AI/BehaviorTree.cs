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
    COUNT
}

public abstract class BehaviorTree : MonoBehaviour
{
    protected Sequence RootNode = new Sequence();
    public BlackBoard BlackBoardInstance
    {
        get { return BlackBoardInstance; }
        private set { BlackBoardInstance = new BlackBoard(); }
    }
    protected IEnumerator BehaviorProcessInstance;

    public abstract void Initialize(ActorController actorController);
    protected IEnumerator BehaviorProcess()
    {
        KojeomLogger.DebugLog("BehaviorProcess Start!!");
        while (!RootNode.Invoke(Time.deltaTime)) yield return null;
        KojeomLogger.DebugLog("Behavior process exit");
    }
    public void StartBT()
    {
        BehaviorProcessInstance = BehaviorProcess();
        StartCoroutine(BehaviorProcessInstance);
    }
    public void StopBT()
    {
        StopCoroutine(BehaviorProcessInstance);
    }
}