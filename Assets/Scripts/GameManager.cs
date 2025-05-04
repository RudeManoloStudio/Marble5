using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelData levelData;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DisplayController display;
    [SerializeField] private FXData fxData;
    [SerializeField] private MusicData musicData;
    [SerializeField] private RankingData rankingData;
    [SerializeField] private int initialCoins = 5;
    [SerializeField] private bool infinisCoins = false;
    [SerializeField] private ScoreData scoreData;
    [SerializeField] private Sprite initialBackground;
    [SerializeField] private ReserveController reserveController;
    [SerializeField] private float cameraShakeDuration;
    [SerializeField] private float dropBillesDuration;

    private Vector2Int gridSize;
    private PlaceBille placeBille;
    private PlacePlomb placePlomb;
    private int compteurBilles;
    private int difficulte;
    private bool isDifficulte = true;
    private int score;
    private int level;
    private int handicap;
    private UserDataManager userDataManager;
    private UserData userData;
    private Dictionary<int, int> scores;
    private int coins;
    private int highScore;
    private float musicVolume;
    private float fxVolume;
    private Camera _camera;
    private int totalStars;


    public static GameManager Instance { get; private set; }

    public Vector2Int GridSize
    {
        get { return gridSize; }
    }

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

    public int TotalStars
    {
        get { return totalStars; }
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

        // FX
        fxVolume = userData.fxVolume;

    }

    private void Start()
    {

        _camera = Camera.main;

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

        if (scores.ContainsKey(level))
        {
            highScore = scores[level];
        }
        else
        {
            highScore = 0;
        }

        gridSize = levelData.layers[level].GridSize;
        handicap = levelData.layers[level].Handicap;

        Vector3Int starsScore = new Vector3Int(levelData.layers[level].FirstStarScore, levelData.layers[level].SecondStarScore, levelData.layers[level].ThirdStarScore);

        uiManager.SetGameMode(starsScore);
        placeBille.Unpause();

        // positionnement de la camera
        _camera.GetComponent<CameraController>().Setup(gridSize);

        // bille + plomb + background + grid
        display.ClearBoard();
        display.SetBilleAndPlomb(levelData.layers[level].Bille, levelData.layers[level].Plomb);
        display.PrepareBackgroundAndGrid(gridSize, levelData.layers[level].BackgroundTexture);
        if (levelData.layers[level].Motif != null) { display.PrepareMotif(gridSize, levelData.layers[level].Motif, handicap); }
        display.PrepareReserve();

        placeBille.Setup(gridSize, levelData.layers[level].Bille, levelData.layers[level].Quinte);
        placePlomb.Setup(gridSize, levelData.layers[level].Plomb);

        difficulte = levelData.layers[level].Difficulte;
        if (difficulte <= 0)
        {
            isDifficulte = false;
        }
        else
        {
            isDifficulte = true;
        }

        coins = initialCoins;
        compteurBilles = 0;
        score = 0;

        uiManager.UpdateReserveBilleCounter(coins);
        uiManager.UpdateReservePlombCounter(difficulte);

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
        uiManager.UpdateReserveBilleCounter(coins);

    }

    private void _OnPoseBille(object billePosition)
    {

        Vector3 bPosition = (Vector3)(billePosition);
        coins--;
        uiManager.UpdateReserveBilleCounter(coins);

        // gameover
        if (coins <= 0 && !infinisCoins)
        {
            
            if (scores.ContainsKey(level))
            {
                if (score >= scores[level])
                {
                    scores[level] = score;
                    highScore = score;
                }
                else
                {
                    highScore = scores[level];
                }
            }
            else
            {
                scores.Add(level, score);
                highScore = score;
            }

            // Sauvegarde du dictionnaire
            DictionaryStorage.SaveDictionaryToFile(scores, "dictionnaire.json");

            // ici calcul du nombre d'étoiles total
            // mise à jour de la propriété TotalStars
            totalStars = 0;
            //scores = DictionaryStorage.LoadDictionaryFromFile<int, int>("dictionnaire.json");
            foreach (KeyValuePair<int, int> kvp in scores)
            {

                if (kvp.Value >= levelData.layers[kvp.Key].FirstStarScore) totalStars++;
                if (kvp.Value >= levelData.layers[kvp.Key].SecondStarScore) totalStars++;
                if (kvp.Value >= levelData.layers[kvp.Key].ThirdStarScore) totalStars++;

            }

            // gameover sequence
            placeBille.Pause();
            StartCoroutine("GameOverSequence");



        }

        if (isDifficulte)
        {
            compteurBilles++;
            if (compteurBilles >= difficulte)
            {
                placePlomb.PlacePlombAt(bPosition);
                compteurBilles = 0;
                uiManager.UpdateReservePlombCounter(difficulte);
            }
            else
            {
                uiManager.UpdateReservePlombCounter(difficulte - compteurBilles);
            }
        }
    }

    private IEnumerator GameOverSequence()
    {

        uiManager.HideReservePanel();

        _camera.GetComponent<CameraShake>().Shake(cameraShakeDuration);
        yield return new WaitForSeconds(cameraShakeDuration);

        display.AnimBilles();
        yield return new WaitForSeconds(cameraShakeDuration + dropBillesDuration);

        uiManager.GameOver(score, highScore);

    }


    private IEnumerator Co_DropBilles()
    {

        yield return new WaitForSeconds(dropBillesDuration);

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
}

public struct LevelStruct
{
    public int ID;
    public bool available;
    public int stars;
}