using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KojeomDevelopTools : MonoBehaviour {
    [SerializeField]
    private Font font;
    private static GUIStyle labelStyle;
    private static KojeomDevelopTools instance;
    public static KojeomDevelopTools GetInstance()
    {
        return instance;
    }
    private Rect mainWindowRect = new Rect(new Vector2(10, 100), new Vector2(1100, 512));
    private void Start()
    {
        labelStyle = new GUIStyle();
        labelStyle.font = font;
        labelStyle.richText = true;
        labelStyle.stretchWidth = true;
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnGUI()
    {
        GUI.Window(0, mainWindowRect, (windowID)=> {
            var logs = KojeomLogger.GetGUIDebugLogs();
            GUI.Label(new Rect(13, 30, 1050, 512), logs, labelStyle);
        }, "DevelopTools");
    }
}
