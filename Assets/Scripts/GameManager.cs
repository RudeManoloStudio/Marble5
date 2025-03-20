using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelData levelData;

    //[Header("Paramètres Grille")]
    //[SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20); // Taille de la grille
    [SerializeField] private GameObject gridBackground;
    [SerializeField] private GameObject gridSG;
    //[SerializeField] private MotifData motif;

    //[Space(5)]
    //[Header("Paramètres Billes")]
    //[SerializeField] private GameObject billePrefab; // La bille à placer
    //[SerializeField] private GameObject plombPrefab; // instance bille noire
    //[SerializeField] private GameObject quintePrefab; // objet qui relie les billes dans une quinte
    [SerializeField] private Transform container;

    [Space(5)]
    [Header("Paramètres Jeu")]
    [SerializeField] private int difficulte; // difficulté du jeu
    [SerializeField] private int coins = 5;  // crédits au départ
    [SerializeField] private bool infinisCoins = false; // crédits infinis pour debug
    [SerializeField] private ScoreData scoreData;

    private Vector2Int gridSize;
    private MotifData motif;
    private GameObject bille;
    private GameObject plomb;
    private GameObject quinte;
    private Material backgroundMaterial;

    private int compteurBilles = 0;
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

    public GameObject Bille
    {
        get { return bille; }
    }
    
    public GameObject Plomb
    {
        get { return plomb; }
    }

    public GameObject Quinte
    {
        get { return quinte; }
    }

    public Transform Container
    {
        get { return container; }
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

        gridSize = levelData.layers[0].GridSize;
        motif = levelData.layers[0].Motif;
        bille = levelData.layers[0].Bille;
        plomb = levelData.layers[0].Plomb;
        quinte = levelData.layers[0].Quinte;
        backgroundMaterial = levelData.layers[0].BackgroundMaterial;
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

    private void _OnPoseBille(object billePosition)
    {
        Vector3 bPosition = (Vector3)(billePosition);
        coins--;

        if (coins <= 0 && !infinisCoins)
        {
            EventManager.TriggerEvent("GameOver");
        }

        compteurBilles++;
        if (compteurBilles >= difficulte)
        { 
            EventManager.TriggerEvent("PosePlomb", bPosition);
            compteurBilles = 0;
        }
        
    }

    private void GenererMotif()
    {
        if (motif != null)
        {
            foreach (Vector2Int position in motif.BillesMotif)
            {
                GameObject newBille = Instantiate(bille, new Vector3Int(position.x + gridSize.x / 2, position.y + gridSize.y / 2), Quaternion.identity);
                newBille.transform.SetParent(container);
                newBille.tag = "Bille";

            }
        }
    }

    private void PositionnerBackgroundEtGrille()
    {

        float h_offset = (gridSize.x % 2 == 0) ? h_offset = 0.5f : h_offset = 0f;
        float v_offset = (gridSize.y % 2 == 0) ? v_offset = 0.5f : v_offset = 0f;

        gridBackground.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.5f);
        gridBackground.transform.localScale = new Vector3(GridSize.x / 10 + 2, 1, GridSize.y / 10 + 2);
        gridBackground.GetComponent<Renderer>().material = backgroundMaterial;

        gridSG.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.4f);
        gridSG.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
        Material gridSGMaterial = gridSG.GetComponent<MeshRenderer>().material;
        gridSGMaterial.SetVector("_tiling", new Vector4(gridSize.x, gridSize.y));

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
        compteurBilles = 0;

        InitializeGame();
    }
    public void Quit()
    {
        Application.Quit();
    }
}

