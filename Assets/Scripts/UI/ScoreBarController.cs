using UnityEngine;
using UnityEngine.UI;

public class ScoreBarController : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    //[SerializeField] private RectTransform scoreBarFillRect;
    [SerializeField] private RectTransform firstStar, secondStar, thirdStar;

    private float maxScore;
    private float currentScore;


    public void SetStars(Vector3Int starsScore)
    {
        currentScore = 0;
        this.maxScore = starsScore.z;

        PositionStars(starsScore);

        UpdateHealthBar();
    }
    public void AddScore(int amount)
    {
        currentScore += amount;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        UpdateHealthBar();
    }

    void UpdateHealthBar()
    {
        fillImage.fillAmount = currentScore / maxScore;
    }

    void PositionStars(Vector3Int starsScore)
    {

        float firstStarPosIL = Mathf.InverseLerp(0f, (float)maxScore, (float)starsScore.x);
        float secondStarPosIL = Mathf.InverseLerp(0f, (float)maxScore, (float)starsScore.y);

        float firstStarPos = Mathf.Lerp(-50, 50, firstStarPosIL);
        float secondStarPos = Mathf.Lerp(-50, 50, secondStarPosIL);

        firstStar.anchoredPosition = new Vector2(firstStarPos, firstStar.anchoredPosition.y);
        secondStar.anchoredPosition = new Vector2(secondStarPos, secondStar.anchoredPosition.y);
        thirdStar.anchoredPosition = new Vector2(50, thirdStar.anchoredPosition.y);
    }
}
