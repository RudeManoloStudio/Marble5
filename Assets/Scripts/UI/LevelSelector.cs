using UnityEngine;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour
{

    [SerializeField] private Text text;
    [SerializeField] private Image star1;
    [SerializeField] private Image star2;
    [SerializeField] private Image star3;


    public void SetLevelID(int level)
    {

        text.text = level.ToString();

        star1.color = new Color(star1.color.r, star1.color.g, star1.color.b, 1f);
        star2.color = new Color(star2.color.r, star2.color.g, star2.color.b, 0.25f);
        star3.color = new Color(star3.color.r, star3.color.g, star3.color.b, 0.25f);

    }


}
