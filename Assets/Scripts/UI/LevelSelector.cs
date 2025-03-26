using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{

    [SerializeField] private Text text;
    [SerializeField] private Image star1;
    [SerializeField] private Image star2;
    [SerializeField] private Image star3;

    public void SetLevelParameters(LevelStruct level)
    {

        text.text = level.ID.ToString();

        if (level.stars == 0 && !level.available)
        {
            //superposer le cadenas
        }
        else
        {
            if (level.stars == 1) star1.color = new Color(star1.color.r, star1.color.g, star1.color.b, 1f);
            if (level.stars == 2) star2.color = new Color(star2.color.r, star2.color.g, star2.color.b, 1f);
            if (level.stars == 3) star3.color = new Color(star2.color.r, star2.color.g, star2.color.b, 1f);
        }

    }
}