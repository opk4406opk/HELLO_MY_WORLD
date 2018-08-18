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
        if(curState != null)
        {
            curState.UpdateState();
        }
    }
}
