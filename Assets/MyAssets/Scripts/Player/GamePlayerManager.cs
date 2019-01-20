using System.Collections;
using UnityEngine;
/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class GamePlayerManager : MonoBehaviour {

    public GamePlayer myGamePlayer;
    public static GamePlayerManager instance;
    private GameObject gamePlayerPrefab;
    public bool isInitializeFinish { get; private set; }

    public void Init()
    {
        isInitializeFinish = false;
        StartCoroutine(InitProcess());
        instance = this;
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

    private IEnumerator InitProcess()
    {
        KojeomLogger.DebugLog(string.Format("[GamePlayerManager] Start InitProcess"), LOG_TYPE.INFO);
        while(true)
        {
            if(WorldManager.instance != null)
            {
                foreach(var state in WorldManager.instance.wholeWorldStates)
                {
                    if(state.Value.genInfo == WorldGenerateInfo.Done &&
                        P2PNetworkManager.GetInstance().GetMyGamePlayer() != null)
                    {
                        gamePlayerPrefab = P2PNetworkManager.GetInstance().playerPrefab;
                        myGamePlayer = P2PNetworkManager.GetInstance().GetMyGamePlayer();
                        myGamePlayer.GetController().Init(Camera.main, myGamePlayer);
                        myGamePlayer.GetController().StartControllProcess();

                        Vector3 worldInstPos = state.Value.subWorldInstance.GetRealCoordPosition();
                        myGamePlayer.GetController().SetPosition(new Vector3(worldInstPos.x, 60.0f, worldInstPos.z));
                        //Player Manager 하위 종속으로 변경.
                        myGamePlayer.transform.parent = gameObject.transform;
                        //
                        isInitializeFinish = true;
                        break;
                    }
                    yield return null;
                }
                if(isInitializeFinish == true)
                {
                    break;
                }
            }
            yield return null;
        }
       
        KojeomLogger.DebugLog(string.Format("[GamePlayerManager] Finish InitProcess"), LOG_TYPE.INFO);
    }
}
