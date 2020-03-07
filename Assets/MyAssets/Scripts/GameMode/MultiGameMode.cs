using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGameMode : AGameModeBase
{
    public override void Init()
    {
        ModeState = GameModeState.MULTI;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Tick(float DeltaTime)
    {
    }
}
