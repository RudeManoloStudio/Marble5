using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{

    private AudioSource audioSource;
    private int currentTrackIndex = 0;
    private SoundData soundData;
    //private bool musicOn = true;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        //soundData = GameManager.Instance.Sounds;

        /*
        if (soundData.Playlist.Count > 0)
        {
            currentTrackIndex = Random.Range(0, soundData.Playlist.Count - 1);

            if (musicOn) PlayTrack(currentTrackIndex);
        }
        */
    }

    public void PlayPlaylist(SoundData soundData)
    {

        this.soundData = soundData;

        if (soundData.Playlist.Count > 0)
        {
            currentTrackIndex = Random.Range(0, soundData.Playlist.Count - 1);

            PlayTrack(currentTrackIndex);
            //if (musicOn) PlayTrack(currentTrackIndex);
        }
    }

    /*
    void Update()
    {

        if (!musicOn)
        {
            audioSource.Pause();
            return;
        }

        if (musicOn)
        {

            audioSource.UnPause();

            if (!audioSource.isPlaying)
            {
                currentTrackIndex = (currentTrackIndex + 1) % soundData.Playlist.Count;
                PlayTrack(currentTrackIndex);
            }
        }
    }
    */

    public void PlayTrack(int index)
    {

        audioSource.clip = soundData.Playlist[index];
        audioSource.Play();
    }

    public void TogglePlayPause()
    {

        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
        /*
        if (musicOn)
        {
            musicOn = false;
        }
        else
        {
            musicOn = true;
        }
        */
    }
}
