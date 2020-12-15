using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AGameState
{
    virtual public void StartState()
    {
        KojeomLogger.DebugLog(string.Format("{0} state Start!", KojeomUtility.EnumToString<GameStateType>(StateType)));
    }
    virtual public void UpdateState(float deltaTime)
    {

    }
    virtual public void EndState()
    {
        KojeomLogger.DebugLog(string.Format("{0} state End!", KojeomUtility.EnumToString<GameStateType>(StateType)));
    }

    protected GameStateType StateType = GameStateType.None;
}
