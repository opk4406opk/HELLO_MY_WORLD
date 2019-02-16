using UnityEngine;
using UnityEngine.Networking;

public class PlayerMoveState : IState
{
    private float moveSpeed;
    private GamePlayer gamePlayer;
    private QuerySDMecanimController aniController;
    private BoxCollider boxCollider;
    private InputData curPressedInput;

    public PlayerMoveState(GamePlayer player)
    {
        gamePlayer = player;
        //
        aniController = gamePlayer.controller.characterObject.queryMecanimController;
        boxCollider = gamePlayer.controller.characterObject.GetBoxCollider();
        //
        moveSpeed = 3.5f;
    }
    public void InitState()
    {
        aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_RUN);
    }

    public void ReleaseState()
    {
        curPressedInput.keyCode = KeyCode.None;
        curPressedInput.mobileInputType = MOBILE_INPUT_TYPE.NONE;
    }

    public void UpdateState()
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
            dir = gamePlayer.transform.TransformDirection(virtualJoystick.GetMoveDirection());
        }
        return dir;
    }
    private Vector3 CalcWindowMoveDir()
    {
        if (curPressedInput.keyCode == KeyCode.W)
        {
            return gamePlayer.transform.forward;
        }
        else if (curPressedInput.keyCode == KeyCode.S)
        {
            return -gamePlayer.transform.forward;
        }
        else if (curPressedInput.keyCode == KeyCode.D)
        {
            return gamePlayer.transform.right;
        }
        else if (curPressedInput.keyCode == KeyCode.A)
        {
            return -gamePlayer.transform.right;
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
            curPressedInput = InputManager.singleton.GetInputData();
            KojeomLogger.DebugLog(string.Format("curPressedKey : {0}", curPressedInput.keyCode), LOG_TYPE.USER_INPUT);
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
        Vector3 move = dir.normalized * moveSpeed;
        gamePlayer.controller.LerpPosition(move);
    }
}
