using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private SoundData soundData;
    [SerializeField] AudioSource audioSource;


    void Start()
    {

        EventManager.AddListener("UpdateScore", _OnUpdateScore);
        EventManager.AddListener("PoseBille", _OnPoseBille);

    }

    void _OnUpdateScore()
    {
        audioSource.PlayOneShot(soundData.UpdateScoreSound);
    }

    void _OnPoseBille()
    {
        audioSource.PlayOneShot(soundData.PoseBilleSound);
    }
}
