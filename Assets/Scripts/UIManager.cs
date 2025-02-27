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
    [SerializeField] private Text yourScoreText;
    //private Text yourScoreText;
    
    void Start () 
    {

        EventManager.AddListener("UpdateScoreAndCoins", _OnUpdateScoreAndCoins);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("GameOver", _OnGameOver);
        EventManager.AddListener("Replay", _OnReplay);

        scoreText = scorePanel.GetComponent<Text>();
        coinsText = coinsPanel.GetComponent<Text>();
        //yourScoreText = yourScorePanel;
        //Debug.Log(yourScoreText);

        initialCoins = GameManager.Instance.Coins;

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

        int[] values = (int[])scoreAndCoinsToAdd;

        //coins++;
        coins = coins + values[0];
        coinsText.text = coins.ToString();

        //score++;
        score = score + values[1];
        scoreText.text = score.ToString();

    }

    void _OnPoseBille()
    {
        coins--;
        coinsText.text = coins.ToString();
    }

    void _OnGameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
        yourScoreText.text = "Your Score : " + score.ToString();
        

        scorePanel.gameObject.SetActive(false);
        coinsPanel.gameObject.SetActive(false);

    }
}
    
