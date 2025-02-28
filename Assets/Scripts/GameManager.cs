using UnityEngine;

public class GameManager : MonoBehaviour
{

    [Header("Paramètres Grille")]
    [SerializeField] private Vector2Int gridSize = new Vector2Int(21, 21); // Taille de la grille
    [SerializeField] private GameObject gridBackground;
    [SerializeField] private GameObject grid;
    [SerializeField] private bool gridBorders = true;
    [SerializeField] private MotifData motif;
    [SerializeField] private GameObject billePrefab; // La bille à placer
    [SerializeField] private Transform container;

    [Space(5)]
    [Header("Paramètres Jeu")]
    [SerializeField] private int coins = 5;  // crédits au départ
    [SerializeField] private bool infinisCoins = false; // crédits infinis pour debug
    [SerializeField] private ScoreData scoreData;
    private int initialCoins;

    public static GameManager Instance { get; private set; }

    public int Coins
    {
        get { return initialCoins; }
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

        EventManager.AddListener("PoseBille", _OnPoseBille);

        initialCoins = coins;

        InitializeGame();

    }

    public void UpdateScoreAndCoins(int quintes)
    {
        coins = coins + quintes;

        Vector2Int values = new Vector2Int();
        values.x = quintes;
        values.y = scoreData.Score[quintes - 1];

        EventManager.TriggerEvent("UpdateScoreAndCoins", values);
    }

    private void _OnPoseBille()
    {
        coins--;

        if (coins <= 0 && !infinisCoins)
        {
            EventManager.TriggerEvent("GameOver");
        }
    }

    private void GenererMotif()
    {
        if (motif != null)
        {
            foreach (Vector2Int position in motif.BillesMotif)
            {
                GameObject bille = Instantiate(billePrefab, new Vector3Int(position.x + gridSize.x / 2, position.y + gridSize.y / 2), Quaternion.identity);
                bille.transform.SetParent(container);

            }
        }
    }

    private void PositionnerBackgroundEtGrille()
    {
        float h_offset = (gridSize.x % 2 == 0) ? h_offset = 0.5f : h_offset = 0f;
        float v_offset = (gridSize.y % 2 == 0) ? v_offset = 0.5f : v_offset = 0f;

        gridBackground.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.5f);

        grid.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.4f);
        grid.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
        Material gridMaterial = grid.GetComponent<MeshRenderer>().material;
        gridMaterial.mainTextureScale = new Vector2(gridSize.x, gridSize.y);

        if (gridBorders)
        {

            int lr_h_offset = (gridSize.x % 2 == 0) ? lr_h_offset = 1 : lr_h_offset = 0;
            int lr_v_offset = (gridSize.y % 2 == 0) ? lr_v_offset = 1 : lr_v_offset = 0;

            LineRenderer lr = grid.GetComponent<LineRenderer>();

            lr.SetPosition(0, new Vector3(-1 + lr_h_offset, -1 + lr_v_offset));
            lr.SetPosition(1, new Vector3(-1 + lr_h_offset, gridSize.y + lr_v_offset));
            lr.SetPosition(2, new Vector3(gridSize.x + lr_h_offset, gridSize.y + lr_v_offset));
            lr.SetPosition(3, new Vector3(gridSize.x + lr_h_offset, -1 + lr_v_offset));

            lr.startWidth = 0.1f;
            lr.endWidth = 0.1f;
            lr.useWorldSpace = true;
        }

    }

    private void InitializeGame()
    {

        GenererMotif();
        PositionnerBackgroundEtGrille();

    }

    public void Replay()
    {
        EventManager.TriggerEvent("Replay");

        foreach (Transform child in container.transform)
        {
            Destroy(child.gameObject);
        }

        coins = initialCoins;

        InitializeGame();
    }
}
