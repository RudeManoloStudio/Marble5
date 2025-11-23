//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class FXManager : MonoBehaviour
{

    [SerializeField] private Slider fxSlider;
    [SerializeField] private AudioSource fxSource;

    private FXData fxData;


    void Start()
    {
        fxSource = GetComponent<AudioSource>();

        // Récupère les données depuis GameManager via une méthode Setup
        Setup(GameManager.Instance.FxVolume, GameManager.Instance.FxData);

        // Abonne-toi à l'événement de changement de valeur
        fxSlider.onValueChanged.AddListener(SetVolume);

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("NoPoseBille", _OnNoPoseBille);
    }

    public void Setup(float volume, FXData data)
    {
        // Initialise le slider avec le volume actuel
        fxSlider.value = volume;
        fxSource.volume = volume;

        fxData = data;
    }

    void SetVolume(float volume)
    {
        fxSource.volume = volume;
        EventManager.TriggerEvent("FxVolumeChanged", volume);
    }

    void _OnUpdateScoreAndCoins(object noUse)
    {
        fxSource.PlayOneShot(fxData.UpdateScoreSound);
    }

    void _OnPoseBille(object noUse)
    {
        fxSource.PlayOneShot(fxData.PoseBilleSound);
    }

    void _OnNoPoseBille()
    {
        fxSource.PlayOneShot(fxData.NoPoseBilleSound);
    }
}
