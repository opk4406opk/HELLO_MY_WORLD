using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CommonNpcAI : BehaviorTree
{
    private Selector Selector = new Selector();
    private Sequence SeqMoveForTarget = new Sequence();
    private Sequence SeqDead = new Sequence();

    private BTNodeMoveForTarget MoveForTargetNode = new BTNodeMoveForTarget();
    private BTNodeDeadProcess DeadProcessNode = new BTNodeDeadProcess();
    private BTNodeCheckDead ChkDeadNode = new BTNodeCheckDead();

    public override void Initialize(ActorController actorController, PathFinderInitData pathData)
    {
        RootNode.AddChild(Selector);
        MoveForTargetNode.SetController(actorController);
        MoveForTargetNode.InitPathFinder(pathData);
        MoveForTargetNode.PathFinding(Vector3.zero);
        Selector.AddChild(SeqMoveForTarget);
        Selector.AddChild(SeqDead);

        SeqMoveForTarget.AddChild(MoveForTargetNode);
        SeqDead.AddChild(DeadProcessNode);
        SeqDead.AddChild(ChkDeadNode);
    }
}
