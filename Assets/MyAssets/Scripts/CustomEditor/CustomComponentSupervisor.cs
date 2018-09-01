#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public class CustomComponentSupervisor : EditorWindow
{
    [MenuItem("CustomEditor/CustomComponentSupervisor")]
    static void Init()
    {
        CustomComponentSupervisor window = (CustomComponentSupervisor)EditorWindow.GetWindow(typeof(CustomComponentSupervisor));
        window.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginToggleGroup("Func", true);
        if (GUILayout.Button("Add GameCharacter to CharPrefabs")) AddGameCharComponentToCharPrefabs();
        if (GUILayout.Button("Add BoxCollider to CharPrefabs")) AddBoxColliderToCharPerfabs();
        if (GUILayout.Button("Add CharacterController to CharPrefabs")) AddCharControllerToCharPrefabs();
        EditorGUILayout.EndToggleGroup();
    }

    private void AddGameCharComponentToCharPrefabs()
    {
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        foreach(var charPrefab in charPrefabs)
        {
            if(charPrefab.GetComponent<GameCharacter>() == null)
            {
                var gameCharacterComponent = charPrefab.AddComponent<GameCharacter>();
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
                coll.size = new Vector3(0.3f, 0.7f, 0.3f);
                coll.center = new Vector3(0.0f, 0.5f, 0.0f);
            }
            else
            {
                KojeomLogger.DebugLog("이미 BoxCollider 컴포넌트가 붙어있습니다.");
            }
        }
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 완료.");
    }

    private void AddCharControllerToCharPrefabs()
    {
        KojeomLogger.DebugLog("CharacterController 컴포넌트 할당 작업 시작.");
        GameObject[] charPrefabs = Resources.LoadAll<GameObject>(ConstFilePath.PREFAB_CHARACTER);
        foreach (var element in charPrefabs)
        {
            if (element.GetComponent<CharacterController>() == null)
            {
                var charController = element.AddComponent<CharacterController>();
            }
            else
            {
                KojeomLogger.DebugLog("이미 CharacterController 컴포넌트가 붙어있습니다.");
            }
        }
        KojeomLogger.DebugLog("CharacterController 컴포넌트 할당 작업 완료.");
    }
}
#endif