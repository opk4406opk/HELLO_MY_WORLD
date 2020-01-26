using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

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
 *  FX_[Category]_[Type]
 */

public class ParticleEffectCategoryContainer
{
    public Dictionary<GameParticleType, SoftObjectPtr> Resources = new Dictionary<GameParticleType, SoftObjectPtr>();
}

public enum AssetPathType
{
    Fx,
    Monster,
    Animal,
    Npc
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

#if UNITY_EDITOR
        var particleGuids = AssetDatabase.FindAssets("FX", new[] { ConstFilePath.GAME_FX_ASSET_PATH });
        MakeAssetPaths(particleGuids, AssetPathType.Fx);
        //
        var npcGuids = AssetDatabase.FindAssets("NPC", new[] { ConstFilePath.NPC_PREFABS_ASSET_PATH });
        MakeAssetPaths(npcGuids, AssetPathType.Npc);
        //
        var monsterGuids = AssetDatabase.FindAssets("MONSTER", new[] { ConstFilePath.MONSTER_PREFABS_ASSET_PATH });
        MakeAssetPaths(monsterGuids, AssetPathType.Monster);
        //
        var animalGuids = AssetDatabase.FindAssets("ANIMAL", new[] { ConstFilePath.ANIMAL_PREFABS_ASSET_PATH });
        MakeAssetPaths(animalGuids, AssetPathType.Animal);
#endif

        // 게임 파티클.
        foreach (var path in GetAssetPaths(AssetPathType.Fx))
        {
            GameParticeEffectCategory category = KojeomUtility.GetParticleCategoryFromAssetPath(path);
            GameParticleType type = KojeomUtility.GetParticleTypeFromAssetPath(path);
            string resorucePath = KojeomUtility.ConvertAssetPathToResourcePath(path);
            if(ParticleEffectPrefabs[(int)category] == null)
            {
                ParticleEffectPrefabs[(int)category] = new ParticleEffectCategoryContainer();
            }
            ParticleEffectPrefabs[(int)category].Resources.Add(type, new SoftObjectPtr(resorucePath));
        }

        // 액터 그룹 할당.
        for (int idx = 0; idx < (int)ACTOR_TYPE.COUNT; idx++)
        {
            ActorPrefabs[idx] = new ActorTypeGroup();
        }
        //
        ActorPrefabs[(int)ACTOR_TYPE.NPC].ActorResourceGroups = new ActorResourceGroup[(int)NPC_TYPE.COUNT];
        foreach (var path in GetAssetPaths(AssetPathType.Npc))
        {
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
        foreach (var path in GetAssetPaths(AssetPathType.Monster))
        {
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
        foreach (var path in GetAssetPaths(AssetPathType.Animal))
        {
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

    private List<string> GetAssetPaths(AssetPathType pathType)
    {
        string filePath = "";
        switch (pathType)
        {
            case AssetPathType.Fx:
                filePath = ConstFilePath.FX_ASSET_LIST_FILE_PATH;
                break;
            case AssetPathType.Monster:
                filePath = ConstFilePath.MONSTER_ASSET_LIST_FILE_PATH;
                break;
            case AssetPathType.Animal:
                filePath = ConstFilePath.ANIMAL_ASSET_LIST_FILE_PATH;
                break;
            case AssetPathType.Npc:
                filePath = ConstFilePath.NPC_ASSET_LIST_FILE_PATH;
                break;
        }
        List<string> assetsPath = new List<string>();
        // deserialize JSON directly from a file
        using (StreamReader file = new StreamReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
        {
            JsonSerializer serializer = new JsonSerializer();
            assetsPath = (List<string>)serializer.Deserialize(file, typeof(List<string>));
        }
        return assetsPath;
    }
#if UNITY_EDITOR
    private void MakeAssetPaths(string[] guids, AssetPathType pathType)
    {
        List<string> assetsPath = new List<string>();
        foreach(var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            assetsPath.Add(path);
        }
        //
        // serialize JSON directly to a file
        string filePath = "";
        switch (pathType)
        {
            case AssetPathType.Fx:
                filePath = ConstFilePath.FX_ASSET_LIST_FILE_PATH;
                break;
            case AssetPathType.Monster:
                filePath = ConstFilePath.MONSTER_ASSET_LIST_FILE_PATH;
                break;
            case AssetPathType.Animal:
                filePath = ConstFilePath.ANIMAL_ASSET_LIST_FILE_PATH;
                break;
            case AssetPathType.Npc:
                filePath = ConstFilePath.NPC_ASSET_LIST_FILE_PATH;
                break;
        }
        using (StreamWriter file = new StreamWriter(File.Open(filePath, FileMode.OpenOrCreate, FileAccess.Write)))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Formatting = Formatting.Indented;
            serializer.Serialize(file, assetsPath);
        }
    }
#endif
}
