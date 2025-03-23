using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FXManager : MonoBehaviour
{

    //[SerializeField] private SoundData soundData;
    private AudioSource audioSource;

    private SoundData soundData;
    private bool fxOn = true;
    //private bool musicOn = true;


    void Start()
    {
        //soundData = GameManager.Instance.Sounds;

        audioSource = GetComponent<AudioSource>();

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("NoPoseBille", _OnNoPoseBille);
    }

    public void Setup(SoundData soundData)
    {
        this.soundData = soundData;
    }

    void _OnUpdateScoreAndCoins(object noUse)
    {
        if (fxOn) audioSource.PlayOneShot(soundData.UpdateScoreSound);
    }

    void _OnPoseBille(object noUse)
    {
        if (fxOn) audioSource.PlayOneShot(soundData.PoseBilleSound);
    }

    void _OnNoPoseBille()
    {
        if (fxOn) audioSource.PlayOneShot(soundData.NoPoseBilleSound);
    }

    public void ToggleFXSound()
    {
        if (fxOn)
        {
            fxOn = false;
        }
        else
        {
            fxOn = true;
        }
    }
}
