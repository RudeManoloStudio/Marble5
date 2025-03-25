using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class UserData
{
    public Dictionary<int, int> scores;
    public bool fxOn;
    public bool musicOn;

    public UserData()
    {
        scores = new Dictionary<int, int>();
        fxOn = true;
        musicOn = true;
    }
}

public class UserDataManager
{
    private string filePath;
    private UserData userData;

    public UserDataManager(string filePath)
    {
        this.filePath = filePath;
        LoadUserData();
    }

    public void AddHighscore(int level, int score)
    {
        userData.scores.Add(level, score);
        SaveUserData();
    }

    public void ToggleFX()
    {
        userData.fxOn = (true) ? userData.fxOn = false : userData.fxOn = true;
        SaveUserData();
    }

    public void ToggleMusic()
    {
        userData.musicOn = (true) ? userData.musicOn = false : userData.musicOn = true;
        SaveUserData();
    }

    public UserData GetUserData()
    {
        return userData;
    }

    private void LoadUserData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            userData = JsonUtility.FromJson<UserData>(json);

        }
        else
        {
            userData = new UserData();
        }
    }

    private void SaveUserData()
    {
        string json = JsonUtility.ToJson(userData);
        File.WriteAllText(filePath, json);
    }
}
