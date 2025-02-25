using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform scorePanel;
    private Text scoreText;
    private int score;

    [SerializeField] private Transform coinsPanel;
    private Text coinsText;
    private int coins;
    private int initialCoins;

    [SerializeField] private Transform gameOverPanel;
    
    void Start () 
    {

        EventManager.AddListener("UpdateScore", _OnUpdateScore);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("GameOver", _OnGameOver);
        EventManager.AddListener("Replay", _OnReplay);

        scoreText = scorePanel.GetComponent<Text>();
        coinsText = coinsPanel.GetComponent<Text>();

        initialCoins = GameManager.Instance.Coins;

        Setup();

    }

    private void Setup()
    {
        gameOverPanel.gameObject.SetActive(false);

        score = 0;
        coins = initialCoins;

        scoreText.text = score.ToString();
        coinsText.text = coins.ToString();

    }

    private void _OnReplay()
    {
        Setup();
    }

    void _OnUpdateScore()
    {
        score++;
        scoreText.text = score.ToString();

        coins++;
        coinsText.text = coins.ToString();
    }

    void _OnPoseBille()
    {
        coins--;
        coinsText.text = coins.ToString();
    }

    void _OnGameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
        scorePanel.gameObject.SetActive(false);
        coinsPanel.gameObject.SetActive(false);

    }
}
    
