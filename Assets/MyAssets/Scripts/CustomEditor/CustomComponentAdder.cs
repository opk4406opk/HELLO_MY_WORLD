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
}
#endif