using UnityEngine;

public class LogoFloatEffect : MonoBehaviour
{
    [Header("Flottement")]
    [SerializeField] private float amplitude = 10f;    // Hauteur du mouvement en pixels
    [SerializeField] private float speed = 1f;         // Vitesse de l'oscillation

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.localPosition;
    }

    private void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * amplitude;
        transform.localPosition = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
