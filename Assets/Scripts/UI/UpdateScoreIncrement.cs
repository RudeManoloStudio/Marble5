using System.Collections;
using UnityEngine;
using TMPro;

public class UpdateScoreIncrement : MonoBehaviour
{
    private TMP_Text targetScoreText;
    private RectTransform rectTransform;
    private TMP_Text myText;

    private float flyDuration = 1.5f;
    private float arcHeight = 150f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        myText = GetComponent<TMP_Text>();
    }

    public void FlyToScore(TMP_Text scoreText)
    {
        targetScoreText = scoreText;
        StartCoroutine(FlyCoroutine());

        // Filet de sécurité : destruction garantie même si la coroutine est interrompue
        Destroy(gameObject, flyDuration + 0.5f);
    }

    private IEnumerator FlyCoroutine()
    {
        Vector3 startPosition = rectTransform.position;
        Vector3 endPosition = targetScoreText.rectTransform.position + new Vector3(50f, 0f, 0f);

        float elapsed = 0f;

        while (elapsed < flyDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / flyDuration;

            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            Vector3 straightLine = Vector3.Lerp(startPosition, endPosition, smoothT);

            float arc = Mathf.Sin(t * Mathf.PI) * arcHeight;

            Vector3 curvedPosition = straightLine + Vector3.up * arc;

            rectTransform.position = curvedPosition;

            float scale = Mathf.Lerp(1.2f, 0.6f, smoothT);
            rectTransform.localScale = Vector3.one * scale;

            yield return null;
        }

        rectTransform.position = endPosition;

        Destroy(gameObject);
    }
}