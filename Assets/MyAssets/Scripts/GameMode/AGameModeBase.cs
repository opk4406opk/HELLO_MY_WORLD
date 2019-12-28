using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GameModeState
{
    SINGLE,
    MULTI,
    NONE,
    COUNT = NONE
}
public enum DetailSingleMode
{
    NONE,
    EDITOR_PLAY,
    SAVE_GAME,
    LOAD_GAME
}
public abstract class AGameModeBase
{
    public abstract void Init();
    public abstract void Tick(float DeltaTime);

    //
    protected GameModeState ModeState = GameModeState.NONE;

    public GameModeState GetGameModeState()
    {
        return ModeState;
    }
}
