using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Paramètres Grille")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(21, 21); // Taille de la grille
    [SerializeField] private GameObject gridBackground;
    [SerializeField] private GameObject grid;
    [SerializeField] private MotifData motif;
    [SerializeField] private GameObject billePrefab; // La bille à placer
    [SerializeField] private GameObject marqueurPrefab; // Préfab du marqueur pour les emplacements vides

    [Space(5)]
    [Header("Paramètres Jeu")]
    [SerializeField] private int coins = 5;  // crédits au départ
    [SerializeField] private bool infinisCoins = false; // crédits infinis pour debug

    public static GameManager Instance { get; private set; }

    public int Coins
    {
        get { return coins; }
        set { coins = value; }
    }

    public Vector2Int GridSize
    {
        get { return gridSize; }
    }

    public GameObject BillePrefab
    {
        get { return billePrefab; }
    }

    private void Awake()
    {
        // Vérifie si une instance existe déjà
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Permet de conserver l'objet entre les scènes
        }
        else
        {
            // Si une instance existe déjà, détruire cet objet
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenererGrille();
        GenererMotif();
        PositionnerBackgroundEtGrille();

        EventManager.AddListener("UpdateScore", _OnUpdateScore);
        EventManager.AddListener("PoseBille", _OnPoseBille);

    }

    private void _OnUpdateScore()
    {
        coins++;
    }

    private void _OnPoseBille()
    {
        coins--;

        if (coins <= 0 && !infinisCoins)
        {
            EventManager.TriggerEvent("GameOver");
        }
    }
    
    private void GenererGrille()
    {
        /*
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 position = new Vector3(x, y, 0.5f);
                GameObject marqueur = Instantiate(marqueurPrefab, position, Quaternion.identity);
                marqueur.transform.SetParent(this.transform);
            }
        }*/
    }

    private void GenererMotif()
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

    private void PositionnerBackgroundEtGrille()
    {
        gridBackground.transform.position = new Vector3(gridSize.x / 2, gridSize.y / 2, 0.5f);

        grid.transform.position = new Vector3(gridSize.x / 2, gridSize.y / 2, 0.4f);
        grid.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
        Material gridMaterial = grid.GetComponent<MeshRenderer>().material;
        gridMaterial.mainTextureScale = new Vector2(gridSize.x, gridSize.y);

    }
}
