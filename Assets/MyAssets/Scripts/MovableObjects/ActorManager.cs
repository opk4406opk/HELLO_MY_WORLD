using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런타임중에 게임내에 생성되는 모든 Actor들을 관리/스포닝을 하는 클래스.
/// </summary>
public class ActorManager : MonoBehaviour
{
    public NPCManager NPCManagerInstance { get; private set; }

    private static ActorManager _Instance = null;
    public static ActorManager Instance
    {
        get
        {
            if (_Instance == null) KojeomLogger.DebugLog("ActorManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _Instance;
        }
    }
    public void Init()
    {
        // to do
        _Instance = this;
        NPCManagerInstance = gameObject.GetComponentInChildren<NPCManager>();
        NPCManagerInstance.Init();
    }

    public void Begin()
    {
        //
        StartCoroutine(Tick());
    }


    private IEnumerator Tick()
    {
        KojeomLogger.DebugLog("ActorManager::Tick Start.", LOG_TYPE.NORMAL);
        while(true)
        {
            yield return null;
        }
        KojeomLogger.DebugLog("ActorManager::Tick End.", LOG_TYPE.NORMAL);
    }

}
