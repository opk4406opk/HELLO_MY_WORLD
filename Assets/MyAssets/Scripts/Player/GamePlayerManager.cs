using UnityEngine;
/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class GamePlayerManager : MonoBehaviour {

    public GamePlayer myGamePlayer;
    public static GamePlayerManager instance;
    private GameObject gamePlayerPrefab;

    public void Init()
    {
        gamePlayerPrefab = P2PNetworkManager.GetInstance().playerPrefab;
        //
        myGamePlayer = P2PNetworkManager.GetInstance().GetMyGamePlayer();
        myGamePlayer.GetController().Init(Camera.main, myGamePlayer);
        //Player Manager 하위 종속으로 변경.
        myGamePlayer.transform.parent = gameObject.transform;
        //
        instance = this;
    }

    public static Vector3 GetGamePlayerInitPos()
    {
        //임시로 정해진 값.
        return new Vector3(0.0f, 60.0f, 0.0f);
    }
    /// <summary>
    /// 플레이어 컨트롤러를 시작합니다.
    /// </summary>
    public void StartController()
    {
        myGamePlayer.GetController().StartControllProcess();
    }
    /// <summary>
    /// 플레이어 컨트롤러를 중지합니다.
    /// </summary>
    public void StopController()
    {
        myGamePlayer.GetController().StopControllProcess();
    }

}
