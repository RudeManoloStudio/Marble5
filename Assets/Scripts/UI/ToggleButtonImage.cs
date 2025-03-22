using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonImage : MonoBehaviour
{
    public Sprite firstImage;
    public Sprite secondImage;
    private bool isFirstImage = true;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(ToggleImage);
        }
    }

    void ToggleImage()
    {
        if (isFirstImage)
        {
            button.image.sprite = secondImage;
        }
        else
        {
            button.image.sprite = firstImage;
        }
        isFirstImage = !isFirstImage;
    }
}