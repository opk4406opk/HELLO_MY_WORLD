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
    private BTNodeTimer TimerNode;

    public override void Initialize(ActorController actorController)
    {
        BlackBoardInstance = new BlackBoard();
        ActorControllerInstance = actorController;
        // create instance
        MoveForTargetNode = new BTNodeMoveForTarget(this, actorController);
        DeadProcessNode = new BTNodeDeadProcess(this, actorController);
        ChkDeadNode = new BTNodeCheckDead(this, actorController);
        TimerNode = new BTNodeTimer(this, actorController);
        //
        RootNode.AddChild(Selector);
        MoveForTargetNode.InitPathFinder();
        TimerNode.SetCallbackAfterTimer(() => {
            if(GamePlayerManager.Instance != null)
            {
                BlackBoardInstance.PathFidningTargetPoint = GamePlayerManager.Instance.MyGamePlayer.Controller.GetPosition();
            }
        });
        //
        Selector.AddChild(SeqMoveForTarget);
        Selector.AddChild(SeqDead);
        // 이동 시퀀스
        SeqMoveForTarget.AddChild(TimerNode);
        SeqMoveForTarget.AddChild(MoveForTargetNode);
        // 
        SeqDead.AddChild(DeadProcessNode);
        SeqDead.AddChild(ChkDeadNode);
    }
}
