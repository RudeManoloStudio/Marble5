using System;
using System.IO;
using UnityEngine;

[Serializable]
public class UserData
{
    public float fxVolume;
    public float musicVolume;
    public bool developerMode; // ← AJOUTER CETTE LIGNE

    public UserData()
    {
        fxVolume = 1f;
        musicVolume = 1f;
        developerMode = false; // ← AJOUTER CETTE LIGNE
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

    public void SavePreferences()
    {
        SaveUserData();
    }

    public void SaveFxVolume(float volume)
    {
        userData.fxVolume = volume;
        SaveUserData();
    }

    public void SaveMusicVolume(float volume)
    {
        userData.musicVolume = volume;
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
            SaveUserData();
        }
    }

    private void SaveUserData()
    {
        string json = JsonUtility.ToJson(userData);
        File.WriteAllText(filePath, json);
    }
    public void SaveDeveloperMode(bool isEnabled)
    {
        userData.developerMode = isEnabled;
        SaveUserData();
    }

    public bool GetDeveloperMode()
    {
        return userData.developerMode;
    }
}
