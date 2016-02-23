using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// 메인화면 -> InGame 씬으로의 전환을 관리하는 클래스.
/// </summary>
public class GameLoading : MonoBehaviour {

    [SerializeField]
    private UIProgressBar progressBar;
    private AsyncOperation asyncOp;

    void Start()
    {
        StartCoroutine(GameLoadProcess());
    }

    void Update()
    {
        progressBar.value = asyncOp.progress;
    }
    
    private IEnumerator GameLoadProcess()
    {
        asyncOp = SceneManager.LoadSceneAsync("InGame");
        yield return asyncOp;
    }
}
