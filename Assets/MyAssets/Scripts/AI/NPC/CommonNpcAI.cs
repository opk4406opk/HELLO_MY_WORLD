using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CommonNpcAI : BehaviorTree
{
    private Selector Selector = new Selector();
    private Sequence SeqMoveForTarget = new Sequence();
    private Sequence SeqDead = new Sequence();

    private BTNodeMoveForTarget MoveForTargetNode;
    private BTNodeDeadProcess DeadProcessNode;
    private BTNodeCheckDead ChkDeadNode;

    public override void Initialize(ActorController actorController, PathFinderInitData pathData)
    {
        // create instance
        MoveForTargetNode = new BTNodeMoveForTarget(this);
        DeadProcessNode = new BTNodeDeadProcess(this);
        ChkDeadNode = new BTNodeCheckDead(this);
        //
        RootNode.AddChild(Selector);
        MoveForTargetNode.SetController(actorController);
        MoveForTargetNode.InitPathFinder(pathData);
        Selector.AddChild(SeqMoveForTarget);
        Selector.AddChild(SeqDead);

        SeqMoveForTarget.AddChild(MoveForTargetNode);
        SeqDead.AddChild(DeadProcessNode);
        SeqDead.AddChild(ChkDeadNode);
    }
}
