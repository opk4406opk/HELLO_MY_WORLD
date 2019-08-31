using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class CommonNpcAI : BehaviorTree
{
    private Sequence SeqMoveForTarget = new Sequence();

    private BTNodeMoveForTarget MoveForTargetNode;
    private BTNodeTimer TargetPosUpdateNode;

    public override void Initialize(ActorController actorController)
    {
        BlackBoardInstance = new BlackBoard();
        ActorControllerInstance = actorController;
        // create instance
        MoveForTargetNode = new BTNodeMoveForTarget(this, actorController);
        //
        TargetPosUpdateNode = new BTNodeTimer(this, actorController);
        TargetPosUpdateNode.SetCallbackAfterTimer(() => {
            if (GamePlayerManager.Instance != null)
            {
                BlackBoardInstance.PathFidningTargetPoint = GamePlayerManager.Instance.MyGamePlayer.Controller.GetPosition();
            }
        });
        //
        RootNode.AddChild(SeqMoveForTarget);
        // 이동 시퀀스
        SeqMoveForTarget.AddChild(TargetPosUpdateNode);
        SeqMoveForTarget.AddChild(MoveForTargetNode);
    }
}
