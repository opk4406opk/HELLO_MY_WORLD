using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour {
    [SerializeField]
    private GameObject prefab_roamingMerchantNPC;
    [SerializeField]
    private GameObject prefab_shopMerchantNPC;

    private List<Actor> _npcs = new List<Actor>();
    public List<Actor> npcs
    {
        get { return _npcs; }
    }
    private Actor lastestClickedActor;

    private static NPCManager _singleton = null;
    public static NPCManager singleton
    {
        get
        {
            if (_singleton == null) KojeomLogger.DebugLog("NPCManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _singleton;
        }
    }
    public void Init()
    {
        // to do
        _singleton = this;
    }

    public void GenerateNPC()
    {
        // 돌아다니는 상인 NPC 생성.
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        foreach(var data in NPCDataFile.instance.roamingMerchantDatas)
        {
            GameObject npc = Instantiate(prefab_roamingMerchantNPC, data.spawnPos, Quaternion.identity);
            var roamingMerchant = npc.GetComponent<RoamingMerchant>();
            roamingMerchant.Init(data.spawnPos,
                WorldManager.instance.worldList[data.spawnWorld],
                ACTOR_TYPE.NPC);
            roamingMerchant.sellingItemIDs = data.sellingItemsID;
            roamingMerchant.textMeshController.Init(gameConfig.ingame_font_size);
            roamingMerchant.textMeshController.SetText(data.name);
            roamingMerchant.OnClickedActor += OnClickedActor;
            _npcs.Add(roamingMerchant);
        }
    }

    /// <summary>
    /// 가장 최근에 선택한 Actor를 가져온다.
    /// </summary>
    /// <returns></returns>
    public Actor GetLastestClickedActor()
    {
        return lastestClickedActor;
    }

    private void OnClickedActor(Actor actor)
    {
        KojeomLogger.DebugLog(string.Format("OnCliked Actor name : {0}, type : {1}", actor.name, actor.GetActorType()));
        lastestClickedActor = actor;
    }
}
