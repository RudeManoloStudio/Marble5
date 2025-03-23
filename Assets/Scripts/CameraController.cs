using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private float panSpeed = 5f;
    [SerializeField] private float panBorderThickness = 10f;
    [SerializeField] private bool enableDragPan = true;
    [SerializeField] private bool enableBorderPan = true;
    [SerializeField] [Range(0.1f, 5f)] private float zoomSpeed = 2.5f; // Vitesse de zoom de la caméra
    [SerializeField] private float minZoom = 6.0f; // Zoom minimum
    [SerializeField] private float maxZoom = 10.0f; // Zoom maximum

    private bool isPanning = false;
    private Vector3 lastMousePosition;
    private Vector2 minPosition; // Limite minimale de déplacement
    private Vector2 maxPosition; // Limite maximale de déplacement
    private Camera mainCamera;
    private int sizeX;
    private int sizeY;

    void Start()
    {
        mainCamera = GetComponent<Camera>();

        //sizeX = GameManager.Instance.GridSize.x;
        //sizeY = GameManager.Instance.GridSize.y;

        //minPosition = new Vector2(0, 0);
        //maxPosition = new Vector2(sizeX - 1, sizeY - 1);

        //Setup();

        EventManager.AddListener("Replay", _OnReplay);
    }

    //private void Setup()
    public void Setup(Vector2Int gridSize)
    {

        sizeX = gridSize.x;
        sizeY = gridSize.y;

        minPosition = new Vector2(0, 0);
        maxPosition = new Vector2(sizeX - 1, sizeY - 1);

        mainCamera.orthographicSize = 6;
        mainCamera.transform.SetPositionAndRotation(new Vector3(sizeX / 2 + 0.5f, sizeY / 2 + 0.5f, -10), Quaternion.identity);
    }

    void Update()
    {
        if (enableDragPan)
        {
            if (Input.GetMouseButtonDown(1)) // Bouton droit de la souris enfoncé
            {
                isPanning = true;
                lastMousePosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(1)) // Bouton droit de la souris relâché
            {
                isPanning = false;
            }

            if (isPanning)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 move = new Vector3(-delta.x, -delta.y, 0) * panSpeed / 4 * Time.deltaTime;
                Vector3 newPosition = transform.position + move;

                // Limiter le déplacement
                newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
                newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

                transform.position = newPosition;
                lastMousePosition = Input.mousePosition;

            }
        }

        if (enableBorderPan && !isPanning)
        {
            Vector3 pos = transform.position;

            if (Input.mousePosition.y >= Screen.height - panBorderThickness)
            {
                pos.y += panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.y <= panBorderThickness)
            {
                pos.y -= panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x >= Screen.width - panBorderThickness)
            {
                pos.x += panSpeed * Time.deltaTime;
            }
            if (Input.mousePosition.x <= panBorderThickness)
            {
                pos.x -= panSpeed * Time.deltaTime;
            }

            // Restreindre le déplacement dans les limites définies
            pos.x = Mathf.Clamp(pos.x, minPosition.x, maxPosition.x);
            pos.y = Mathf.Clamp(pos.y, minPosition.y, maxPosition.y);
            transform.position = pos;
        }

        // Zoom de la caméra
        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - zoom, minZoom, maxZoom);
    }

    private void _OnReplay()
    {
        Vector2Int initialSize = new Vector2Int(sizeX, sizeY);
        Setup(initialSize);
    }
}