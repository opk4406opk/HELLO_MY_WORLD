public class StateMachineController
{
    private IState curState;
    public void ChangeState(IState toState)
    {
        if(curState != null)
        {
            curState.ReleaseState();
        }
        curState = toState;
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
