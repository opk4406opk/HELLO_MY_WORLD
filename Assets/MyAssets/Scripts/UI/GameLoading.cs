using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

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
