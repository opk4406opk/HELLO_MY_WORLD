using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleGameMode : AGameModeBase
{
    public override void Init()
    {
        ModeState = GameModeState.SINGLE;
        GameNetworkManager.GetInstance().UserNetType = GameUserNetType.Host;
        GameNetworkManager.GetInstance().ConnectToGameServer("127.0.0.1", 8000);
        //
        Cursor.lockState = CursorLockMode.Confined;
    }

    public override void Tick(float DeltaTime)
    {
    }
}
