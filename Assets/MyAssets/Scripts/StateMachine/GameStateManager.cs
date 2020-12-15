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
                CurrentStateInstance = new PrepareGameState();
                break;
            case GameStateType.Start:
                CurrentStateInstance = new StartGameState();
                break;
            case GameStateType.InGame:
                CurrentStateInstance = new InGameGameState();
                break;
            case GameStateType.End:
                CurrentStateInstance = new EndGameState();
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
