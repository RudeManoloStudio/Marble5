using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private AudioSource musicSource;
    private List<AudioClip> playlist;
    private int currentTrackIndex = 0;

    void Start()
    {

        // Initialise le slider avec le volume actuel
        float initialVolume = GameManager.Instance.MusicVolume;
        volumeSlider.value = initialVolume;
        //volumeSlider.value = musicSource.volume;
        musicSource.volume = initialVolume;

        playlist = GameManager.Instance.Playlist;

        if (playlist.Count > 0)
        {

            PlayNextClip();

        }

        // Abonne-toi à l'événement de changement de valeur
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    void Update()
    {
        if (!musicSource.isPlaying)
        {
            PlayNextClip();
        }
    }

    void PlayNextClip()
    {

        musicSource.clip = playlist[currentTrackIndex];
        musicSource.Play();
        currentTrackIndex = (currentTrackIndex + 1) % playlist.Count;

    }
    void SetVolume(float volume)
    {
        musicSource.volume = volume;
        // Optionnel : sauvegarder le volume si besoin
        GameManager.Instance.SetMusicVolume(volume);
        //PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
    }
}




/*
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
//}

