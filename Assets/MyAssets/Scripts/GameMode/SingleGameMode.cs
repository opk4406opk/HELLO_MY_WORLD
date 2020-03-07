using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGameMode : AGameModeBase
{
    public override void Init()
    {
        ModeState = GameModeState.SINGLE;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Tick(float DeltaTime)
    {
    }
}
