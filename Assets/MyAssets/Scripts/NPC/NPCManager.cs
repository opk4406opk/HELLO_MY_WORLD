using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {
    [SerializeField]
    private NPCDataFile npcDataFile;
    [SerializeField]
    private GameManager gameMangager;

    [SerializeField]
    private GameObject prefab_roamingMerchantNPC;
    [SerializeField]
    private GameObject prefab_shopMerchantNPC;

    private List<Actor> npcs = new List<Actor>();

    public void Init()
    {
        // to do
    }

    public void GenerateNPC()
    {
        foreach(var data in npcDataFile.roamingMerchantDatas)
        {
            GameObject npc = Instantiate(prefab_roamingMerchantNPC, data.spwanPos, Quaternion.identity);
            npc.GetComponent<RoamingMerchant>().Init(data.spwanPos, gameMangager.worldList[data.spwanWorld]);
            npc.GetComponent<RoamingMerchant>().textMeshController.Init(GameConfig.inGameFontSize);
            npc.GetComponent<RoamingMerchant>().textMeshController.SetText(data.name);
            npcs.Add(npc.GetComponent<RoamingMerchant>());
        }
        
    }
	
}
