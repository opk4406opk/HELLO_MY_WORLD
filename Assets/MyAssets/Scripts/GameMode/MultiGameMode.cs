﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiGameMode : AGameModeBase
{
    public override void Init()
    {
        ModeState = GameModeState.MULTI;
    }

    public override void UpdateProcess(float DeltaTime)
    {
    }
}
