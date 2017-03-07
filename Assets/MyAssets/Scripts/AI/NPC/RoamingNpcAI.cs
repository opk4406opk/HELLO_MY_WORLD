using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class RoamingNpcAI : BehaviorTree
{
    private Sequence root = new Sequence();
    private Selector selector = new Selector();
    private Sequence seqMoveForTarget = new Sequence();
    private Sequence seqDead = new Sequence();

    private MoveForTarget moveForTarget = new MoveForTarget();
    private DeadProcess deadProcess = new DeadProcess();
    private ChkDead chkDead = new ChkDead();

    public override void Init(ActorController actorController, PathFinderInitData pathData)
    {
        root.AddChild(selector);
        moveForTarget.controller = actorController;
        moveForTarget.InitPathFinder(pathData);
        moveForTarget.pathFinder.SetGoalPathNode(22, 25);
        moveForTarget.PathFinding();
        selector.AddChild(seqMoveForTarget);
        selector.AddChild(seqDead);

        seqMoveForTarget.AddChild(moveForTarget);
        seqDead.AddChild(deadProcess);
        seqDead.AddChild(chkDead);
    }

    private IEnumerator behaviorProcess;
    public override IEnumerator BehaviorProcess()
    {
        Debug.Log("BehaviorProcess Start!!");
        while (!root.Invoke()) yield return null;
        Debug.Log("behavior process exit");
    }

    public override void StartBT()
    {
        behaviorProcess = BehaviorProcess();
        StartCoroutine(behaviorProcess);
    }

    public override void StopBT()
    {
        StopCoroutine(behaviorProcess);
    }

}
