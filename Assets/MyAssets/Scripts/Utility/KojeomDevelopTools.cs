using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KojeomDevelopTools : MonoBehaviour {
    [SerializeField]
    private Font font;
    private bool isLogging;
    private static GUIStyle lableGUIStyle;
    private static GUIStyle groupGUIStyle;
    private static GUIContent boxGUIContent;
    private static KojeomDevelopTools instance;
    public static KojeomDevelopTools GetInstance()
    {
        return instance;
    }
    private Rect mainWindowRect = new Rect(new Vector2(10, 100), new Vector2(1100, 512));
    private Vector2 scrollPosition = Vector2.zero;
    private void Start()
    {
        isLogging = true;
        lableGUIStyle = new GUIStyle();
        lableGUIStyle.font = font;
        lableGUIStyle.richText = true;
        lableGUIStyle.stretchWidth = true;
        //
        groupGUIStyle = new GUIStyle();
        //
        boxGUIContent = new GUIContent();
        boxGUIContent.tooltip = "GroupBox...";

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnGUI()
    {
        GUI.BeginGroup(new Rect(5, 15, 1100, 1096), "GUI_DEBUG_VIEW_GROUP", groupGUIStyle);
        GUI.Box(new Rect(5, 15, 1100, 512), boxGUIContent);
        if (isLogging)
        {
            scrollPosition = GUI.BeginScrollView(new Rect(5, 15, 1095, 512), scrollPosition, new Rect(0, 0, 4096, 2048));
            GUI.Label(new Rect(5, 15, 1100, 512), KojeomLogger.GetGUIDebugLogs(), lableGUIStyle);
            GUI.EndScrollView();
        }
        if (GUI.Button(new Rect(10, 530, 128, 64), "LOGGING_ON") == true)
        {
            isLogging = true;
        }
        if (GUI.Button(new Rect(140, 530, 128, 64), "LOGGING_OFF") == true)
        {
            isLogging = false;
        }
        GUI.EndGroup();
       // //window 안에 버튼, 스크롤뷰를 넣으면 정상동작이 되지 않는다.
       // GUI.Window(0, mainWindowRect, (windowID) =>
       //{
          
       //}, "DevelopTools");
    }
}
