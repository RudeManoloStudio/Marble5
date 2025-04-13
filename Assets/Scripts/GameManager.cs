using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelData levelData;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DisplayController display;
    [SerializeField] private FXData fxData;
    [SerializeField] private MusicData musicData;
    [SerializeField] private int initialCoins = 5;
    [SerializeField] private bool infinisCoins = false;
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private Sprite initialBackground;
    [SerializeField] private ReserveController reserveController;

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
    //private bool musicOn;
    private float musicVolume;
    //private bool fxOn;
    private float fxVolume;
    public static GameManager Instance { get; private set; }

    public float MusicVolume
    {
        get { return musicVolume; }
    }

    public List<AudioClip> Playlist
    {
        get { return musicData.Playlist; }
    }

    public float FxVolume
    {
        get { return fxVolume; }
    }
    public FXData FxData
    {
        get { return fxData; }
    }

    public Sprite InitialBackground
    {
        get { return initialBackground; }
    }

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

        // preferences user
        string filePath = Application.persistentDataPath + "/UserPreferences.json";
        userDataManager = new UserDataManager(filePath);
        userData = userDataManager.GetUserData();

        // Musique
        musicVolume = userData.musicVolume;
        //musicOn = userData.musicOn;

        // FX
        fxVolume = userData.fxVolume;
        //fxOn = userData.fxOn;

        //EventManager.AddListener("ToggleMusic", _OnToggleMusic);
        //EventManager.AddListener("ToggleFX", _OnToggleFX);

    }

    private void Start()
    {

        placeBille = GetComponent<PlaceBille>();
        placePlomb = GetComponent<PlacePlomb>();

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
        PrepareLevelX();

    }

    private void PrepareLevelX()
    {

        uiManager.SetGameMode();
        placeBille.Unpause();

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
        display.PrepareBackgroundAndGrid(gridSize, levelData.layers[level].BackgroundTexture);
        if (levelData.layers[level].Motif != null) { display.PrepareMotif(gridSize, levelData.layers[level].Motif, handicap); }

        placeBille.Setup(gridSize, levelData.layers[level].Bille, levelData.layers[level].Quinte);
        placePlomb.Setup(gridSize, levelData.layers[level].Plomb);

        //reserveController.SetBilleAndPlomb(levelData.layers[level].Bille, levelData.layers[level].Plomb);

        difficulte = levelData.layers[level].Difficulte;

        coins = initialCoins;
        compteurBilles = 0;
        score = 0;

        //PrepareReserve();
    }

    private void PrepareReserve()
    {
        int nextPlomb = difficulte;

        for (int x = 0; x < initialCoins; x++)
        {
            
            reserveController.AddBille();

            nextPlomb--;

            if (nextPlomb <= 0)
            {
                reserveController.AddPlomb();
                nextPlomb = difficulte;

            }

        }
    }

    public void Replay()
    {
        PrepareLevelX();
    }

    public void UpdateScoreAndCoins(int quintes)
    {
        coins = coins + quintes;

        Vector2Int values = new Vector2Int();
        values.x = quintes;
        values.y = scoreData.Score[quintes - 1];

        EventManager.TriggerEvent("UpdateScoreAndCoins", values);

        score += scoreData.Score[quintes - 1];

        uiManager.UpdateScore(score, scoreData.Score[quintes - 1]);

        /*
        for (int x = 0; x < quintes; x++)
        {
            reserveController.AddBille();
        }
        */

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
            placeBille.Pause();
            display.DropBilles();
        
        }

        compteurBilles++;
        if (compteurBilles >= difficulte)
        {
            placePlomb.PlacePlombAt(bPosition);
            //reserveController.RemoveBille();
            compteurBilles = 0;
        }

        //reserveController.RemoveBille();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void SetMusicVolume(float volume)
    {
        userDataManager.SaveMusicVolume(volume);
    }

    public void SetFxVolume(float volume)
    {
        userDataManager.SaveFxVolume(volume);
    }

    private void _OnToggleMusic()
    {

        //musicOn = musicOn == true ? false : true;
        //userDataManager.ToggleMusic(musicOn);

    }

    private void _OnToggleFX()
    {

        //fxOn = fxOn == true ? false : true;
        //userDataManager.ToggleFX(fxOn);

    }
}

public struct LevelStruct
{
    public int ID;
    public bool available;
    public int stars;
}