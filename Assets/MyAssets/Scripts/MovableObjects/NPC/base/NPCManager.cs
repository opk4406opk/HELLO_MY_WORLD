using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : NPCSpawner
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

    public override void RandomSpawn()
    { 
        var gameConfig = GameConfigDataFile.singleton.GetGameConfigData();
        foreach (var data in NPCDataFile.Instance.NpcSpawnDatas)
        {
            
        }
    }

    public override void Spawn(NPC_TYPE npcType, World world, int num, bool initShow = false)
    {
        Actor instance = Instantiate(PrefabStorage.Instance.ActorPrefabs[(int)ACTOR_TYPE.NPC].Group[(int)npcType].LoadSynchro(), Vector3.zero, Quaternion.identity).GetComponent<Actor>();
        //
        NPCSpawnData spawnData;
        NPCDataFile.Instance.NpcSpawnDatas.TryGetValue(npcType, out spawnData);
        switch(npcType)
        {
            case NPC_TYPE.MERCHANT:
            case NPC_TYPE.GUARD:
                instance.Init(spawnData, world);
                break;
        }
    }
}
