using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBarController : MonoBehaviour
{

    [SerializeField] private Image fillImage;
    [SerializeField] private RectTransform firstStar, secondStar, thirdStar;
    [SerializeField] private Sprite fullStar;
    [SerializeField] private Sprite emptyStar;

    [Header("Couleurs des étoiles")]
    [SerializeField] private Color star1Color = new Color(1f, 1f, 0.6f, 1f);      // Jaune clair
    [SerializeField] private Color star2Color = new Color(1f, 0.9f, 0.2f, 1f);    // Jaune
    [SerializeField] private Color star3Color = new Color(1f, 0.75f, 0f, 1f);     // Or
    [SerializeField] private Color starNotObtainedColor = new Color(1f, 1f, 1f, 1f); // Blanc pur

    [Header("Animation")]
    [SerializeField] private float pulseScale = 4f;         // Multiplicateur de taille pendant le pulse
    [SerializeField] private float pulseDuration = 4.0f;    // Durée de l'animation en secondes

    [Header("Pluie d'étoiles")]
    [SerializeField] private GameObject starParticlePrefab; // Prefab d'une mini-étoile pour la pluie
    [SerializeField] private int starCount = 15;            // Nombre d'étoiles dans la pluie
    [SerializeField] private float starRainDuration = 1.5f; // Durée de la pluie

    private float maxScore;
    private float currentScore;
    private int firstStarScore;
    private int secondStarScore;
    private int thirdStarScore;

    // Pour éviter de rejouer l'animation si l'étoile est déjà obtenue
    private bool firstStarObtained;
    private bool secondStarObtained;
    private bool thirdStarObtained;

    public void SetStars(Vector3Int starsScore)
    {

        firstStarScore = starsScore.x;
        secondStarScore = starsScore.y;
        thirdStarScore = starsScore.z;

        currentScore = 0;
        this.maxScore = starsScore.z;

        PositionStars(starsScore);

        // Réinitialiser les flags d'étoiles obtenues
        firstStarObtained = false;
        secondStarObtained = false;
        thirdStarObtained = false;

        UpdateHealthBar();
    }

    public void AddScore(int amount)
    {

        currentScore += amount;
        currentScore = Mathf.Clamp(currentScore, 0, maxScore);
        UpdateHealthBar();

        // Changement de couleur avec animation pulse
        if (currentScore >= firstStarScore && !firstStarObtained)
        {
            firstStarObtained = true;
            firstStar.gameObject.GetComponent<Image>().sprite = fullStar;
            firstStar.gameObject.GetComponent<Image>().color = star1Color;
            StartCoroutine(PulseStar(firstStar));
            StartCoroutine(StarRain(firstStar, star1Color, 1)); // Multiplicateur x1
        }
        if (currentScore >= secondStarScore && !secondStarObtained)
        {
            secondStarObtained = true;
            secondStar.gameObject.GetComponent<Image>().sprite = fullStar;
            secondStar.gameObject.GetComponent<Image>().color = star2Color;
            StartCoroutine(PulseStar(secondStar));
            StartCoroutine(StarRain(secondStar, star2Color, 2)); // Multiplicateur x2
        }
        if (currentScore >= thirdStarScore && !thirdStarObtained)
        {
            thirdStarObtained = true;
            thirdStar.gameObject.GetComponent<Image>().sprite = fullStar;
            thirdStar.gameObject.GetComponent<Image>().color = star3Color;
            StartCoroutine(PulseStar(thirdStar));
            StartCoroutine(StarRain(thirdStar, star3Color, 4)); // Multiplicateur x4
        }

    }

    public int GetStars()
    {

        int x = 0;

        if (currentScore >= thirdStarScore)
        {
            x = 3;
        }
        else if (currentScore >= secondStarScore)
        {
            x = 2;
        }
        else if (currentScore >= firstStarScore)
        {
            x = 1;
        }

        return x;
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

        firstStar.gameObject.GetComponent<Image>().sprite = emptyStar;
        firstStar.gameObject.GetComponent<Image>().color = starNotObtainedColor;

        secondStar.gameObject.GetComponent<Image>().sprite = emptyStar;
        secondStar.gameObject.GetComponent<Image>().color = starNotObtainedColor;

        thirdStar.gameObject.GetComponent<Image>().sprite = emptyStar;
        thirdStar.gameObject.GetComponent<Image>().color = starNotObtainedColor;

    }

    private IEnumerator PulseStar(RectTransform star)
    {
        // Sauvegarder le scale original de l'étoile (pas forcément 1)
        Vector3 originalScale = star.localScale;
        Vector3 targetScale = originalScale * pulseScale;
        float halfDuration = pulseDuration / 2f;

        // Phase 1 : Grossir
        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            star.localScale = Vector3.Lerp(originalScale, targetScale, t);
            yield return null;
        }

        // Phase 2 : Revenir à la taille normale
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            star.localScale = Vector3.Lerp(targetScale, originalScale, t);
            yield return null;
        }

        star.localScale = originalScale;
    }

    private IEnumerator StarRain(RectTransform sourceStar, Color starColor, int multiplier)
    {
        if (starParticlePrefab == null) yield break;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) yield break;

        int totalStars = starCount * multiplier;
        float totalDuration = starRainDuration * multiplier;

        for (int i = 0; i < totalStars; i++)
        {
            // Créer une mini-étoile
            GameObject miniStar = Instantiate(starParticlePrefab, canvas.transform);
            RectTransform miniStarRect = miniStar.GetComponent<RectTransform>();
            Image miniStarImage = miniStar.GetComponent<Image>();

            if (miniStarImage != null)
            {
                miniStarImage.color = starColor;
            }

            // Position initiale : autour de l'étoile source
            Vector3 startPos = sourceStar.position;
            startPos.x += Random.Range(-50f, 50f);
            miniStarRect.position = startPos;

            // Taille aléatoire
            float randomScale = Random.Range(0.3f, 0.8f);
            miniStarRect.localScale = Vector3.one * randomScale;

            // Lancer l'animation de chute
            StartCoroutine(FallingStar(miniStarRect, miniStarImage));

            // Délai entre chaque étoile
            yield return new WaitForSeconds(totalDuration / totalStars);
        }
    }

    private IEnumerator FallingStar(RectTransform star, Image image)
    {
        float fallSpeed = Random.Range(200f, 400f);
        float horizontalDrift = Random.Range(-50f, 50f);
        float rotationSpeed = Random.Range(-360f, 360f);
        float lifetime = 1.5f;
        float elapsed = 0f;

        Vector3 startPos = star.position;
        Color startColor = image.color;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / lifetime;

            // Chute avec dérive horizontale
            star.position = startPos + new Vector3(horizontalDrift * t, -fallSpeed * elapsed, 0);

            // Rotation
            star.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // Fade out progressif
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            image.color = c;

            yield return null;
        }

        Destroy(star.gameObject);
    }
}
