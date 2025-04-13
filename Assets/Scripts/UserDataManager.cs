using System;
using System.IO;
using UnityEngine;

[Serializable]
public class UserData
{   
    //public bool fxOn;
    //public bool musicOn;
    public float fxVolume;
    public float musicVolume;

    public UserData()
    {
        //fxOn = true;
        //musicOn = true;
        fxVolume = 1f;
        musicVolume = 1f;
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

    /*
    public void ToggleFX(bool state)
    {
        //userData.fxOn = true ? userData.fxOn = false : userData.fxOn = true;
        userData.fxOn = state;
        SaveUserData();
    }
    */

    /*
    public void ToggleMusic(bool state)
    {
        //userData.musicOn = state;
        SaveUserData();
    }
    */

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
}
