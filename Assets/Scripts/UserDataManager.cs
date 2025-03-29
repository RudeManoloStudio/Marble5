using System;
using System.IO;
using UnityEngine;

[Serializable]
public class UserData
{   
    public bool fxOn;
    public bool musicOn;

    public UserData()
    {   
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

    public void SavePreferences()
    {
        SaveUserData();
    }

    public void ToggleFX(bool state)
    {
        //userData.fxOn = true ? userData.fxOn = false : userData.fxOn = true;
        userData.fxOn = state;
        SaveUserData();
    }

    public void ToggleMusic(bool state)
    {
        userData.musicOn = state;
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
}
