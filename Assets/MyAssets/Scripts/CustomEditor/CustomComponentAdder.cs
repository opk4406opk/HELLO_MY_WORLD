#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class CustomComponentAdder : EditorWindow
{
    [MenuItem("CustomEditor/CustomComponentAdder")]
    static void Init()
    {
        CustomComponentAdder window = (CustomComponentAdder)EditorWindow.GetWindow(typeof(CustomComponentAdder));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginToggleGroup("Func", true);
        if (GUILayout.Button("Add GameCharacter to CharPrefabs")) AddGameCharComponentToCharPrefabs();
        if (GUILayout.Button("Add BoxCollider to CharPrefabs")) AddBoxColliderToCharPerfabs();
        EditorGUILayout.EndToggleGroup();
    }

    private void AddGameCharComponentToCharPrefabs()
    {
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        foreach(var element in charPrefabs)
        {
            if(element.GetComponent<GameCharacter>() == null)
            {
                element.AddComponent<GameCharacter>();
            }
            else
            {
                KojeomLogger.DebugLog("이미 GameCharacter 컴포넌트가 붙어있습니다.");
            }
        }
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 완료.");
    }

    private void AddBoxColliderToCharPerfabs()
    {
        KojeomLogger.DebugLog("BoxCollider 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        foreach (var element in charPrefabs)
        {
            if (element.GetComponent<BoxCollider>() == null)
            {
                var coll = element.AddComponent<BoxCollider>();
                coll.size = new Vector3(0.5f, 1.0f, 0.5f);
                coll.center = new Vector3(0.0f, 0.5f, 0.0f);
            }
            else
            {
                KojeomLogger.DebugLog("이미 BoxCollider 컴포넌트가 붙어있습니다.");
            }
        }
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 완료.");
    }
}
#endif