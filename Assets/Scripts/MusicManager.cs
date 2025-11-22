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
        musicSource.volume = initialVolume;

        playlist = GameManager.Instance.Playlist;

        if (playlist.Count > 0)
        {

            PlayNextClip();

        }

        // Abonne-toi � l'�v�nement de changement de valeur
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

