using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AActorState
{
    protected Actor ActorInstance;
}

public enum ActorStateType
{
    //commons
    Idle, // default.
    Run,
    Walk
}
