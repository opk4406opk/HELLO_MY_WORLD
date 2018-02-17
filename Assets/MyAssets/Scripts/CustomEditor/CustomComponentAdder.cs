using System.Collections;
using System.Collections.Generic;
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
                PlayerController controller = element.GetComponent<PlayerController>();
                element.GetComponent<GameCharacter>().SetController(controller);
            }
            else
            {
                KojeomLogger.DebugLog("이미 GameCharacter 컴포넌트가 붙어있습니다. 컨트롤러만 할당합니다.");
                PlayerController controller = element.GetComponent<PlayerController>();
                element.GetComponent<GameCharacter>().SetController(controller);
            }
        }
        KojeomLogger.DebugLog("GameCharacter 컴포넌트 할당 작업 완료.");
    }
}
