using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateController {

    private IState curState;
    public void SetState(IState state)
    {
        if(curState != null)
        {
            curState.ReleaseState();
        }
        curState = state;
        curState.InitState();
    }

    public void UpdateState()
    {
        curState.UpdateState();
    }
}
