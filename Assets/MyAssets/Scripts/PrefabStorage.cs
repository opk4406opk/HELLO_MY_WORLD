using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임내에 사용되는 프리팹들을 저장하고 있는 클래스.
/// </summary>
public class PrefabStorage : MonoBehaviour {

    private static GameObject[] characterPrefabs;

    public static PrefabStorage instance = null;
    private PrefabStorage()
    {
        characterPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
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
