using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup loaderCanvasGroup;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject settingsPanel;

    [Scene] [SerializeField] private int gameSceneToLoad;
    private void Start()
    {
        loaderCanvasGroup.alpha = 1;
        loaderCanvasGroup.DOFade(0f, 1f);
    }

    public void LoadGameScene()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(loaderCanvasGroup.DOFade(1f, 1f))
            .AppendCallback(() => SceneManager.LoadScene(gameSceneToLoad));
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSettingsMenu()
    {
        ShowOrhideSettingsMenu(true);
    }

    public void BackToMainMenu()
    {
        ShowOrhideSettingsMenu(false);
    }

    private void ShowOrhideSettingsMenu(bool active)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(loaderCanvasGroup.DOFade(1f, 0.5f))
            .AppendCallback(() => settingsPanel.SetActive(active))
            .AppendCallback(() => mainMenuPanel.SetActive(!active))
            .Append(loaderCanvasGroup.DOFade(0f, 0.5f));
    }

    public void RestartPanel(GameObject panel)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(loaderCanvasGroup.DOFade(1f, 0.5f))
            .AppendCallback(() => panel.SetActive(false))
            .AppendCallback(() => panel.SetActive(true))
            .Append(loaderCanvasGroup.DOFade(0f, 0.5f));
    }

    public void SetDefaultFont()
    {
        PlayerPrefs.SetString(PlayerPrefsEnum.Font.ToString(), PlayerPrefsEnum.FontDefault.ToString());
        PlayerPrefs.Save();
    }

    public void SetComicSansFont()
    {
        PlayerPrefs.SetString(PlayerPrefsEnum.Font.ToString(), PlayerPrefsEnum.FontComicSans.ToString());
        PlayerPrefs.Save();
    }
}
