using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private CanvasGroup loaderCanvasGroup;

    private void Start()
    {
        loaderCanvasGroup.alpha = 1;
        loaderCanvasGroup.DOFade(0f, 1f);
    }

    public void LoadScene(string sceneName)
    {
        loaderCanvasGroup.DOFade(1f, 0.5f);
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        yield return new WaitForSeconds(0.5f);
        
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            if (asyncOperation.progress >= 0.9f)
                asyncOperation.allowSceneActivation = true;
            yield return null;
        }
    }
}
