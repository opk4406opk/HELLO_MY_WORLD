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
        aniController = gamePlayer.charInstance.GetQueryMecanimController();
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
        Vector3 speed = dir.normalized * moveSpeed;

        World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
        if(containWorld == null)
        {
            return;
        }
        CustomAABB playerAABB = gamePlayer.charInstance.GetCustomAABB(speed);
        var collideInfo = containWorld.customOctree.Collide(gamePlayer.charInstance.GetCustomAABB());
        if (collideInfo.isCollide)
        {
            KojeomLogger.DebugLog(string.Format("Player collision with Block(AABB) x : {0}, y : {1}, z : {2}, type : {3}",
                collideInfo.GetBlock().centerX, collideInfo.GetBlock().centerY,
                collideInfo.GetBlock().centerZ, (BlockTileType)collideInfo.GetBlock().type), LOG_TYPE.DEBUG_TEST);

            float normalFaceX = 0.0f, normalFaceY = 0.0f, normalFaceZ = 0.0f;
            float collisionTime = CustomAABB.SweptAABB(playerAABB, collideInfo.aabb, ref normalFaceX, ref normalFaceY, ref normalFaceZ);
            float remainTime = 1.0f - collisionTime;
            Vector3 slide = new Vector3(playerAABB.vx * remainTime * normalFaceZ, 0.0f, playerAABB.vz * remainTime * normalFaceX);
            KojeomLogger.DebugLog(string.Format("collisionTime :{0}, slidePos : {1}, normalFaceX : {2}, normalFaceY : {3}, normalFaceZ : {4}",
                collisionTime, slide, normalFaceX, normalFaceY, normalFaceZ), LOG_TYPE.DEBUG_TEST);
            if(collisionTime < 1.0f)
            {
                gamePlayer.GetController().LerpPosition(slide);
            }
        }
        else
        {
            gamePlayer.GetController().LerpPosition(speed);
        }
    }
}
