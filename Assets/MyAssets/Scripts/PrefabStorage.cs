using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 게임내에 사용되는 프리팹들을 저장하고 있는 클래스.
/// </summary>
public class PrefabStorage : MonoBehaviour {

    #region world
    public SoftObjectPtr WorldPrefab { get; private set; }
    public SoftObjectPtr CommonChunkPrefab { get; private set; }
    public SoftObjectPtr WaterChunkPrefab { get; private set; }
    #endregion

    #region Actor
    public SoftObjectPtrGroup[] ActorPrefabs { get; private set; } = new SoftObjectPtrGroup[(int)ACTOR_TYPE.COUNT];
    #endregion
    //
    //
    private readonly GameObject[] CharacterPrefabs;

    public static PrefabStorage Instance = null;
    private PrefabStorage()
    {
        CharacterPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        //
        CommonChunkPrefab = new SoftObjectPtr(ConstFilePath.COMMON_CHUNK_PREFAB);
        WorldPrefab = new SoftObjectPtr(ConstFilePath.SUB_WORLD_PREFAB);
        WaterChunkPrefab = new SoftObjectPtr(ConstFilePath.WATER_CHUNK_PREFAB);
        //
        for(int idx = 0; idx < (int)ACTOR_TYPE.COUNT; idx++)
        {
            ActorPrefabs[idx] = new SoftObjectPtrGroup();
        }

        ActorPrefabs[(int)ACTOR_TYPE.NPC].Group = new SoftObjectPtr[(int)NPC_TYPE.COUNT];
        var npcGuids = AssetDatabase.FindAssets("NPC", new[] { ConstFilePath.NPC_PREFABS });
        foreach (var guid in npcGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ActorPrefabs[(int)ACTOR_TYPE.NPC].Group[(int)KojeomUtility.GetActorDetailTypeFromAssetPath<NPC_TYPE>(path)] = new SoftObjectPtr(path);
        }

        ActorPrefabs[(int)ACTOR_TYPE.MONSTER].Group = new SoftObjectPtr[(int)MONSTER_TYPE.COUNT];
        var monsterGuids = AssetDatabase.FindAssets("MONSTER", new[] { ConstFilePath.NPC_PREFABS });
        foreach(var guid in monsterGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ActorPrefabs[(int)ACTOR_TYPE.MONSTER].Group[(int)KojeomUtility.GetActorDetailTypeFromAssetPath<MONSTER_TYPE>(path)] = new SoftObjectPtr(path);
        }
    }

    public static PrefabStorage GetInstance()
    {
        if (Instance == null) Instance = new PrefabStorage();
        return Instance;
    }

    public GameObject GetCharacterPrefab(int characterType)
    {
        return CharacterPrefabs[characterType];
    }
}
