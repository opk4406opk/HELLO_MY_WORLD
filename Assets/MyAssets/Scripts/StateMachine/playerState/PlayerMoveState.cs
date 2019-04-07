using UnityEngine;
using UnityEngine.Networking;

public class PlayerMoveState : APlayerState, IState
{
    private float MoveSpeed;
    private QuerySDMecanimController AniController;
    private BoxCollider BoxColliderInstance;
    private InputData CurPressedInput;

    public PlayerMoveState(GamePlayer gamePlayer)
    {
        GamePlayer = gamePlayer;
        //
        AniController = GamePlayer.Controller.CharacterInstance.QueryMecanimController;
        BoxColliderInstance = GamePlayer.Controller.CharacterInstance.BoxColliderInstance;
        //
        MoveSpeed = 3.5f;
    }
    public void InitState()
    {
        AniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
    }

    public void ReleaseState()
    {
        CurPressedInput.keyCode = KeyCode.None;
        CurPressedInput.mobileInputType = MOBILE_INPUT_TYPE.NONE;
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
        if (CurPressedInput.keyCode == KeyCode.W)
        {
            return GamePlayer.Controller.CharacterInstance.transform.forward;
        }
        else if (CurPressedInput.keyCode == KeyCode.S)
        {
            return -GamePlayer.Controller.CharacterInstance.transform.forward;
        }
        else if (CurPressedInput.keyCode == KeyCode.D)
        {
            return GamePlayer.Controller.CharacterInstance.transform.right;
        }
        else if (CurPressedInput.keyCode == KeyCode.A)
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
        if(InputManager.singleton != null)
        {
            CurPressedInput = InputManager.singleton.GetInputData();
            KojeomLogger.DebugLog(string.Format("curPressedKey : {0}", CurPressedInput.keyCode), LOG_TYPE.USER_INPUT);
        }
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
        //
        P2PNetworkManager.GetInstance().PushCharStateMessage(GAMEPLAYER_CHAR_STATE.MOVE);
        Vector3 move = dir.normalized * MoveSpeed;
        GamePlayer.Controller.LerpPosition(move);
    }
}
