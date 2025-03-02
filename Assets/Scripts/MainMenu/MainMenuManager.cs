using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup loaderCanvasGroup;
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject SettingsPanel;
    private void Start()
    { 
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
        loaderCanvasGroup.alpha = 1;
        loaderCanvasGroup.DOFade(0f, 1f);
    }

    public void LoadScene(string sceneName)
    {
        loaderCanvasGroup.DOFade(1f, 0.5f);
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSettingsMenu()
    {
        StartCoroutine(ShowOrhideSettingsMenuCourutine(true));
    }

    public void BackToMainMenu()
    {
        StartCoroutine(ShowOrhideSettingsMenuCourutine(false));
    }
    
    private IEnumerator ShowOrhideSettingsMenuCourutine(bool active)
    {
        yield return loaderCanvasGroup.DOFade(1f, 0.5f).WaitForCompletion();
        SettingsPanel.SetActive(active);
        MainMenuPanel.SetActive(!active);
        yield return loaderCanvasGroup.DOFade(0f, 0.5f);
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
