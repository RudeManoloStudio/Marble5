using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelData levelData;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DisplayController display;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private FXManager fxManager;
    [SerializeField] private int coins = 5;  // cr�dits au d�part
    [SerializeField] private bool infinisCoins = false; // cr�dits infinis pour debug
    [SerializeField] private ScoreData scoreData;

    private Vector2Int gridSize;
    private PlaceBille placeBille;
    private PlacePlomb placePlomb;
    private int compteurBilles = 0;
    private int initialCoins;
    private int difficulte;
    private int score;
    private int level;

    // ceci sera supprim� quand on pourra sauvegarder sur disque
    private Dictionary<int, int> scores;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // V�rifie si une instance existe d�j�
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Permet de conserver l'objet entre les sc�nes
        }
        else
        {
            // Si une instance existe d�j�, d�truire cet objet
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        placeBille = GetComponent<PlaceBille>();
        placePlomb = GetComponent<PlacePlomb>();

        // ici on va coder le chargement des pr�f�rences user
        // pour l'instant pas de sauvegarde
        scores = new Dictionary<int, int>();

        PrepareMainMenu();

        EventManager.AddListener("PoseBille", _OnPoseBille);

        initialCoins = coins;
        score = 0;
    }

    public void PrepareMainMenu()
    {
        // preparation du main menu
        // on envoie au uiManager les éléments nécessaires dans une liste
        // pour chaque level : int ID & int étoiles obtenues & available

        List<LevelStruct> list = new List<LevelStruct>();

        bool nextLevelAvailable = false;

        for (int x = 0; x < levelData.layers.Length; x++)
        {

            LevelStruct levelStruct = new LevelStruct();

            levelStruct.ID = x;
            levelStruct.stars = 0;

            if (x == 0)
            {
                levelStruct.available = true;
            }
            else
            {
                if (nextLevelAvailable)
                {
                    levelStruct.available = true;
                    nextLevelAvailable = false;
                }
                else
                {
                    levelStruct.available = false;
                }
            }

            int stars = 0;
            if (scores.ContainsKey(x))
            {
                int value = scores[x];
                if (value >= levelData.layers[x].FirstStarScore)
                {
                    stars++;
                }
                if (value >= levelData.layers[x].SecondStarScore)
                {
                    stars++;
                }
                if (value >= levelData.layers[x].ThirdStarScore)
                {
                    stars++;
                }
            }

            if (stars > 0)
            {
                levelStruct.available = true;
                levelStruct.stars = stars;

                nextLevelAvailable = true;
            }

            list.Add(levelStruct);
        }

        uiManager.SetMainPanel(list);
        display.ResetBoard();
    }



    public void PrepareLevel(int level)
    {

        this.level = level;

        uiManager.SetGameMode();

        if (scores.ContainsKey(level))
        {
            uiManager.SetHighScoreText(scores[level]);
        }
        else
        {
            uiManager.SetHighScoreText(0);
        }

        gridSize = levelData.layers[level].GridSize;

        // positionnement de la camera
        Camera.main.GetComponent<CameraController>().Setup(gridSize);

        // bille + plomb + background + grid
        display.ClearBoard();
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
        compteurBilles = 0;
        score = 0;
    }

    public void Replay()
    {
        PrepareLevel(level);
    }

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
            
            if (scores.ContainsKey(level))
            {
                if (score >= scores[level])
                {
                    scores[level] = score;
                }
            }
            else
            {
                scores.Add(level, score);
            }
            
            uiManager.GameOver();
        
        }

        compteurBilles++;
        if (compteurBilles >= difficulte)
        {
            placePlomb.PlacePlombAt(bPosition);
            compteurBilles = 0;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}

public struct LevelStruct
{
    public int ID;
    public bool available;
    public int stars;
}