using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// 게임내 사용자(캐릭터)를 관리하는 클래스.
/// </summary>
public class GamePlayerManager : MonoBehaviour {

    public GamePlayer MyGamePlayer { get; private set; }
    public static GamePlayerManager Instance { get; private set; }
    public bool bInitialize { get; private set; }

    public void Init()
    {
        bInitialize = false;
        Instance = this;
    }

    public void Make(Vector3 initPosition, Action finishCallBack = null)
    {
        KojeomLogger.DebugLog(string.Format("[GamePlayerManager] Start Make"), LOG_TYPE.INFO);
        //
        var instance = Instantiate(GameResourceSupervisor.GetInstance().GamePlayerPrefab.LoadSynchro(), Vector3.zero, Quaternion.identity);
        MyGamePlayer = instance.GetComponent<GamePlayer>();
        MyGamePlayer.Initialize(GameLocalDataManager.GetInstance().CharacterType, GameLocalDataManager.GetInstance().CharacterName, initPosition);
        //Player Manager 하위 종속으로 변경.
        MyGamePlayer.transform.parent = gameObject.transform;
        // call back.
        finishCallBack?.Invoke();

        KojeomLogger.DebugLog(string.Format("[GamePlayerManager] Finish Make"), LOG_TYPE.INFO);
        bInitialize = true;
    }

    public bool IsValidPlayer()
    {
        return bInitialize == true;
    }
}
