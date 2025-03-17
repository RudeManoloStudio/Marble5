using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class HighscoreData
{
    public List<int> highscores;

    public HighscoreData()
    {
        highscores = new List<int>();
    }
}

public class HighscoreManager
{
    private string filePath;
    private HighscoreData highscoreData;

    public HighscoreManager(string filePath)
    {
        this.filePath = filePath;
        LoadHighscores();
    }

    public void AddHighscore(int score)
    {
        highscoreData.highscores.Add(score);
        highscoreData.highscores.Sort();
        highscoreData.highscores.Reverse();

        if (highscoreData.highscores.Count > 5)
        {
            highscoreData.highscores.RemoveAt(5);
        }

        SaveHighscores();
    }

    public List<int> GetHighscores()
    {
        return highscoreData.highscores;
    }

    private void LoadHighscores()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            highscoreData = JsonUtility.FromJson<HighscoreData>(json);
        }
        else
        {
            highscoreData = new HighscoreData();
        }
    }

    private void SaveHighscores()
    {
        string json = JsonUtility.ToJson(highscoreData);
        File.WriteAllText(filePath, json);
    }
}
