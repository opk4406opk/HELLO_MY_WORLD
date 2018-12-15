using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임내에 사용되는 프리팹들을 저장하고 있는 클래스.
/// </summary>
public class PrefabStorage : MonoBehaviour {

    public GameObject subWorldPrefab { get; private set; }
    public GameObject commonChunkPrefab { get; private set; }
    public GameObject waterChunkPrefab { get; private set; }
    private static GameObject[] characterPrefabs;

    public static PrefabStorage instance = null;
    private PrefabStorage()
    {
        characterPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        //
        commonChunkPrefab = Resources.Load<GameObject>(ConstFilePath.COMMON_CHUNK_PREFAB);
        waterChunkPrefab = Resources.Load<GameObject>(ConstFilePath.WATER_CHUNK_PREFAB);
        subWorldPrefab = Resources.Load<GameObject>(ConstFilePath.SUB_WORLD_PREFAB);
    }

    public static PrefabStorage GetInstance()
    {
        if (instance == null) instance = new PrefabStorage();
        return instance;
    }

    public GameObject GetCharacterPrefab(int characterType)
    {
        return characterPrefabs[characterType];
    }
}
