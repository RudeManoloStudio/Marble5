using UnityEngine;



public class GrilleManager : MonoBehaviour
{

    [SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20); // Taille de la grille
    [SerializeField] private GameObject gridBackground;
    [SerializeField] private MotifData motif;
    [SerializeField] private GameObject billePrefab; // La bille à placer
    [SerializeField] private GameObject marqueurPrefab; // Préfab du marqueur pour les emplacements vides
    public int coins = 5;

    public static GrilleManager Instance { get; private set; }

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        GenererGrille();
        GenererMotif();
        PositionnerBackground();
        PositionnerCamera();
    }

    void GenererGrille()
    {

        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 position = new Vector3(x, y, 0.5f);
                GameObject marqueur = Instantiate(marqueurPrefab, position, Quaternion.identity);
                marqueur.transform.SetParent(this.transform);
            }
        }

        Debug.Log("Grille générée !");
    }

    void GenererMotif()
    {
        if (motif != null)
        {
            foreach (Vector2Int position in motif.BillesMotif)
            {
                GameObject bille = Instantiate(billePrefab, new Vector3Int(position.x + gridSize.x / 2, position.y + gridSize.y / 2), Quaternion.identity);
                bille.transform.SetParent(this.transform);

            }
        }
    }

    void PositionnerBackground()
    {
        gridBackground.transform.position = new Vector3(gridSize.x / 2, gridSize.y / 2, 0.5f);
    }

    void PositionnerCamera()
    {
        Camera.main.transform.position = new Vector3(gridSize.x / 2, gridSize.y / 2, -10f);
    }

}
