using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameUISupervisor : MonoBehaviour {
    [SerializeField]
    private Camera ingameUICamera;

    private static InGameUISupervisor _singleton = null;
    public static InGameUISupervisor singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("InGameUISupervisor 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }
    public void Init()
    {
        _singleton = this;
        KojeomLogger.DebugLog("InGameUISupervisor 초기화.");
    }

    public Camera GetIngameUICamera()
    {
        return ingameUICamera;
    }
}
