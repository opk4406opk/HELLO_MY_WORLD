using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextMeshController : MonoBehaviour {

    [SerializeField]
    private TextMesh textMesh;
    public void Init(int fontSize)
    {
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.fontSize = fontSize;
    }

    public void SetText(string text)
    {
        textMesh.text = text;
    }

}
