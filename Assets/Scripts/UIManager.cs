using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform headerPanel;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Text scoreText;
    [SerializeField] private Transform quitPanel;
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Transform gameOverPanel;




    private int score;
    private int highScore;

    //[SerializeField] private Transform coinsPanel;
    //[SerializeField] private Text coinsText;
    //private int coins;
    //private int initialCoins;

    //[SerializeField] private Transform gameOverPanel;
    //[SerializeField] private Text yourScoreText;


    //private HighscoreManager highscoreManager;
    //[SerializeField] private Text highscoreText;

    //[SerializeField] private Button toggleFX;
    //[SerializeField] private Sprite fxOn;
    //[SerializeField] private Sprite fxOff;
    //[SerializeField] private Button QuitButton;

    private void Start()
    {

        gameOverPanel.gameObject.SetActive(false);
        headerPanel.gameObject.SetActive(false);
        quitPanel.gameObject.SetActive(false);

    }
    public void SetMainPanel(int levels)
    {

        mainPanel.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(false);
        headerPanel.gameObject.SetActive(false);

        ClearMenu();

        for (int x = 0; x < levels ; x++)
        {

            GameObject lp = Instantiate(levelPrefab, levelPanel);
                        
            lp.GetComponent<LevelSelector>().SetLevelID(x);

            Button button = lp.GetComponent<Button>();
            AddLevelButton(button, x);

        }
    }

    private void AddLevelButton(Button b, int x)
    {
        b.onClick.AddListener(() => GameManager.Instance.PrepareLevel(x));
    }

    private void ClearMenu()
    {
        foreach (Transform child in levelPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetGameMode()
    {
        mainPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        headerPanel.gameObject.SetActive(true);
    }

    public void GameOver()
    {

        //highscoreManager.AddHighscore(score);
        //UpdateHighscoreText();

        //gameOverPanel.gameObject.SetActive(true);
        //yourScoreText.text = "Your Score : " + score.ToString();

        headerPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(true);
        //coinsPanel.gameObject.SetActive(false);
    }

    void StartLevel() 
    {

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        //EventManager.AddListener("GameOver", _OnGameOver);
        EventManager.AddListener("Replay", _OnReplay);

        //initialCoins = GameManager.Instance.Coins;

        //string filePath = Application.persistentDataPath + "/highscores.json";
        //highscoreManager = new HighscoreManager(filePath);
        //highScore = highscoreManager.GetHighscores()[0];

        //SetupLevel();

    }



    private void SetupLevel()
    {

        //gameOverPanel.gameObject.SetActive(false);
        headerPanel.gameObject.SetActive(true);
        quitPanel.gameObject.SetActive(false);
        //coinsPanel.gameObject.SetActive(true);

        score = 0;
        //coins = initialCoins;

        highScoreText.text = highScore.ToString();
        scoreText.text = score.ToString();
        //coinsText.text = coins.ToString();

        //toggleFX.image.sprite = fxOn;

    }

    private void _OnReplay()
    {
        SetupLevel();
    }

    void _OnUpdateScoreAndCoins(object scoreAndCoinsToAdd)
    {

        Vector2Int values = (Vector2Int)scoreAndCoinsToAdd;

        //coins = coins + values.x;
        //coinsText.text = coins.ToString();

        score = score + values.y;
        scoreText.text = score.ToString();

    }

    void _OnPoseBille(object data)
    {
        //coins--;
        //coinsText.text = coins.ToString();
    }

    /*
    void _OnGameOver()
    {

        highscoreManager.AddHighscore(score);
        UpdateHighscoreText();

        //gameOverPanel.gameObject.SetActive(true);
        //yourScoreText.text = "Your Score : " + score.ToString();
        
        headerPanel.gameObject.SetActive(false);
        //coinsPanel.gameObject.SetActive(false);
    }
    */

    private void UpdateHighscoreText()
    {
        //highscoreText.text = "Highscores:\n" + string.Join("\n", highscoreManager.GetHighscores());
    }

    public void ShowQuitPanel()
    {
        quitPanel.gameObject.SetActive(true);
    }

    public void HideQuitPanel()
    {
        quitPanel.gameObject.SetActive(false);
    }

    /*
    public void ShowQuitPanel(bool show)
    {
        if (show)
        {
            quitPanel.gameObject.SetActive(true);
        }
        else
        {
            quitPanel.gameObject.SetActive(false);
        }
    }
    /*


    /*
    public void ToggleFXImage()
    {
        if (toggleFX.image.sprite = fxOn)
        {
            toggleFX.image.sprite = fxOff;
        }
        else
        {
            toggleFX.image.sprite = fxOn;
        }
    }*/

}
    
