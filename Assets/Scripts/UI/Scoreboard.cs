using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Scoreboard : MonoBehaviour
{
    [SerializeField] private Record record;
    [SerializeField] private Transform recordHolder;
    [SerializeField] private int amountOfRecords;

    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "TimeRecords.json");
        
        DisplayRecords();
    }

    public void DisplayRecords()
    {
        foreach (Transform child in recordHolder)
        {
            Destroy(child.gameObject);
        }

        if (!File.Exists(filePath))
        {
            gameObject.SetActive(false);
            return;
        }

        string json = File.ReadAllText(filePath);
        TimerResults timerResults = JsonUtility.FromJson<TimerResults>(json);

        if (timerResults == null || timerResults.results == null || timerResults.results.Count == 0)
        {
            gameObject.SetActive(false);
            return;
        }

        List<TimerResult> sortedResults = timerResults.results.OrderBy(r => r.time).ToList();
        int recordsToDisplay = Mathf.Min(amountOfRecords, sortedResults.Count);

        for (int i = 0; i < recordsToDisplay; i++)
        {
            TimerResult result = sortedResults[i];
            Record recordItem = Instantiate(record,
                Vector3.zero,
                quaternion.identity,
                recordHolder);
            
            recordItem.RecordText.text = FormatTime(result.time);
        }
    }

    private string FormatTime(float timeToDisplay)
    {
        int minutes = Mathf.FloorToInt(timeToDisplay / 60);
        int seconds = Mathf.FloorToInt(timeToDisplay % 60);
        int milliseconds = Mathf.FloorToInt((timeToDisplay * 100) % 100);
        return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}