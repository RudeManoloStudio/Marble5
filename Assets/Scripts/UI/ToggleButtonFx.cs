using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonFx : MonoBehaviour
{

    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        if (!GameManager.Instance.FxOn)
        {
            ToggleImage();
        }

        if (button != null)
        {
            button.onClick.AddListener(ToggleImage);
            button.onClick.AddListener(TriggerFx);
        }
    }

    public void ToggleImage()
    {

        if (button.image.sprite == onSprite)
        {
            button.image.sprite = offSprite;
        }
        else
        {
            button.image.sprite = onSprite;
        }

    }

    private void TriggerFx()
    {
        EventManager.TriggerEvent("ToggleFX");
    }
}