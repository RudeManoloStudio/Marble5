using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SoundData : ScriptableObject
{
    public AudioClip UpdateScoreSound;
    public AudioClip PoseBilleSound;
    public AudioClip NoPoseBilleSound;
    public List<AudioClip> Playlist;
}
