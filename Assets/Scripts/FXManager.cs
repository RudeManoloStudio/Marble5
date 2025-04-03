using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class FXManager : MonoBehaviour
{

    private AudioSource audioSource;

    private FXData fxData;
    private bool fxOn = true;


    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        fxOn = GameManager.Instance.FxOn;
        fxData = GameManager.Instance.FxData;

        EventManager.AddListener("ToggleFX", _OnToggleFx);
        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("NoPoseBille", _OnNoPoseBille);
    }

    void _OnUpdateScoreAndCoins(object noUse)
    {
        if (fxOn) audioSource.PlayOneShot(fxData.UpdateScoreSound);
    }

    void _OnPoseBille(object noUse)
    {
        if (fxOn) audioSource.PlayOneShot(fxData.PoseBilleSound);
    }

    void _OnNoPoseBille()
    {
        if (fxOn) audioSource.PlayOneShot(fxData.NoPoseBilleSound);
    }

    private void _OnToggleFx()
    {

        fxOn = fxOn == true ? false : true;

    }
}
