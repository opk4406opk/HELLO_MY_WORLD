using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KojeomDevelopTools : MonoBehaviour {
    private Rect mainWindowRect = new Rect(new Vector2(0, 100), new Vector2(512, 384));

    void OnGUI()
    {
        GUI.Window(0, mainWindowRect, (windowID)=> {

        }, "DevelopTools");
    }
}
