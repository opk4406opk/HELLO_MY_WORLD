using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// scene 사이의 데이터 통신을 위한 클래스.
/// </summary>
public class SceneToScene_Data : MonoBehaviour {

    private Dictionary<string, string> _gameChDatas;
    public Dictionary<string, string> gameChDatas
    {
        get { return _gameChDatas; }
    }
    
    public void SetData(string key, string value)
    {
        _gameChDatas.Add(key, value);
    }

    public void ClearElement()
    {
        _gameChDatas.Clear();
    }

    void Start()
    {
        _gameChDatas = new Dictionary<string, string>();
        DontDestroyOnLoad(gameObject);
    }
}
