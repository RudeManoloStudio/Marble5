using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float panSpeed = 5f;
    [SerializeField] [Range(0.1f, 5f)] private float zoomSpeed = 2.5f;
    [SerializeField] private float minZoom = 10.0f;
    [SerializeField] private float maxZoom = 20.0f;

    private bool isPanning = false;
    private Vector3 lastMousePosition;
    private Vector2 minPosition;
    private Vector2 maxPosition;
    private Camera mainCamera;
    private int sizeX;
    private int sizeY;

    // Touch tracking
    private Vector2 lastTouchMidpoint;
    private float lastTouchDistance;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        EventManager.AddListener("Replay", _OnReplay);
    }

    public void Setup(Vector2Int gridSize)
    {
        sizeX = gridSize.x;
        sizeY = gridSize.y;

        minPosition = new Vector2(0, 0);
        maxPosition = new Vector2(sizeX - 1, sizeY - 1);

        mainCamera.orthographicSize = 16f;
        mainCamera.transform.SetPositionAndRotation(new Vector3(sizeX / 2 + 0.5f, sizeY / 2 + 0.5f, -10), Quaternion.identity);
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        HandleMouseInput(); // Garde les contrôles PC
#endif

#if UNITY_ANDROID || UNITY_IOS
        HandleTouchInput(); // Contrôles tactiles pour mobile
#endif
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isPanning = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            isPanning = false;
        }

        if (isPanning)
        {
            Vector3 delta = Input.mousePosition - lastMousePosition;
            Vector3 move = new Vector3(-delta.x, -delta.y, 0) * panSpeed / 4 * Time.deltaTime;
            Vector3 newPosition = transform.position + move;

            newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

            transform.position = newPosition;
            lastMousePosition = Input.mousePosition;
        }

        float zoom = Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize - zoom, minZoom, maxZoom);
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevTouchDeltaMag = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentTouchDeltaMag = (touch0.position - touch1.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - currentTouchDeltaMag;

            // Zoom
            float zoomChange = deltaMagnitudeDiff * zoomSpeed * 0.01f;
            mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + zoomChange, minZoom, maxZoom);

            // Pan
            Vector2 currentMidpoint = (touch0.position + touch1.position) / 2;
            Vector2 previousMidpoint = (touch0PrevPos + touch1PrevPos) / 2;

            Vector2 deltaMidpoint = currentMidpoint - previousMidpoint;

            Vector3 move = new Vector3(-deltaMidpoint.x, -deltaMidpoint.y, 0) * panSpeed * 0.005f;
            Vector3 newPosition = transform.position + move;

            newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
            newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

            transform.position = newPosition;
        }
    }

    private void _OnReplay()
    {
        Vector2Int initialSize = new Vector2Int(sizeX, sizeY);
        Setup(initialSize);
    }
}
