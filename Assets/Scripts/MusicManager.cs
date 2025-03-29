using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{

    private AudioSource audioSource;
    private List<AudioClip> playlist;
    private bool musicOn;
    private int currentTrackIndex = 0;

    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        musicOn = GameManager.Instance.MusicOn;

        playlist = GameManager.Instance.Playlist;

        if (playlist.Count > 0 && musicOn)
        {

            PlayNextClip();

        }

        EventManager.AddListener("ToggleMusic", _OnToggleMusic);

    }

    void Update()
    {
        if (!audioSource.isPlaying && musicOn)
        {
            PlayNextClip();
        }
    }

    void PlayNextClip()
    {

        audioSource.clip = playlist[currentTrackIndex];
        audioSource.Play();
        currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;

    }

    private void _OnToggleMusic()
    {

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.UnPause();
        }

        musicOn = !musicOn;

    }



    /*
    public void PreparePlaylist(List<AudioClip> playlist, bool musicOn)
    {

        //this.soundData = soundData;

        if (playlist.Count > 0)
        {
            //currentTrackIndex = Random.Range(0, soundData.Playlist.Count - 1);
            audioSource.clip = playlist[0];
            if (musicOn) PlayTrack();
        }
    }

    public void PlayTrack()
    {
        audioSource.Play();
    }

    public void Toggle()
    {

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }
    */
}
