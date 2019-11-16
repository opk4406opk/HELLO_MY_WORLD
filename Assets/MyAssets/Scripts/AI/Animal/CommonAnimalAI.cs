using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonAnimalAI : BehaviorTree
{
    public override void Initialize(ActorController actorController)
    {
        BlackBoardInstance = new BlackBoard();
        ActorControllerInstance = actorController;
    }
}
