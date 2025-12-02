using UnityEngine;
using UnityEngine.EventSystems;

public class SecretDevModeToggle : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private int requiredTaps = 5;
    [SerializeField] private float maxTimeBetweenTaps = 0.5f;

    private int tapCount = 0;
    private float lastTapTime;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.unscaledTime - lastTapTime <= maxTimeBetweenTaps)
        {
            tapCount++;
        }
        else
        {
            tapCount = 1;
        }

        lastTapTime = Time.unscaledTime;

        if (tapCount >= requiredTaps)
        {
            ToggleDeveloperMode();
            tapCount = 0;
            lastTapTime = 0f; // Reset pour éviter que le prochain clic rapide compte
        }
    }

    private void ToggleDeveloperMode()
    {
        bool newState = !GameManager.Instance.DeveloperMode;
        GameManager.Instance.SetDeveloperMode(newState);

        // Rafraîchir l'affichage des niveaux avec un léger délai
        StartCoroutine(RefreshMenuDelayed());

        // Feedback visuel : petit scale bounce sur l'étoile
        StartCoroutine(ScaleBounce());
    }

    private System.Collections.IEnumerator RefreshMenuDelayed()
    {
        // Attendre la fin de la frame pour que Destroy() soit effectué
        yield return null;
        GameManager.Instance.PrepareMainMenu();
    }

    private System.Collections.IEnumerator ScaleBounce()
    {
        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;
        float duration = 0.15f;

        // Scale up
        float elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        // Scale down
        elapsed = 0f;
        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed / duration);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
    }
}
