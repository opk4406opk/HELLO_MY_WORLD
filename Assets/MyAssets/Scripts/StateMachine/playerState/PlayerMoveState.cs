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
        aniController = gamePlayer.charInstance.GetAniController();
        boxCollider = gamePlayer.charInstance.GetBoxCollider();
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
            // 여기서 TransformDirection 하는 이유??
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

    ///// <summary>
    ///// 클라이언트로부터 서버로 호출되는 RPC 함수.
    ///// Commands are sent from player objects on the client to player objects on the server.
    ///// </summary>
    ///// <param name="clientConnectionID"></param>
    //[Command]
    //private void CmdMoveEvent(int clientConnectionID)
    //{
    //    KojeomLogger.DebugLog(string.Format("[RPC_CALL] Move from connectionID : {0}", clientConnectionID), LOG_TYPE.NETWORK_SERVER_INFO);
    //}

    private void Move()
    {
        if(InputManager.singleton != null)
        {
            curPressedInput = InputManager.singleton.GetInputData();
            KojeomLogger.DebugLog(string.Format("curPressedKey : {0}", curPressedInput.keyCode), LOG_TYPE.USER_INPUT);
        }
        Vector3 dir = Vector3.zero, newPos = Vector3.zero;
        Vector3 originPos = gamePlayer.transform.position;

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
        GameNetworkManager.GetInstance().PushCharStateMessage(GAMEPLAYER_CHAR_STATE.MOVE);
        //
        KojeomLogger.DebugLog("player moving..", LOG_TYPE.USER_INPUT);
        newPos = gamePlayer.transform.position + (dir * moveSpeed * Time.deltaTime);
        gamePlayer.transform.position = newPos;

        World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
        var collideInfo = containWorld.customOctree.Collide(gamePlayer.charInstance.GetCustomAABB());
        if (collideInfo.isCollide)
        {
            KojeomLogger.DebugLog(string.Format("player가 이동하는 위치에 Block (pos : {0})이 존재합니다. 이동하지 않습니다.",
                collideInfo.hitBlockCenter), LOG_TYPE.USER_INPUT);
            gamePlayer.transform.position = originPos;
        }
    }
}
