using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HighscoreManager : MonoBehaviour
{
    private string filePath;
    private List<int> highscores;

    void Start()
    {
        filePath = Application.persistentDataPath + "/highscores.json";
        LoadHighscores();
    }

    public void AddHighscore(int score)
    {
        highscores.Add(score);
        highscores.Sort();
        highscores.Reverse();

        if (highscores.Count > 5)
        {
            highscores.RemoveAt(10);
        }

        SaveHighscores();
    }

    public List<int> GetHighscores()
    {
        return highscores;
    }

    private void LoadHighscores()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            highscores = JsonUtility.FromJson<List<int>>(json);
        }
        else
        {
            highscores = new List<int>();
        }
    }

    private void SaveHighscores()
    {
        string json = JsonUtility.ToJson(highscores);
        File.WriteAllText(filePath, json);
    }

    public String UpdateHighscoreText()
    {
        String text;
        text = "Highscores:\n" + string.Join("\n", highscores);

        return text;
    }
}
