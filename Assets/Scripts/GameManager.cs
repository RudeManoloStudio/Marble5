using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelData levelData;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DisplayController display;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private FXManager fxManager;


    //[Header("Paramètres Grille")]
    //[SerializeField] private Vector2Int gridSize = new Vector2Int(20, 20); // Taille de la grille
    //[SerializeField] private GameObject gridBackground;
    //[SerializeField] private GameObject gridSG;
    //[SerializeField] private MotifData motif;

    //[Space(5)]
    //[Header("Paramètres Billes")]
    //[SerializeField] private GameObject billePrefab; // La bille à placer
    //[SerializeField] private GameObject plombPrefab; // instance bille noire
    //[SerializeField] private GameObject quintePrefab; // objet qui relie les billes dans une quinte
    //[SerializeField] private Transform container;

    //[Space(5)]
    //[Header("Paramètres Jeu")]
    //[SerializeField] private int difficulte; // difficulté du jeu
    [SerializeField] private int coins = 5;  // crédits au départ
    [SerializeField] private bool infinisCoins = false; // crédits infinis pour debug
    [SerializeField] private ScoreData scoreData;

    private Vector2Int gridSize;
    //private MotifData motif;
    //private GameObject bille;
    //private GameObject plomb;
    //private GameObject quinte;
    //private Material backgroundMaterial;
    //private int difficulte;

    private PlaceBille placeBille;
    private PlacePlomb placePlomb;
    private int compteurBilles = 0;
    private int initialCoins;
    private int difficulte;
    private int level;
    private int score;

    // ceci sera supprimé quand on pourra sauvegarder sur disque
    private Dictionary<int, int> scores;

    public static GameManager Instance { get; private set; }

    /*
    public int Coins
    {
        get { return initialCoins; }
        set { coins = value; }
    }
    */

    /*
    public SoundData Sounds
    {
        get { return levelData.layers[0].Sounds; }
    }
    */

    /*
    public Vector2Int GridSize
    {
        get { return gridSize; }
    }
    */    

    /*
    public GameObject Bille
    {
        get { return bille; }
    }
    */
    
    /*
    public GameObject Plomb
    {
        get { return plomb; }
    }
    */

    /*
    public GameObject Quinte
    {
        get { return quinte; }
    }
    */

    /*
    public Transform Container
    {
        get { return container; }
    }
    */

    
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

        placeBille = GetComponent<PlaceBille>();
        placePlomb = GetComponent<PlacePlomb>();

        //ici on va coder le chargement des préférences user
        scores = new Dictionary<int, int>();

        MainMenu();

        EventManager.AddListener("PoseBille", _OnPoseBille);

        initialCoins = coins;
        score = 0;
    }

    public void MainMenu()
    {
        // preparation du main menu
        uiManager.SetMainPanel(levelData.layers.Length);
        display.HideBackground();
    }



    public void PrepareLevel(int levelToPrepare)
    {
        
        // pour l'instant x = 0
        levelToPrepare = 0;

        this.level = levelToPrepare;

        // pour l'instant on cache le main UI sans vérifier si le niveau est accessible
        // et on affiche le header UI
        uiManager.SetGameMode();

        if (scores.ContainsKey(0))
        {
            uiManager.SetHighScoreText(scores[0]);
        }
        else
        {
            uiManager.SetHighScoreText(0);
        }

        

        gridSize = levelData.layers[level].GridSize;

        // positionnement de la camera
        Camera.main.GetComponent<CameraController>().Setup(gridSize);

        // bille + plomb + background + grid
        display.SetBilleAndPlomb(levelData.layers[level].Bille, levelData.layers[level].Plomb);
        display.ShowBackground();
        display.PrepareBackgroundAndGrid(gridSize, levelData.layers[level].BackgroundMaterial);
        if (levelData.layers[level] != null) { display.PrepareMotif(gridSize, levelData.layers[level].Motif); }

        placeBille.Setup(gridSize, levelData.layers[level].Bille, levelData.layers[level].Quinte);
        placePlomb.Setup(gridSize, levelData.layers[level].Plomb);

        difficulte = levelData.layers[level].Difficulte;

        // Musique
        musicManager.PlayPlaylist(levelData.layers[level].Sounds);

        // FX
        fxManager.Setup(levelData.layers[level].Sounds);

        coins = initialCoins;

        

        //motif = levelData.layers[0].Motif;
        //bille = levelData.layers[0].Bille;
        //plomb = levelData.layers[0].Plomb;
        //quinte = levelData.layers[0].Quinte;
        //backgroundMaterial = levelData.layers[0].BackgroundMaterial;
        //difficulte = levelData.layers[0].Difficulte;


    }

    public void Replay()
    {

        uiManager.SetGameMode();

        // positionnement de la camera
        Camera.main.GetComponent<CameraController>().Setup(gridSize);

        display.ClearBoard();
        if (levelData.layers[level] != null) { display.PrepareMotif(gridSize, levelData.layers[level].Motif); }



        //EventManager.TriggerEvent("Replay");


        //foreach (Transform child in container.transform)
        //{
        //Destroy(child.gameObject);
        //}


        coins = initialCoins;
        compteurBilles = 0;
        score = 0;

        //InitializeGame();
    }

    /*
    private void Start()
    {

        EventManager.AddListener("PoseBille", _OnPoseBille);

        initialCoins = coins;

        InitializeGame();

    }
    */

    public void UpdateScoreAndCoins(int quintes)
    {
        coins = coins + quintes;

        Vector2Int values = new Vector2Int();
        values.x = quintes;
        values.y = scoreData.Score[quintes - 1];

        EventManager.TriggerEvent("UpdateScoreAndCoins", values);

        score += scoreData.Score[quintes - 1];

        uiManager.UpdateScore(score);
    }

    private void _OnPoseBille(object billePosition)
    {
        Vector3 bPosition = (Vector3)(billePosition);
        coins--;

        if (coins <= 0 && !infinisCoins)
        {
            



            uiManager.GameOver();
        }

        /*
        if (coins <= 0 && !infinisCoins)
        {
            EventManager.TriggerEvent("GameOver");
        }
        */

        compteurBilles++;
        if (compteurBilles >= difficulte)
        {
            placePlomb.PlacePlombAt(bPosition);
            compteurBilles = 0;
        }

        /*
        if (compteurBilles >= difficulte)
        { 
            EventManager.TriggerEvent("PosePlomb", bPosition);
            compteurBilles = 0;
        }*/
        
    }

    /*
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
    */

    /*
    private void PositionnerBackgroundEtGrille()
    {

        float h_offset = (gridSize.x % 2 == 0) ? h_offset = 0.5f : h_offset = 0f;
        float v_offset = (gridSize.y % 2 == 0) ? v_offset = 0.5f : v_offset = 0f;

        gridBackground.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.5f);
        gridBackground.transform.localScale = new Vector3(gridSize.x / 10 + 2, 1, gridSize.y / 10 + 2);
        gridBackground.GetComponent<Renderer>().material = backgroundMaterial;

        gridSG.transform.position = new Vector3(gridSize.x / 2 + h_offset, gridSize.y / 2 + v_offset, 0.4f);
        gridSG.transform.localScale = new Vector3(gridSize.x, gridSize.y, 1);
        Material gridSGMaterial = gridSG.GetComponent<MeshRenderer>().material;
        gridSGMaterial.SetVector("_tiling", new Vector4(gridSize.x, gridSize.y));

    }
    */

    /*
    private void InitializeGame()
    {

        //GenererMotif();
        //PositionnerBackgroundEtGrille();

    }
    */

    /*
    public void Replay()
    {
        EventManager.TriggerEvent("Replay");

        
        //foreach (Transform child in container.transform)
        //{
            //Destroy(child.gameObject);
        //}
        

        coins = initialCoins;
        compteurBilles = 0;

        InitializeGame();
    }
    */

    public void Quit()
    {
        Application.Quit();
    }
}

