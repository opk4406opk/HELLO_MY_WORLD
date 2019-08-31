using System.Collections;
using UnityEngine;
/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class GamePlayerManager : MonoBehaviour {

    public GamePlayer MyGamePlayer { get; private set; }
    public static GamePlayerManager Instance { get; private set; }
    public bool IsInitializeFinish { get; private set; }

    public void Init()
    {
        IsInitializeFinish = false;
        StartCoroutine(InitProcess());
        Instance = this;
    }
  
    private IEnumerator InitProcess()
    {
        KojeomLogger.DebugLog(string.Format("[GamePlayerManager] Start InitProcess"), LOG_TYPE.INFO);
        while(true)
        {
            if(WorldManager.Instance != null)
            {
                foreach(var state in WorldManager.Instance.WholeWorldStates)
                {
                    if(state.Value.RealTimeStatus == WorldRealTimeStatus.LoadFinish)
                    {
                        Vector3 worldInstPos = state.Value.SubWorldInstance.WorldOffsetCoordinate;
                        //
                        var instance = Instantiate(GameResourceSupervisor.GetInstance().GamePlayerPrefab.LoadSynchro(), Vector3.zero, Quaternion.identity);
                        MyGamePlayer = instance.GetComponent<GamePlayer>();
                        MyGamePlayer.Initialize(GameLocalDataManager.GetInstance().CharacterType,
                            GameLocalDataManager.GetInstance().CharacterName,
                            new Vector3(worldInstPos.x, 60.0f, worldInstPos.z));
                        //Player Manager 하위 종속으로 변경.
                        MyGamePlayer.transform.parent = gameObject.transform;
                        //
                        IsInitializeFinish = true;
                        break;
                    }
                    yield return null;
                }
                if(IsInitializeFinish == true)
                {
                    break;
                }
            }
            yield return null;
        }
       
        KojeomLogger.DebugLog(string.Format("[GamePlayerManager] Finish InitProcess"), LOG_TYPE.INFO);
    }

   
}
