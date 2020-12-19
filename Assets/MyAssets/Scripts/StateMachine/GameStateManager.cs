using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStateType
{
    None,
    Prepare,
    Start,
    InGame,
    End,
}
public class GameStateManager
{
    public GameStateType CurrentStateType { get; private set; }
    private AGameState CurrentStateInstance = null;
    public void Initialize()
    {

    }

    public void ChangeState(GameStateType newStateType)
    {
        if (CurrentStateInstance != null) CurrentStateInstance.EndState();

        switch (newStateType)
        {
            case GameStateType.Prepare:
                CurrentStateInstance = new PrepareGameState(this);
                break;
            case GameStateType.Start:
                CurrentStateInstance = new StartGameState(this);
                break;
            case GameStateType.InGame:
                CurrentStateInstance = new InGameGameState(this);
                break;
            case GameStateType.End:
                CurrentStateInstance = new EndGameState(this);
                break;
        }
        CurrentStateType = newStateType;
        CurrentStateInstance.StartState();
    }

    public void UpdateProcess()
    {
        if (CurrentStateInstance != null) CurrentStateInstance.UpdateState(Time.deltaTime);
    }

}
