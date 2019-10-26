using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/*   게임에서 사용되는 액터 프리팹 이름 규칙. ( = Naming Rule)
 * 
 *   NPC, Monster..etc preafb -> [ActorTypc]_[DetailType]_[ResourceID]_[Name]
 *   
 *   ex) MONSTER_Cyclopes_RID(Cyclopes)_Cyclopes
 *      (대분류)(소분류)(리소스)(이름)
 *      대분류 : ACTOR_TYPE
 *      소분류 : MONSTER_TYPE
 *      리소스 : 소분류 Enum 값과 동일하다. 
 *      이름 : 실제 이름 ( 인게임에 등장할 때 사용되는..)
*/

public class ActorTypeGroup
{
    public ActorResourceGroup[] ActorResourceGroups;
}

public class ActorResourceGroup
{
    public Dictionary<string, SoftObjectPtr> Resoruces = new Dictionary<string, SoftObjectPtr>();
}


/*  파티클 이펙트 프리팹 이름 규칙.
 *  
 *  [Category]_[Name]
 */

public class ParticleEffectCategoryContainer
{
    public ParticleEffectResourceGroup[] PartiecleEffectResources;
}

public class ParticleEffectResourceGroup
{
    public Dictionary<string, SoftObjectPtr> Resources = new Dictionary<string, SoftObjectPtr>();
}

/// <summary>
/// 게임내에 전반적으로 사용되는 프리팹들을 저장하고 있는 클래스.
/// (스킬, 버프 제외)
/// </summary>
public class GameResourceSupervisor
{

    #region world
    public SoftObjectPtr WorldAreaPrefab { get; private set; }
    public SoftObjectPtr SubWorldPrefab { get; private set; }
    public SoftObjectPtr TerrainChunkPrefab { get; private set; }
    public SoftObjectPtr WaterChunkPrefab { get; private set; }
    public SoftObjectPtr EnviromentChunkPrefab { get; private set; }
    #endregion

    #region Actor
    public ActorTypeGroup[] ActorPrefabs { get; private set; } = new ActorTypeGroup[(int)ACTOR_TYPE.COUNT];
    #endregion

    #region Player
    public SoftObjectPtr GamePlayerPrefab { get; private set; }
    public SoftObjectPtr GamePlayerCameraPrefab { get; private set; }
    #endregion

    #region ParticleSystem
    public ParticleEffectCategoryContainer[] ParticleEffectPrefabs { get; private set; } = new ParticleEffectCategoryContainer[(int)GameParticeEffectCategory.COUNT];
    #endregion
    //
    //
    private readonly GameObject[] CharacterPrefabs;

    private static GameResourceSupervisor Instance = null;
    private GameResourceSupervisor()
    {
        GamePlayerPrefab = new SoftObjectPtr(ConstFilePath.GAME_NET_PLAYER_PREFAB_RESOURCE_PATH);
        GamePlayerCameraPrefab = new SoftObjectPtr(ConstFilePath.PLAYER_CAMERA_PREFAB);
        //
        CharacterPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER_RESOURCE_PATH);
        //
        WorldAreaPrefab = new SoftObjectPtr(ConstFilePath.WORLD_AREA_PREFAB);
        TerrainChunkPrefab = new SoftObjectPtr(ConstFilePath.TERRAIN_CHUNK_PREFAB_RESOURCE_PATH);
        SubWorldPrefab = new SoftObjectPtr(ConstFilePath.SUB_WORLD_PREFAB);
        WaterChunkPrefab = new SoftObjectPtr(ConstFilePath.WATER_CHUNK_PREFAB_RESOURCE_PATH);
        EnviromentChunkPrefab = new SoftObjectPtr(ConstFilePath.ENVIROMENT_CHUNK_PREFAB_RESOURCE_PATH);

        // 게임 파티클.
        for(int idx = 0; idx < (int)GameParticeEffectCategory.COUNT; idx++)
        {
            ParticleEffectPrefabs[idx] = new ParticleEffectCategoryContainer();
        }

