using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents Current;

    private void Awake()
    {
        Current = this;
    }

    public event Action OnGenerateMap;
    public void GenerateMap()
    {
        OnGenerateMap?.Invoke();
    }

    public event Action<int> OnAddScore;
    public void AddScore(int value)
    {
        OnAddScore?.Invoke(value);
    }

    public event Action<int> OnUpdateUIScore;
    public void UpdateUIScore(int value)
    {
        OnUpdateUIScore?.Invoke(value);
    }

    public event Action<float, float> OnFadeLoader;
    public void FadeLoader(float endAlphaValue, float duration)
    {
        OnFadeLoader?.Invoke(endAlphaValue, duration);
    }

    public event Action OnSetPlayerPosition;
    public void SetStartPlayerPosition()
    {
        OnSetPlayerPosition?.Invoke();
    }

    public event Action OnNextLevel;
    public void NextLevel()
    {
        OnNextLevel?.Invoke();
    }

    public event Action<bool> OnActiveTimer;

    public void ActiveTimer(bool active)
    {
        OnActiveTimer?.Invoke(active);
    }

    public event Action<float> OnSendTimer;
    public void SendTimer(float time)
    {
        OnSendTimer?.Invoke(time);
    }

    public event Action OnGotHit;
    public void GotHit()
    {
        OnGotHit?.Invoke();
    }
    
    public event Action<int> OnUpdateHitCounterUI;
    public void UpdateHitCounterUI(int value)
    {
        OnUpdateHitCounterUI?.Invoke(value);
    }

    public event Action<bool> OnGameRunning;
    public void GameRunning(bool isRunning)
    {
        OnGameRunning?.Invoke(isRunning);
    }
}