using DG.Tweening;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelConfigurationSO levelConfiguration;
    private int score;
    private int hitCount;

    private void Start()
    {
        GameEvents.Current.OnAddScore += AddScore;
        GameEvents.Current.OnSendTimer += SaveTime;
        GameEvents.Current.OnGotHit += GotHit;
    }

    private void OnDisable()
    {
        GameEvents.Current.OnAddScore -= AddScore;
        GameEvents.Current.OnSendTimer += SaveTime;
        GameEvents.Current.OnGotHit -= GotHit;
    }

    private void AddScore(int scorePoints)
    {
        score += scorePoints;
        if (score >= levelConfiguration.CoinValueToComplete)
        {
            score = levelConfiguration.CoinValueToComplete;
            NextLevelInitialization();
        }

        GameEvents.Current.UpdateUIScore(score);
    }
    
    private void NextLevelInitialization()
    {
        Sequence sequence = DOTween.Sequence();

        sequence
            .AppendCallback(() => GameEvents.Current.NextLevel())
            .AppendCallback(() => GameEvents.Current.FadeLoader(1f, 1f))
            .AppendInterval(1f)
            .AppendCallback(() => GameEvents.Current.GenerateMap())
            .AppendCallback(() => GameEvents.Current.SetStartPlayerPosition())
            .AppendCallback(() => score = hitCount = 0)
            .AppendInterval(1f)
            .AppendCallback(() => GameEvents.Current.ActiveTimer(true))
            .AppendCallback(() => GameEvents.Current.FadeLoader(0f, 1f));
    }

    private void SaveTime(float time)
    {
        TimeSaverJSON.SaveTime(time);
    }

    private void GotHit()
    {
        hitCount++;
        GameEvents.Current.UpdateHitCounterUI(hitCount);
    }
}