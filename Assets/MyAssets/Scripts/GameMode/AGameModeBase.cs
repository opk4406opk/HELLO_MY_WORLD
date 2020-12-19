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
    EDITOR_PLAY, // 에디터에서 곧 바로 플레이.
    NEW_GAME, // 새 게임 시작.
    LOAD_GAME // 저장된 데이터 불러오고 플레이.
}
public abstract class AGameModeBase
{
    public abstract void Init();
    public abstract void UpdateProcess(float DeltaTime);

    //
    protected GameModeState ModeState = GameModeState.NONE;

    public GameModeState GetGameModeState()
    {
        return ModeState;
    }
}