        // 액터.
        for (int idx = 0; idx < (int)ACTOR_TYPE.COUNT; idx++)
        {
            ActorPrefabs[idx] = new ActorTypeGroup();
        }
        ActorPrefabs[(int)ACTOR_TYPE.NPC].ActorResourceGroups = new ActorResourceGroup[(int)NPC_TYPE.COUNT];
        var npcGuids = AssetDatabase.FindAssets("NPC", new[] { ConstFilePath.NPC_PREFABS_ASSET_PATH });
        foreach (var guid in npcGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            NPC_TYPE type = KojeomUtility.GetActorDetailTypeFromAssetPath<NPC_TYPE>(path);
            string resourceID = KojeomUtility.GetResourceIDFromAssetPath(path);
            string resourcePath = KojeomUtility.ConvertAssetPathToResourcePath(path);
            if (ActorPrefabs[(int)ACTOR_TYPE.NPC].ActorResourceGroups[(int)type] == null)
            {
                ActorPrefabs[(int)ACTOR_TYPE.NPC].ActorResourceGroups[(int)type] = new ActorResourceGroup();
            }
            ActorPrefabs[(int)ACTOR_TYPE.NPC].ActorResourceGroups[(int)type].Resoruces.Add(resourceID, new SoftObjectPtr(resourcePath));
        }

        ActorPrefabs[(int)ACTOR_TYPE.MONSTER].ActorResourceGroups = new ActorResourceGroup[(int)MONSTER_TYPE.COUNT];
        var monsterGuids = AssetDatabase.FindAssets("MONSTER", new[] { ConstFilePath.MONSTER_PREFABS_ASSET_PATH });
        foreach (var guid in monsterGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            MONSTER_TYPE type = KojeomUtility.GetActorDetailTypeFromAssetPath<MONSTER_TYPE>(path);
            string resourceID = KojeomUtility.GetResourceIDFromAssetPath(path);
            string resourcePath = KojeomUtility.ConvertAssetPathToResourcePath(path);
            if (ActorPrefabs[(int)ACTOR_TYPE.MONSTER].ActorResourceGroups[(int)type] == null)
            {
                ActorPrefabs[(int)ACTOR_TYPE.MONSTER].ActorResourceGroups[(int)type] = new ActorResourceGroup();
            }
            ActorPrefabs[(int)ACTOR_TYPE.MONSTER].ActorResourceGroups[(int)type].Resoruces.Add(resourceID, new SoftObjectPtr(resourcePath));
        }

        ActorPrefabs[(int)ACTOR_TYPE.ANIMAL].ActorResourceGroups = new ActorResourceGroup[(int)ANIMAL_TYPE.COUNT];
        var animalGuids = AssetDatabase.FindAssets("ANIMAL", new[] { ConstFilePath.ANIMAL_PREFABS_ASSET_PATH });
        foreach (var guid in animalGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ANIMAL_TYPE type = KojeomUtility.GetActorDetailTypeFromAssetPath<ANIMAL_TYPE>(path);
            string resourceID = KojeomUtility.GetResourceIDFromAssetPath(path);
            string resourcePath = KojeomUtility.ConvertAssetPathToResourcePath(path);
            if (ActorPrefabs[(int)ACTOR_TYPE.ANIMAL].ActorResourceGroups[(int)type] == null)
            {
                ActorPrefabs[(int)ACTOR_TYPE.ANIMAL].ActorResourceGroups[(int)type] = new ActorResourceGroup();
            }
            ActorPrefabs[(int)ACTOR_TYPE.ANIMAL].ActorResourceGroups[(int)type].Resoruces.Add(resourceID, new SoftObjectPtr(resourcePath));
        }

    }

    public static GameResourceSupervisor GetInstance()
    {
        if (Instance == null)
        {
            Instance = new GameResourceSupervisor();
        }
        return Instance;
    }

    public GameObject GetCharacterPrefab(int characterType)
    {
        return CharacterPrefabs[characterType];
    }
}
