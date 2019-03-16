using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// deprecated.
[Obsolete("ActorCollideManager Deprecated. will be deleted soon.")]
public class ActorCollideManager : MonoBehaviour
{
    private static ActorCollideManager _singleton = null;
    public static ActorCollideManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("ActorCollideManager 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }
    public void Init()
    {
        _singleton = this;
        // to do
    }

    public bool IsNpcCollide(Ray ray)
    {
        return false;
    }
}
