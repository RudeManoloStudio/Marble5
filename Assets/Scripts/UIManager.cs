using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform scorePanel;
    [SerializeField] private Text scoreText;
    private int score;

    [SerializeField] private Transform coinsPanel;
    [SerializeField] private Text coinsText;
    private int coins;
    private int initialCoins;

    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private Text yourScoreText;


    private HighscoreManager highscoreManager;
    [SerializeField] private Text highscoreText;


    void Start () 
    {

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("GameOver", _OnGameOver);
        EventManager.AddListener("Replay", _OnReplay);

        initialCoins = GameManager.Instance.Coins;

        //highscoreManager = GetComponent<HighscoreManager>();
        string filePath = Application.persistentDataPath + "/highscores.json";
        highscoreManager = new HighscoreManager(filePath);
        //UpdateHighscoreText();

        Setup();

    }

    private void Setup()
    {

        gameOverPanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(true);
        coinsPanel.gameObject.SetActive(true);

        score = 0;
        coins = initialCoins;

        scoreText.text = score.ToString();
        coinsText.text = coins.ToString();

    }

    private void _OnReplay()
    {
        Setup();
    }

    void _OnUpdateScoreAndCoins(object scoreAndCoinsToAdd)
    {

        //int[] values = (int[])scoreAndCoinsToAdd;
        Vector2Int values = (Vector2Int)scoreAndCoinsToAdd;

        //coins++;
        coins = coins + values.x;
        coinsText.text = coins.ToString();

        //score++;
        score = score + values.y;
        scoreText.text = score.ToString();

    }

    void _OnPoseBille(object data)
    {
        coins--;
        coinsText.text = coins.ToString();
    }

    void _OnGameOver()
    {
        highscoreManager.AddHighscore(score);
        UpdateHighscoreText();

        //highscoreManager.AddHighscore(score);
        //highscoreText.text = highscoreManager.UpdateHighscoreText();

        gameOverPanel.gameObject.SetActive(true);
        yourScoreText.text = "Your Score : " + score.ToString();
        
        scorePanel.gameObject.SetActive(false);
        coinsPanel.gameObject.SetActive(false);

    }

    private void UpdateHighscoreText()
    {
        highscoreText.text = "Highscores:\n" + string.Join("\n", highscoreManager.GetHighscores());
    }
}
    
