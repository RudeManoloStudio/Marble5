using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{

    [SerializeField] private SoundData soundData;
    [SerializeField] AudioSource audioSource;


    void Start()
    {

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("NoPoseBille", _OnNoPoseBille);

    }

    void _OnUpdateScoreAndCoins(object noUse)
    {
        audioSource.PlayOneShot(soundData.UpdateScoreSound);
    }

    void _OnPoseBille()
    {
        audioSource.PlayOneShot(soundData.PoseBilleSound);
    }

    void _OnNoPoseBille()
    {
        audioSource.PlayOneShot(soundData.NoPoseBilleSound);
    }
}
