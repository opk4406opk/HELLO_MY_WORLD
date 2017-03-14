using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCollideManager : MonoBehaviour {
    [SerializeField]
    private NPCManager npcManager;

    public void Init()
    {
        // to do
    }

    public bool IsNpcCollide(Ray ray)
    {
        foreach (var npc in npcManager.npcs)
        {
            if (CustomRayCast.InterSectWithBOX(ray, npc.GetActorController().GetMinExtent(),
                npc.GetActorController().GetMaxExtent()))
            {
                ((INpc)npc).Talk();
                return true;
            }
        }
        return false;
    }

}
