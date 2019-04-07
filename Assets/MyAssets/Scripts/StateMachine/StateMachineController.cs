public class StateMachineController {

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

    public void Tick(float deltaTime)
    {
        if(curState != null)
        {
            curState.UpdateState(deltaTime);
        }
    }
}
