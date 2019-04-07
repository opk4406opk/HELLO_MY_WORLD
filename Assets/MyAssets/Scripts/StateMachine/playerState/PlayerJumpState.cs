using UnityEngine;

public class PlayerJumpState : APlayerState, IState
{
    private float JumpScale;
    private float JumpSpeed;
   
    private QuerySDMecanimController AniController;

    private float t;

    public PlayerJumpState(GamePlayer player)
    {
        GamePlayer = player;
        JumpSpeed = 2.5f;
        JumpScale = 1.8f;
        t = 0.0f;
        AniController = GamePlayer.Controller.CharacterInstance.QueryMecanimController;
    }
    public void InitState()
    {
        AniController.ChangeAnimation(QuerySDMecanimController.QueryChanSDAnimationType.NORMAL_FLY_UP);
        KojeomCoroutineHelper.singleton.StartCoroutineService(Jump(), "Jump");
    }

    public void ReleaseState()
    {
    }

    public void UpdateState(float deltaTime)
    {
    }

    private System.Collections.IEnumerator Jump()
    {
        //
        Vector3 startPos = GamePlayer.Controller.CharacterInstance.transform.position;
        Vector3 destPos = startPos + (GamePlayer.Controller.CharacterInstance.transform.up * JumpScale);
        while (t <= 1.0f)
        {
            World containWorld = WorldManager.Instance.ContainedWorld(GamePlayer.Controller.CharacterInstance.transform.position);
            Vector3 topOffsetedPos = GamePlayer.Controller.CharacterInstance.transform.position;
            topOffsetedPos += new Vector3(0.0f, 0.1f, 0.0f);

            CollideInfo collideInfo = containWorld.CustomOctreeInstance.Collide(topOffsetedPos);
            if (collideInfo.IsCollide == false)
            {
                startPos = GamePlayer.Controller.CharacterInstance.transform.position;
                destPos.x = startPos.x;
                destPos.z = startPos.z;

                Vector3 newPos = Vector3.Lerp(startPos, destPos, t);
                GamePlayer.Controller.CharacterInstance.transform.position = newPos;
                t += (Time.deltaTime * JumpSpeed);
            }
            yield return null;
        }
        t = 0.0f;
    }
}
