using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] private LevelData levelData;
    [SerializeField] private UIManager uiManager;
    [SerializeField] private DisplayController display;
    [SerializeField] private ReserveController reserveController;
    [SerializeField] private FXData fxData;
    [SerializeField] private MusicData musicData;
    [SerializeField] private RankingData rankingData;
    [SerializeField] private int initialCoins = 5;
    [SerializeField] private bool infinisCoins = false;
    [SerializeField] private ScoreData scoreData;
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
    private int globalScore;
    private string rank;
    private bool developerMode;
    private bool inGame;

    public static GameManager Instance { get; private set; }

    public bool InGame
    {
        get { return inGame; }
        set { inGame = value; }
    }

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

    
    public int TotalStars
    {
        get { return totalStars; }
    }

    public int GlobalScore
    {
        get { return globalScore; }
    }

    public bool DeveloperMode
    {
        get { return developerMode; }
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
        // Developer Mode - toujours désactivé au lancement
        developerMode = false;

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
        EventManager.AddListener("QuinteFormee", _OnQuinteFormee);
        EventManager.AddListener("LevelSelected", _OnLevelSelected);
        EventManager.AddListener("ReturnToMainMenu", _OnReturnToMainMenu);

        // Initialise MusicManager
        MusicManager musicManager = FindObjectOfType<MusicManager>();
        if (musicManager != null)
        {
            musicManager.Setup(musicVolume, musicData.Playlist);
        }

        // Ecoute les changements de volume
        EventManager.AddListener("MusicVolumeChanged", _OnMusicVolumeChanged);

        // Initialise FXManager
        FXManager fxManager = FindObjectOfType<FXManager>();
        if (fxManager != null)
        {
            fxManager.Setup(fxVolume, fxData);
        }

        // Ecoute les changements de volume FX
        EventManager.AddListener("FxVolumeChanged", _OnFxVolumeChanged);

    }

    private void _OnMusicVolumeChanged(object data)
    {
        float volume = (float)data;
        SetMusicVolume(volume);
    }

    private void _OnFxVolumeChanged(object data)
    {
        float volume = (float)data;
        SetFxVolume(volume);
    }

    private void _OnLevelSelected(object data)
    {
        int level = (int)data;
        PrepareLevel(level);
    }

    private void _OnReturnToMainMenu()
    {
        PrepareMainMenu();
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
                if (developerMode) // ← MODE DEV : tout est dispo
                {
                    levelStruct.available = true;

                }
                else if (nextLevelAvailable) // ← MODE NORMAL
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
                stars = CalculateStars(value, levelData.layers[x]);
            }

            if (stars > 0)
            {
                levelStruct.available = true;
                levelStruct.stars = stars;

                nextLevelAvailable = true;
            }

            list.Add(levelStruct);
        }

        // Calcul du score global et du total d'étoiles
        CalculateGlobalScore();

        // on calcule le rank
        for (int x = rankingData.layers.Length - 1; x >= 0; x--)
        {
            if (totalStars >= rankingData.layers[x].Stars)
            {
                rank = rankingData.layers[x].Rank;
                break;
            }
        }

        uiManager.SetMainPanel(list, rank, globalScore);
        display.ResetBoard();
    }

    public void SetDeveloperMode(bool isEnabled)
    {
        developerMode = isEnabled;
        userDataManager.SaveDeveloperMode(isEnabled);
        uiManager.UpdateDeveloperModeLabel(isEnabled);
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
        inGame = true;

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

        // Nouvelle réserve visuelle
        reserveController.Setup(
            levelData.layers[level].Bille,
            levelData.layers[level].Plomb,
            coins,
            difficulte
        );

    }
        private void _OnQuinteFormee(object data)
    {
        int quinteTrouvees = (int)data;
        UpdateScoreAndCoins(quinteTrouvees);
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

        uiManager.UpdateScore(score, scoreData.Score[quintes - 1], quintes);
        reserveController.AjouterBilles(quintes);

    }

    private void _OnPoseBille(object billePosition)
    {

        Vector3 bPosition = (Vector3)(billePosition);
        coins--;
        reserveController.ConsommerBille();

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

            // Mise à jour du score global et du total d'étoiles
            CalculateGlobalScore();

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

    /// <summary>
    /// Calcule le nombre d'étoiles obtenues pour un score donné sur un niveau donné.
    /// </summary>
    /// <param name="score">Le score obtenu</param>
    /// <param name="levelLayer">Les données du niveau (LevelData.Layer)</param>
    /// <returns>Nombre d'étoiles (0 à 3)</returns>
    private int CalculateStars(int score, LevelData.Layer levelLayer)
    {
        int stars = 0;

        if (score >= levelLayer.FirstStarScore) stars++;
        if (score >= levelLayer.SecondStarScore) stars++;
        if (score >= levelLayer.ThirdStarScore) stars++;

        return stars;
    }

    /// <summary>
    /// Calcule le score global basé sur les scores de tous les niveaux.
    /// Formule : score × multiplicateur selon le nombre d'étoiles
    /// 1 étoile = ×1, 2 étoiles = ×1.5, 3 étoiles = ×2
    /// </summary>
    private void CalculateGlobalScore()
    {
        globalScore = 0;
        totalStars = 0;

        foreach (KeyValuePair<int, int> kvp in scores)
        {
            int levelId = kvp.Key;
            int levelScore = kvp.Value;
            int stars = CalculateStars(levelScore, levelData.layers[levelId]);

            totalStars += stars;

            // Multiplicateur selon le nombre d'étoiles
            float multiplier = 0f;
            if (stars == 1) multiplier = 1.0f;
            else if (stars == 2) multiplier = 1.5f;
            else if (stars == 3) multiplier = 2.0f;

            globalScore += Mathf.RoundToInt(levelScore * multiplier);
        }
    }
}

public struct LevelStruct
{
    public int ID;
    public bool available;
    public int stars;
}