using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KojeomDevelopTools : MonoBehaviour {
    [SerializeField]
    private Font font;
    private bool isLogging;
    private static GUIStyle guiStyle;
    private static KojeomDevelopTools instance;
    public static KojeomDevelopTools GetInstance()
    {
        return instance;
    }
    private Rect mainWindowRect = new Rect(new Vector2(10, 100), new Vector2(1100, 512));
    private void Start()
    {
        isLogging = true;
        guiStyle = new GUIStyle();
        guiStyle.font = font;
        guiStyle.richText = true;
        guiStyle.stretchWidth = true;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 612, 128, 64), "LOGGING_ON") == true)
        {
            isLogging = true;
        }
        if (GUI.Button(new Rect(140, 612, 128, 64), "LOGGING_OFF") == true)
        {
            isLogging = false;
        }
        GUI.Window(0, mainWindowRect, (windowID)=> {
            if (isLogging)
            {
                var logs = KojeomLogger.GetGUIDebugLogs();
                GUI.Label(new Rect(13, 30, 1050, 512), logs, guiStyle);
            }
        }, "DevelopTools");
    }
}
