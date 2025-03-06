using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private LevelConfigurationSO levelConfiguration;
    
    [SerializeField] private CanvasGroup loaderCanvasGroup;
    [SerializeField] private CanvasGroup gameMenuCanvasGroup;
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI hitsCounterText;

    [SerializeField] private GameObject gameMenu;
    [Scene] [SerializeField] private int mainMenuScene;

    private float elapsedTime;
    private bool isRunning = true;

    private void Start()
    {
        GameEvents.Current.OnUpdateUIScore += UpdateScore;
        GameEvents.Current.OnFadeLoader += FadeLoader;
        GameEvents.Current.OnActiveTimer += ActiveTimer;
        GameEvents.Current.OnNextLevel += NextLevelSetup;
        GameEvents.Current.OnUpdateHitCounterUI += UpdateHitCounter;
        
        UpdateHitCounter(0);
        UpdateScore(0);
        
        loaderCanvasGroup.alpha = 1f;
        loaderCanvasGroup.DOFade(0f, 1f);
    }

    private void OnDisable()
    {
        GameEvents.Current.OnUpdateUIScore -= UpdateScore;
        GameEvents.Current.OnFadeLoader -= FadeLoader;
        GameEvents.Current.OnActiveTimer -= ActiveTimer;
        GameEvents.Current.OnNextLevel -= NextLevelSetup;
        GameEvents.Current.OnUpdateHitCounterUI -= UpdateHitCounter;
    }
    
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            ShowOrHideGameMenu(true);
        }
        
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            DisplayTime(elapsedTime);
        }
    }

    private void NextLevelSetup()
    {
        Sequence sequence = DOTween.Sequence();

        sequence
            .AppendCallback(() => ActiveTimer(false))
            .AppendCallback(() => GameEvents.Current.SendTimer(elapsedTime))
            .AppendInterval(1f)
            .AppendCallback(() => elapsedTime = 0f)
            .AppendCallback(() => UpdateScore(0))
            .AppendCallback(() => UpdateHitCounter(0));
    }

    public void ShowOrHideGameMenu(bool show)
    {
        Sequence sequence = DOTween.Sequence();
        
        if (show)
        {
            sequence
                .AppendCallback(() => GameEvents.Current.GameRunning(false))
                .AppendCallback(() => ActiveTimer(false))
                .AppendCallback(() => gameMenu.SetActive(true))
                .AppendCallback(() => gameMenuCanvasGroup.DOFade(1f, 1f))
                .Append(loaderCanvasGroup.DOFade(1f, 1f));
        }
        else
        {
            sequence
                .AppendCallback(() => gameMenuCanvasGroup.DOFade(0f, 1f))
                .Append(loaderCanvasGroup.DOFade(0f, 1f))
                .AppendCallback(() => GameEvents.Current.GameRunning(true))
                .AppendCallback(() => ActiveTimer(true))
                .AppendCallback(() => gameMenu.SetActive(false));
        }
    }

    public void LoadMainMenuScene()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(gameMenuCanvasGroup.DOFade(0f, 1f))
            .AppendCallback(() => SceneManager.LoadScene(mainMenuScene));
    }

    private void DisplayTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        int milliseconds = Mathf.FloorToInt((timeToDisplay * 100) % 100);
        timerText.text = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }

    private void ActiveTimer(bool active)
    {
        isRunning = active;
    }

    private void UpdateScore(int score)
    {
        scoreText.text = $"{score} / {levelConfiguration.CoinValueToComplete}";
    }

    private void FadeLoader(float alphaValue, float duration)
    {
        loaderCanvasGroup.DOFade(alphaValue, duration);
    }

    private void UpdateHitCounter(int hitCount)
    {
        hitsCounterText.text = hitCount.ToString();
    }
}