using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelData levelData;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DisplayController display;
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private FXManager fxManager;
    [SerializeField] private int initialCoins = 5;
    [SerializeField] private bool infinisCoins = false;
    [SerializeField] private ScoreData scoreData;

    private Vector2Int gridSize;
    private PlaceBille placeBille;
    private PlacePlomb placePlomb;
    private int compteurBilles;
    private int difficulte;
    private int score;
    private int level;
    private int handicap;
    private UserDataManager userDataManager;
    private UserData userData;
    private Dictionary<int, int> scores;
    private int coins;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        placeBille = GetComponent<PlaceBille>();
        placePlomb = GetComponent<PlacePlomb>();

        string filePath = Application.persistentDataPath + "/UserPreferences.json";
        userDataManager = new UserDataManager(filePath);
        userData = userDataManager.GetUserData();
        

        scores = new Dictionary<int, int>();
        // Chargement du dictionnaire depuis le fichier
        if (DictionaryStorage.LoadDictionaryFromFile<int, int>("dictionnaire.json") == null)
        {
            // Sauvegarde du dictionnaire
            DictionaryStorage.SaveDictionaryToFile(scores, "dictionnaire.json");
        }

        scores = DictionaryStorage.LoadDictionaryFromFile<int, int>("dictionnaire.json");

        PrepareMainMenu();

        EventManager.AddListener("PoseBille", _OnPoseBille);
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
        handicap = levelData.layers[level].Handicap;

        // positionnement de la camera
        Camera.main.GetComponent<CameraController>().Setup(gridSize);

        // bille + plomb + background + grid
        display.ClearBoard();
        display.SetBilleAndPlomb(levelData.layers[level].Bille, levelData.layers[level].Plomb);
        display.ShowBackground();
        display.PrepareBackgroundAndGrid(gridSize, levelData.layers[level].BackgroundMaterial);
        if (levelData.layers[level].Motif != null) { display.PrepareMotif(gridSize, levelData.layers[level].Motif, handicap); }


        placeBille.Setup(gridSize, levelData.layers[level].Bille, levelData.layers[level].Quinte);
        placePlomb.Setup(gridSize, levelData.layers[level].Plomb);

        difficulte = levelData.layers[level].Difficulte;

        // Musique
        musicManager.PreparePlaylist(levelData.layers[level].Sounds);
        if (userData.musicOn)
        {
            musicManager.PlayTrack();
        }

        // FX
        if (userData.fxOn) fxManager.Setup(levelData.layers[level].Sounds);

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

            // Sauvegarde du dictionnaire
            DictionaryStorage.SaveDictionaryToFile(scores, "dictionnaire.json");
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

    public void ToggleMusic()
    {
        userData.musicOn = userData.musicOn == true ? false : true;
        userDataManager.ToggleMusic(userData.musicOn);

        musicManager.Toggle();

    }
}

public struct LevelStruct
{
    public int ID;
    public bool available;
    public int stars;
}