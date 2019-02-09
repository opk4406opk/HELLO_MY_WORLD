using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MovableObjectSpawner
{
    public List<Actor> NPCGroup { get; private set; } = new List<Actor>();
    private Actor LastestClickedActor;

    private static NPCManager _Singleton = null;
    public static NPCManager Singleton
    {
        get
        {
            if (_Singleton == null) KojeomLogger.DebugLog("NPCManager 제대로 초기화 되지 않았습니다", LOG_TYPE.ERROR);
            return _Singleton;
        }
    }
    public override void Init()
    {
        // to do
        _Singleton = this;
    }

    /// <summary>
    /// 가장 최근에 선택한 Actor를 가져온다.
    /// </summary>
    /// <returns></returns>
    public Actor GetLastestClickedActor()
    {
        return LastestClickedActor;
    }

    private void OnClickedActor(Actor actor)
    {
        KojeomLogger.DebugLog(string.Format("OnCliked Actor name : {0}, type : {1}", actor.name, actor.GetActorType()));
        LastestClickedActor = actor;
    }

    public override void Spawn()
    { 
        // 돌아다니는 상인 NPC 생성.
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        foreach (var data in NPCDataFile.Instance.NpcSpawnDatas)
        {
            //GameObject npc = Instantiate(prefab_roamingMerchantNPC, data.spawnPos, Quaternion.identity);
            //var roamingMerchant = npc.GetComponent<RoamingMerchant>();
            //roamingMerchant.Init(data.spawnPos,
            //    WorldManager.instance.wholeWorldStates[data.WorldUniqueID].subWorldInstance,
            //    ACTOR_TYPE.NPC);
            //roamingMerchant.sellingItemIDs = data.sellingItemsID;
            //roamingMerchant.textMeshController.Init(gameConfig.ingame_font_size);
            //roamingMerchant.textMeshController.SetText(data.name);
            //roamingMerchant.OnClickedActor += OnClickedActor;
            //npcs.Add(roamingMerchant);
        }
    }
}
