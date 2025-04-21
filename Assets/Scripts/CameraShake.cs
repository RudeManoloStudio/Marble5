using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private float duration;
    [SerializeField] float magnitude = 0.1f;

    private Vector3 originalPos;

    public void Shake(float duration)
    {

        this.duration = duration;
        originalPos = transform.localPosition;
        StopAllCoroutines(); // au cas où
        StartCoroutine(DoShake());

    }

    private System.Collections.IEnumerator DoShake()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
