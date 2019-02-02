using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임내에 사용되는 프리팹들을 저장하고 있는 클래스.
/// </summary>
public class PrefabStorage : MonoBehaviour {

    public SoftGameObjectPtr WorldPrefab { get; private set; }
    public SoftGameObjectPtr CommonChunkPrefab { get; private set; }
    public SoftGameObjectPtr WaterChunkPrefab { get; private set; }
    //
    //
    private static GameObject[] CharacterPrefabs;

    public static PrefabStorage instance = null;
    private PrefabStorage()
    {
        CharacterPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        //
        CommonChunkPrefab = new SoftGameObjectPtr
        {
            ObjectPath = ConstFilePath.COMMON_CHUNK_PREFAB
        };
        WorldPrefab = new SoftGameObjectPtr
        {
            ObjectPath = ConstFilePath.SUB_WORLD_PREFAB
        };
        WaterChunkPrefab = new SoftGameObjectPtr
        {
            ObjectPath = ConstFilePath.WATER_CHUNK_PREFAB
        };
    }

    public static PrefabStorage GetInstance()
    {
        if (instance == null) instance = new PrefabStorage();
        return instance;
    }

    public GameObject GetCharacterPrefab(int characterType)
    {
        return CharacterPrefabs[characterType];
    }
}
