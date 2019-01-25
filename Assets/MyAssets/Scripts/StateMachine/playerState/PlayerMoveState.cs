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

        World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);   
        if(containWorld == null)
        {
            return;
        }

        Vector3 origin = gamePlayer.controller.characterObject.transform.position;
        CustomAABB last = gamePlayer.controller.characterObject.GetCustomAABB();
        gamePlayer.controller.LerpPosition(move);
        //
        CustomAABB playerAABB = gamePlayer.controller.characterObject.GetCustomAABB(move);
        var collideInfo = containWorld.customOctree.Collide(playerAABB);
        if (collideInfo.isCollide)
        {
            float dist = Vector3.Distance(playerAABB.position, collideInfo.aabb.position);
            float between = (playerAABB.width + collideInfo.aabb.width) / 2;
            if (dist < between)
            {
                gamePlayer.controller.SetPosition(new Vector3(origin.x + (dist - between) / 3,
                    origin.y, origin.z + (dist - between) / 3));
            }
            else
            {
                float normalX = 0.0f, normalY = 0.0f, normalZ = 0.0f;
                float collisionTime = CustomAABB.SweptAABB(playerAABB, collideInfo.aabb,
                         ref normalX, ref normalY, ref normalZ);
                //KojeomLogger.DebugLog(string.Format("coll time : {0}", collisionTime), LOG_TYPE.DEBUG_TEST);
                if (collisionTime < 1.0f)
                {
                    Vector3 sliding = new Vector3(playerAABB.vx * normalZ, 0.0f, playerAABB.vz * normalX);
                    gamePlayer.controller.LerpPosition(sliding);
                }
            }
        }
    }
}
