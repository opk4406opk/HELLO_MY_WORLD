﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameGameState : AGameState
{
    public InGameGameState(GameStateManager stateManager)
    {
        StateManagerInstance = stateManager;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override void StartState()
    {
        StateType = GameStateType.InGame;
        base.StartState();
    }

    public override void EndState()
    {
        base.EndState();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
    }
}
