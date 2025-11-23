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
        // Récupère les données depuis GameManager via une méthode Setup
        Setup(GameManager.Instance.MusicVolume, GameManager.Instance.Playlist);

        // Abonne-toi à l'événement de changement de valeur
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    public void Setup(float volume, List<AudioClip> musicPlaylist)
    {
        // Initialise le slider avec le volume actuel
        volumeSlider.value = volume;
        musicSource.volume = volume;

        playlist = musicPlaylist;

        if (playlist.Count > 0)
        {
            PlayNextClip();
        }
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
        EventManager.TriggerEvent("MusicVolumeChanged", volume);
    }

    private void OnDestroy()
    {
        volumeSlider.onValueChanged.RemoveListener(SetVolume);
    }
}

