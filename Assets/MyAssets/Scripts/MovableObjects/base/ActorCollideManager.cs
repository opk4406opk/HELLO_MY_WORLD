using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if(NPCManager.Singleton != null && NPCManager.Singleton.NPCGroup != null)
        {
            foreach (var npc in NPCManager.Singleton.NPCGroup)
            {
                if (CustomRayCast.InterSectWithBOX(ray, npc.GetController().GetMinExtent(),
                    npc.GetController().GetMaxExtent()))
                {
                    ((INpc)npc).Talk();
                    return true;
                }
            }
        }
        return false;
    }
}
