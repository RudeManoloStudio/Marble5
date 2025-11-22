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

        fxData = GameManager.Instance.FxData;

        // Initialise le slider avec le volume actuel
        float initialVolume = GameManager.Instance.FxVolume;
        fxSlider.value = initialVolume;
        fxSource.volume = initialVolume;

        // Abonne-toi � l'�v�nement de changement de valeur
        fxSlider.onValueChanged.AddListener(SetVolume);

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("NoPoseBille", _OnNoPoseBille);
    }

    void SetVolume(float volume)
    {
        fxSource.volume = volume;
        // Optionnel : sauvegarder le volume si besoin
        GameManager.Instance.SetFxVolume(volume);
        //PlayerPrefs.SetFloat("MusicVolume", volume);
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
