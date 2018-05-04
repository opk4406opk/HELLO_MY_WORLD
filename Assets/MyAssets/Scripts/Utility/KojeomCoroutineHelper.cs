using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 코루틴이 선언된 클래스가 아닌 외부 오브젝트에서 실행시킬 수 있도록 해주는 helper 클래스.
/// </summary>
public class KojeomCoroutineHelper : MonoBehaviour {

    private static KojeomCoroutineHelper _singleton = null;
    public static KojeomCoroutineHelper singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("KojeomCoroutineHelper 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }
    public void Init()
    {
        _singleton = this;
    }

    public void StartCoroutineService(IEnumerator routine)
    {
        KojeomLogger.DebugLog(string.Format("[CoRoutine-Helper] routine start : {0}", routine.ToString()));
        StartCoroutine(routine);
    }
    
}
