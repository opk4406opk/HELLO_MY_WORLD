using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// scene 사이의 데이터 통신을 위한 클래스. ( For UI) 
/// </summary>
public class SceneToScene_Data : MonoBehaviour {

    private Dictionary<string, string> _gameChDatas = new Dictionary<string, string>();
    public Dictionary<string, string> gameChDatas
    {
        get { return _gameChDatas; }
    }

    private Dictionary<string, string> _gameInvenItemDatas = new Dictionary<string, string>();
    public Dictionary<string, string> gameInvenItemDatas
    {
        get { return _gameInvenItemDatas; }
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
}
