using UnityEngine;
using UnityEngine.Networking;

public class PlayerMoveState : APlayerState, IState
{
    private float MoveSpeed;
    private QuerySDMecanimController AniController;
    private BoxCollider BoxColliderInstance;

    public PlayerMoveState(GamePlayer gamePlayer, InputData inputData)
    {
        GamePlayer = gamePlayer;
        InputData = inputData;
        BoxColliderInstance = GamePlayer.Controller.CharacterInstance.BoxColliderInstance;
        MoveSpeed = 3.5f;
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
            // 스크린좌표에서 얻은 2차원 방향값을 3차원 좌표계로 변환.
            dir = GamePlayer.Controller.CharacterInstance.transform.TransformDirection(virtualJoystick.GetMoveDirection());
        }
        return dir;
    }
    private Vector3 CalcWindowMoveDir()
    {
        if (InputData.KeyCodeValue == KeyCode.W)
        {
            return GamePlayer.Controller.CharacterInstance.transform.forward;
        }
        else if (InputData.KeyCodeValue == KeyCode.S)
        {
            return -GamePlayer.Controller.CharacterInstance.transform.forward;
        }
        else if (InputData.KeyCodeValue == KeyCode.D)
        {
            return GamePlayer.Controller.CharacterInstance.transform.right;
        }
        else if (InputData.KeyCodeValue == KeyCode.A)
        {
            return -GamePlayer.Controller.CharacterInstance.transform.right;
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void Move()
    {
        KojeomLogger.DebugLog(string.Format("USER InputData : {0}", InputData.KeyCodeValue), LOG_TYPE.USER_INPUT);
        //
        Vector3 dir = Vector3.zero;
        if(Application.platform == RuntimePlatform.WindowsEditor ||
            Application.platform == RuntimePlatform.WindowsPlayer)
        {
            dir = CalcWindowMoveDir();
        }
        else if(Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            dir = CalcMobileMoveDir();
        }
        else if(dir == Vector3.zero)
        {
            return;
        }
        Vector3 move = dir.normalized * MoveSpeed;
        GamePlayer.Controller.LerpPosition(move);
    }
}
