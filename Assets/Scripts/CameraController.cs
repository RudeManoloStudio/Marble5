using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5.0f; // Vitesse de déplacement de la caméra

    [Range(0.1f, 5f)]
    [SerializeField] private float zoomSpeed = 2.5f; // Vitesse de zoom de la caméra

    [SerializeField] private float minZoom = 4.0f; // Zoom minimum
    [SerializeField] private float maxZoom = 10.0f; // Zoom maximum

    private Vector2 minPosition; // Limite minimale de déplacement
    private Vector2 maxPosition; // Limite maximale de déplacement

    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        int x = GameManager.Instance.GridSize.x;
        int y = GameManager.Instance.GridSize.y;

        minPosition = new Vector2(0, 0);
        maxPosition = new Vector2(x - 1, y - 1);

        mainCamera.orthographicSize = 6;

        mainCamera.transform.SetPositionAndRotation(new Vector3(x / 2, y / 2, -10), Quaternion.identity);

    }

    void Update()
    {
        // Déplacement de la caméra
        float moveHorizontal = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveVertical = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + new Vector3(moveHorizontal, moveVertical);

        // Restreindre le déplacement dans les limites définies
        newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

        transform.position = newPosition;

        // Zoom de la caméra
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - zoom, minZoom, maxZoom);
    }
}