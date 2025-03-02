using System;
using System.Collections;
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
        score += scorePoints;
        if (score >= 10)
            StartCoroutine(NextLevelInitialization());
        else
            UIManager.instance.UpdateScore(score);   
    }

    private IEnumerator NextLevelInitialization()
    {
        UIManager.instance.UpdateScore(10);
        
        yield return StartCoroutine(UIManager.instance.FadeLoader(1f,1f));
        
        player.transform.position = player.startPosition;
        score = 0;
        UIManager.instance.UpdateScore(score);
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(mapGenerator.GenerateMap());
        yield return StartCoroutine(UIManager.instance.FadeLoader(0f,1f));
    }
}
