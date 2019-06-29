using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RoamingNpcAI : BehaviorTree
{
    private Selector selector = new Selector();
    private Sequence seqMoveForTarget = new Sequence();
    private Sequence seqDead = new Sequence();

    private MoveForTarget moveForTarget = new MoveForTarget();
    private DeadProcess deadProcess = new DeadProcess();
    private CheckDead chkDead = new CheckDead();

    public override void Initialize(ActorController actorController, PathFinderInitData pathData)
    {
        RootNode.AddChild(selector);
        moveForTarget.SetController(actorController);
        moveForTarget.InitPathFinder(pathData);
        moveForTarget.PathFinding(Vector3.zero);
        selector.AddChild(seqMoveForTarget);
        selector.AddChild(seqDead);

        seqMoveForTarget.AddChild(moveForTarget);
        seqDead.AddChild(deadProcess);
        seqDead.AddChild(chkDead);
    }
}
