using System;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    [SerializeField] private TextMeshProUGUI ScoreText;

    private void Awake()
    {
        if(instance == null) instance = this;
    }

    public void UpdateScore(int score)
    {
        ScoreText.text = score.ToString();
    }
}
