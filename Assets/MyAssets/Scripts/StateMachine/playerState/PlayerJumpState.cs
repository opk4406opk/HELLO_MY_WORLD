using UnityEngine;

public class PlayerJumpState : IState
{
    private float jumpScale;
    private float jumpSpeed;
    private GamePlayer gamePlayer;
    private QuerySDMecanimController aniController;

    private float t;

    public PlayerJumpState(GamePlayer player)
    {
        gamePlayer = player;
        jumpSpeed = 2.5f;
        jumpScale = 1.8f;
        t = 0.0f;
        aniController = gamePlayer.GetController().characterObject.GetQueryMecanimController();
    }
    public void InitState()
    {
        aniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_FLY_UP);
        KojeomCoroutineHelper.singleton.StartCoroutineService(Jump(), "Jump");
    }

    public void ReleaseState()
    {
    }

    public void UpdateState()
    {
    }

    private System.Collections.IEnumerator Jump()
    {
        //
        Vector3 startPos = gamePlayer.transform.position;
        Vector3 destPos = startPos + (gamePlayer.transform.up * jumpScale);
        while (t <= 1.0f)
        {
            World containWorld = WorldManager.instance.ContainedWorld(gamePlayer.transform.position);
            Vector3 topOffsetedPos = gamePlayer.transform.position;
            topOffsetedPos += new Vector3(0.0f, 0.1f, 0.0f);

            CollideInfo collideInfo = containWorld.customOctree.Collide(topOffsetedPos);
            if (collideInfo.isCollide == false)
            {
                startPos = gamePlayer.transform.position;
                destPos.x = startPos.x;
                destPos.z = startPos.z;

                Vector3 newPos = Vector3.Lerp(startPos, destPos, t);
                gamePlayer.transform.position = newPos;
                t += (Time.deltaTime * jumpSpeed);
            }
            yield return null;
        }
        t = 0.0f;
    }
}
