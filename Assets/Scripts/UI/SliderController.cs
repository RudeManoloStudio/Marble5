using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{

    [SerializeField] private Slider slider;        // Le slider
    [SerializeField] private Image icon;           // L’image à changer
    [SerializeField] private Sprite iconZero;      // Sprite lorsque slider = 0
    [SerializeField] private Sprite iconNormal;    // Sprite lorsque slider > 0

    private void Start()
    {
        slider.onValueChanged.AddListener(OnSliderValueChanged);
        OnSliderValueChanged(slider.value); // Force la mise à jour au start
    }

    private void OnSliderValueChanged(float value)
    {
        if (value == 0)
        {
            icon.sprite = iconZero;
        }
        else
        {
            icon.sprite = iconNormal;
        }
    }
}
