using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    //[SerializeField] private Text ScoreText;
    [SerializeField] private Transform scorePanel;
    private Text scoreText;
    private int score = 0;

    //[SerializeField] private Text CoinsText;
    [SerializeField] private Transform coinsPanel;
    private Text coinsText;
    private int coins;

    [SerializeField] private Transform gameOverPanel;
    
    void Start () 
    {

        gameOverPanel.gameObject.SetActive(false);

        EventManager.AddListener("UpdateScore", _OnUpdateScore);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("GameOver", _OnGameOver);

        //ScoreText.text = score.ToString();
        scoreText = scorePanel.GetComponent<Text>();
        scoreText.text = score.ToString();

        coins = GameManager.Instance.Coins;
        //CoinsText.text = coins.ToString();
        coinsText = coinsPanel.GetComponent<Text>();
        coinsText.text = coins.ToString();

    }

    void _OnUpdateScore()
    {
        score++;
        //ScoreText.text = score.ToString();
        scoreText.text = score.ToString();

        coins++;
        //CoinsText.text = coins.ToString();
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
    
