using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CommonNpcAI : BehaviorTree
{
    private Sequence SeqMoveForTarget = new Sequence();

    private BTNodeMoveForTarget MoveForTargetNode;
    private BTNodeTimer TimerNode;

    public override void Initialize(ActorController actorController)
    {
        BlackBoardInstance = new BlackBoard();
        ActorControllerInstance = actorController;
        // create instance
        MoveForTargetNode = new BTNodeMoveForTarget(this, actorController);
        MoveForTargetNode.InitPathFinder();
        //
        TimerNode = new BTNodeTimer(this, actorController);
        TimerNode.SetCallbackAfterTimer(() => {
            if (GamePlayerManager.Instance != null)
            {
                BlackBoardInstance.PathFidningTargetPoint = GamePlayerManager.Instance.MyGamePlayer.Controller.GetPosition();
            }
        });
        //
        RootNode.AddChild(SeqMoveForTarget);
        // 이동 시퀀스
        SeqMoveForTarget.AddChild(TimerNode);
        SeqMoveForTarget.AddChild(MoveForTargetNode);
    }
}
