using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private int score = 0;
    [SerializeField] private PlayerController player;
    [SerializeField] private MapGenerator mapGenerator;

    private void Awake()
    { 
        if (Instance == null) Instance = this;
    }

    public void addScore(int scorePoints)
    {
        if (score >= 10)
        {
            player.transform.position = player.startPosition;
            mapGenerator.GenerateMap();
        }
            
        
        score += scorePoints;
        UIManager.instance.UpdateScore(score);
    }
}
