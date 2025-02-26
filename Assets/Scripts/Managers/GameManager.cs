using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int score = 0;

    private void Awake()
    { 
        if (Instance == null) Instance = this;
    }

    public void addScore(int scorePoints)
    {
        score += scorePoints;
        UIManager.instance.UpdateScore(score);
    }
}
