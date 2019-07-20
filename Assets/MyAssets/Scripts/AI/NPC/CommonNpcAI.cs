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

    public override void Initialize(ActorController actorController)
    {
        // create instance
        MoveForTargetNode = new BTNodeMoveForTarget(this, actorController);
        DeadProcessNode = new BTNodeDeadProcess(this, actorController);
        ChkDeadNode = new BTNodeCheckDead(this, actorController);
        //
        RootNode.AddChild(Selector);
        MoveForTargetNode.InitPathFinder();
        Selector.AddChild(SeqMoveForTarget);
        Selector.AddChild(SeqDead);

        SeqMoveForTarget.AddChild(MoveForTargetNode);
        SeqDead.AddChild(DeadProcessNode);
        SeqDead.AddChild(ChkDeadNode);
    }
}
