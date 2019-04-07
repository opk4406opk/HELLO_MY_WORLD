
public interface IState
{
    void InitState();
    void UpdateState(float deltaTime);
    void ReleaseState();
}
