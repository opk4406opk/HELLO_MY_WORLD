using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonAnimalAI : BehaviorTree
{
    private Sequence SeqWandering = new Sequence();
    private BTNodeWandering WanderingNode;

    public override void Initialize(ActorController actorController)
    {
        BlackBoardInstance = new BlackBoard();
        ActorControllerInstance = actorController;

        RootNode.AddChild(SeqWandering);
        WanderingNode = new BTNodeWandering(this, actorController);
        SeqWandering.AddChild(WanderingNode);
    }
}
