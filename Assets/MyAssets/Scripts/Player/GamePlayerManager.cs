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
            if(WorldAreaManager.Instance != null)
            {
                foreach(var worldArea in WorldAreaManager.Instance.WorldAreas)
                {
                    foreach (var state in worldArea.Value.SubWorldStates)
                    {
                        if (state.Value.RealTimeStatus == SubWorldRealTimeStatus.LoadFinish)
                        {
                            Vector3 initialPosition = state.Value.SubWorldInstance.RandomRealPositionAtSurface();
                            //
                            var instance = Instantiate(GameResourceSupervisor.GetInstance().GamePlayerPrefab.LoadSynchro(), Vector3.zero, Quaternion.identity);
                            MyGamePlayer = instance.GetComponent<GamePlayer>();
                            MyGamePlayer.Initialize(GameLocalDataManager.GetInstance().CharacterType,
                                GameLocalDataManager.GetInstance().CharacterName, initialPosition);
                            //Player Manager 하위 종속으로 변경.
                            MyGamePlayer.transform.parent = gameObject.transform;
                            //
                            IsInitializeFinish = true;
                            break;
                        }
                        yield return null;
                    }
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
