using UnityEngine;

public class PlayerJumpState : IState
{
    private float jumpScale;
    private GamePlayer gamePlayer;
    private QuerySDMecanimController aniController;

    private Vector3 destPos;
    private Vector3 originPos;

    private float t;

    public PlayerJumpState(GamePlayer player)
    {
        gamePlayer = player;
        jumpScale = 0.5f;
        t = 0.0f;
        aniController = gamePlayer.charInstance.GetAniController();
    }
    public void InitState()
    {
        aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_FLY_UP);
        originPos = gamePlayer.transform.position;
        destPos = originPos + (gamePlayer.transform.up * jumpScale);

        KojeomCoroutineHelper.singleton.StartCoroutineService(Jump());
    }

    public void ReleaseState()
    {
    }

    public void UpdateState()
    {
    }

    private System.Collections.IEnumerator Jump()
    {
        KojeomLogger.DebugLog("player jump start..");
        while (t <= 1.0f)
        {
            World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
            Vector3 topOffsetedPos = gamePlayer.transform.position;
            topOffsetedPos += new Vector3(0.0f, 0.1f, 0.0f);

            CollideInfo collideInfo = containWorld.customOctree.Collide(topOffsetedPos);
            if (collideInfo.isCollide == false)
            {
                Vector3 newPos = Vector3.Lerp(originPos, destPos, t);
                gamePlayer.transform.position = newPos;
                t += Time.deltaTime;
                KojeomLogger.DebugLog(string.Format("t : {0}", t));
            }
            yield return null;
        }
        t = 0.0f;
        KojeomLogger.DebugLog("player jump end..");
    }
}
