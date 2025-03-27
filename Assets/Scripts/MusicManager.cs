using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private SoundData soundData;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayPlaylist(SoundData soundData)
    {

        this.soundData = soundData;

        if (soundData.Playlist.Count > 0)
        {
            currentTrackIndex = Random.Range(0, soundData.Playlist.Count - 1);

            PlayTrack(currentTrackIndex);
        }
    }

    public void PlayTrack(int index)
    {

        audioSource.clip = soundData.Playlist[index];
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
