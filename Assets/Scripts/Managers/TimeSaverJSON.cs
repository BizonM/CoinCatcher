using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class TimeSaverJSON
{
    private static readonly string filePath = Path.Combine(Application.persistentDataPath, "TimeRecords.json");

    public static void SaveTime(float time)
    {
        TimerResults timerResults = new TimerResults();

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            timerResults = JsonUtility.FromJson<TimerResults>(json);
        }

        TimerResult newResult = new TimerResult { time = time };
        timerResults.results.Add(newResult);

        string updatedJson = JsonUtility.ToJson(timerResults, true);
        File.WriteAllText(filePath, updatedJson);
    }
}

[Serializable]
public class TimerResult
{
    public float time;
}

[Serializable]
public class TimerResults
{
    public List<TimerResult> results = new List<TimerResult>();
}