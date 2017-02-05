using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public abstract class Node
{
    public abstract bool Invoke();
}

public class CompositeNode : Node
{
    public override bool Invoke()
    {
        throw new NotImplementedException();
    }

    public void AddChild(Node node)
    {
        childrens.Push(node);
    }

    public Stack<Node> GetChildrens()
    {
        return childrens;
    }
    private Stack<Node> childrens = new Stack<Node>();
}

public class Selector : CompositeNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (node.Invoke())
            {
                return true;
            }
        }
        return false;
    }
}

public class Sequence : CompositeNode
{
    public override bool Invoke()
    {
        foreach (var node in GetChildrens())
        {
            if (!node.Invoke())
            {
                return false;
            }
        }
        return true;
    }
}

public class MoveForTarget : Node
{
    private readonly float dist = 0.12f;
    private Stack<PathNode> path;
    private Vector2 src, dest;
    private PathNode p = null;

    public CustomAstar pathFinder
    {
        get { return _pathFinder; }
    }
    private CustomAstar _pathFinder = new CustomAstar();
   
    public ActorController controller
    {
        set { _controller = value; }
    }
    private ActorController _controller;
    public override bool Invoke()
    {
        if (path.Count > 0)
        {
            src.x = _controller.curPos.position.x;
            src.y = _controller.curPos.position.z;
            dest.x = p.worldCoordX;
            dest.y = p.worldCoordZ;
            Debug.Log(Vector2.Distance(src, dest));
            if (Vector2.Distance(src, dest) <= dist) p = path.Pop();
            else _controller.Move(new Vector3(dest.x - src.x, 0.0f, dest.y - src.y), 1.5f);
            return false;
        }
        else return true;
    }
    public void InitPathFinder(PathFinderInitData data)
    {
        _pathFinder.Init(data);
    }
    public void PathFinding()
    {
        path = _pathFinder.PathFinding();
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

public class ChkDead : Node
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

public abstract class BehaviorTree : MonoBehaviour
{
    public abstract void Init(ActorController actorController, PathFinderInitData pathData);
    public abstract void StartBT();
    public abstract void StopBT();
    public abstract IEnumerator BehaviorProcess();
}