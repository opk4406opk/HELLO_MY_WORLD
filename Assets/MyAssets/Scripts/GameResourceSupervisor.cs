using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/*   게임에서 사용되는 프리팹 이름 규칙. ( = Naming Rule)
 * 
 *   NPC, Monster..etc preafb -> [ActorTypc]_[DetailType]_[ResourceID]_[Name]
*/

/// <summary>
/// 게임내에 사용되는 프리팹들을 저장하고 있는 클래스.
/// </summary>
public class GameResourceSupervisor : MonoBehaviour {

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

    public static GameResourceSupervisor Instance = null;
    private GameResourceSupervisor()
    {
        CharacterPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        //
        CommonChunkPrefab = new SoftObjectPtr(ConstFilePath.COMMON_CHUNK_PREFAB_RESOURCE_PATH);
        WorldPrefab = new SoftObjectPtr(ConstFilePath.SUB_WORLD_PREFAB);
        WaterChunkPrefab = new SoftObjectPtr(ConstFilePath.WATER_CHUNK_PREFAB_RESOURCE_PATH);
        //
        for(int idx = 0; idx < (int)ACTOR_TYPE.COUNT; idx++)
        {
            ActorPrefabs[idx] = new SoftObjectPtrGroup();
        }

        ActorPrefabs[(int)ACTOR_TYPE.NPC].Group = new SoftObjectPtr[(int)NPC_TYPE.COUNT];
        var npcGuids = AssetDatabase.FindAssets("NPC", new[] { ConstFilePath.NPC_PREFABS_ASSET_PATH });
        foreach (var guid in npcGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ActorPrefabs[(int)ACTOR_TYPE.NPC].Group[KojeomUtility.GetResourceNumberFromAssetPath(path)] = new SoftObjectPtr(KojeomUtility.ConvertAssetPathToResourcePath(path));
            KojeomUtility.GetResourceNumberFromAssetPath(path);
        }

        ActorPrefabs[(int)ACTOR_TYPE.MONSTER].Group = new SoftObjectPtr[(int)MONSTER_TYPE.COUNT];
        var monsterGuids = AssetDatabase.FindAssets("MONSTER", new[] { ConstFilePath.NPC_PREFABS_ASSET_PATH });
        foreach(var guid in monsterGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ActorPrefabs[(int)ACTOR_TYPE.MONSTER].Group[KojeomUtility.GetResourceNumberFromAssetPath(path)] = new SoftObjectPtr(KojeomUtility.ConvertAssetPathToResourcePath(path));
        }
    }

    public static GameResourceSupervisor GetInstance()
    {
        if (Instance == null) Instance = new GameResourceSupervisor();
        return Instance;
    }

    public GameObject GetCharacterPrefab(int characterType)
    {
        return CharacterPrefabs[characterType];
    }
}
