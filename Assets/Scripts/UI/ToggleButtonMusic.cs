/*
using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonMusic : MonoBehaviour
{

    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    private Button button;

    void Start()
    {
        button = GetComponent<Button>();

        
        if (!GameManager.Instance.MusicOn)
        {
            ToggleImage();
        }

        if (button != null)
        {
            button.onClick.AddListener(ToggleImage);
            button.onClick.AddListener(TriggerMusic);
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

    private void TriggerMusic()
    {
        EventManager.TriggerEvent("ToggleMusic");
    }
}
*/