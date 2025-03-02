using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private CanvasGroup LoaderCanvasGroup;
    [SerializeField] private TextMeshProUGUI ScoreText;

    private void Awake()
    {
        ScoreText.text = "0/10";
        LoaderCanvasGroup.alpha = 1f;
        LoaderCanvasGroup.DOFade(0f, 1f);
        if(instance == null) instance = this;
    }

    public void UpdateScore(int score)
    {
        ScoreText.text = score.ToString() + "/10";
    }

    public IEnumerator FadeLoader(float alphaValue, float duration)
    {
        LoaderCanvasGroup.DOFade(alphaValue, duration);
        yield return new WaitForSeconds(duration);
    }
}
