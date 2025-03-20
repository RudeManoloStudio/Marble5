using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private SoundData soundData;
    [SerializeField] AudioSource audioSource;

    private bool soundOn = true;


    void Start()
    {

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("NoPoseBille", _OnNoPoseBille);

    }

    void _OnUpdateScoreAndCoins(object noUse)
    {
        if (soundOn) audioSource.PlayOneShot(soundData.UpdateScoreSound);
    }

    void _OnPoseBille(object noUse)
    {
        if (soundOn) audioSource.PlayOneShot(soundData.PoseBilleSound);
    }

    void _OnNoPoseBille()
    {
        if (soundOn) audioSource.PlayOneShot(soundData.NoPoseBilleSound);
    }

    public void ToggleFXSound()
    {
        if (soundOn)
        {
            soundOn = false;
        }
        else
        {
            soundOn = true;
        }
    }
}
