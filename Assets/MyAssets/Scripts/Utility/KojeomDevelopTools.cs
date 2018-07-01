using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KojeomDevelopTools : MonoBehaviour {
    private static KojeomDevelopTools instance;
    public static KojeomDevelopTools GetInstance()
    {
        return instance;
    }
    private Rect mainWindowRect = new Rect(new Vector2(0, 100), new Vector2(512, 384));
    private void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnGUI()
    {
        GUI.Window(0, mainWindowRect, (windowID)=> {
            var logs = KojeomLogger.GetStackedLogs();
            GUI.Label(new Rect(0, 30, 256, 384), logs);
        }, "DevelopTools");
    }
}
