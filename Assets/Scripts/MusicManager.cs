using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private SoundData soundData;
    //private bool musicOn;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PreparePlaylist(SoundData soundData, bool musicOn)
    {

        this.soundData = soundData;

        if (soundData.Playlist.Count > 0)
        {
            currentTrackIndex = Random.Range(0, soundData.Playlist.Count - 1);
            audioSource.clip = soundData.Playlist[currentTrackIndex];
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
}
