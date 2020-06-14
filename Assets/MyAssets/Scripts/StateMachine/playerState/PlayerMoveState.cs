using UnityEngine;
using UnityEngine.Networking;

public class PlayerMoveState : APlayerState, IState
{
    private const float MoveSpeed = 6.5f;
    private QuerySDMecanimController AniController;
    private BoxCollider BoxColliderInstance;

    public PlayerMoveState(GamePlayer gamePlayer, InputData inputData)
    {
        GamePlayer = gamePlayer;
        InputData = inputData;
        BoxColliderInstance = GamePlayer.Controller.CharacterInstance.BoxColliderInstance;
    }
    public void InitState()
    {
        if (GamePlayer.Controller.CharacterInstance.QueryMecanimController != null)
        {
            GamePlayer.Controller.CharacterInstance.QueryMecanimController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
        }
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
        Move();
    }

    private Vector3 CalcMobileMoveDir()
    {
        Vector3 dir = Vector3.zero;
        var virtualJoystick = VirtualJoystickManager.singleton;
        if(virtualJoystick != null)
        {
            // 스크린좌표에서 얻은 2차원 방향값을 3차원 좌표계( = 플레이어가 존재하는 월드 좌표계)로 변환.
            dir = GamePlayer.Controller.CharacterInstance.transform.TransformDirection(virtualJoystick.GetMoveDirection());
        }
        return dir;
    }
    private Vector3 CalcWindowMoveDir()
    {
        Vector3 dirVector = Vector3.zero;
        foreach(KeyCode keycode in InputData.KeyCodeValues)
        {
            switch(keycode)
            {
                case KeyCode.W:
                    dirVector += GamePlayer.Controller.CharacterInstance.transform.forward;
                    break;
                case KeyCode.S:
                    dirVector += GamePlayer.Controller.CharacterInstance.transform.forward * -1;
                    break;
                case KeyCode.D:
                    dirVector += GamePlayer.Controller.CharacterInstance.transform.right;
                    break;
                case KeyCode.A:
                    dirVector += GamePlayer.Controller.CharacterInstance.transform.right * -1;
                    break;
            }
        }
        return dirVector;
    }

    private void Move()
    {
        if (InputData.KeyCodeValues.Count > 0)
        {
            for(int idx = 0; idx < InputData.KeyCodeValues.Count; idx++)
            {
                KojeomLogger.DebugLog(string.Format("USER InputData : {0}", InputData.KeyCodeValues[idx]), LOG_TYPE.USER_INPUT);
            }
        }
        //
        Vector3 dir = Vector3.zero;
        switch(KojeomUtility.GetInputDeviceType())
        {
            case INPUT_DEVICE_TYPE.MOBILE:
                dir = CalcMobileMoveDir();
                break;
            case INPUT_DEVICE_TYPE.WINDOW:
                dir = CalcWindowMoveDir();
                break;
        }

        if(dir == Vector3.zero)
        {
            return;
        }
        Vector3 moveValue = dir.normalized * MoveSpeed;
        GamePlayer.Controller.Move(moveValue);
    }
}
