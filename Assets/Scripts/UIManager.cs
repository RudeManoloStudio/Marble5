using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Text ScoreText;
    private int score = 0;

    [SerializeField] private Text CoinsText;
    private int coins;

    [SerializeField] private Transform gameOverPanel;
    
    void Start () 
    {

        gameOverPanel.gameObject.SetActive(false);

        EventManager.AddListener("UpdateScore", _OnUpdateScore);
        EventManager.AddListener("PoseBille", _OnPoseBille);
        EventManager.AddListener("GameOver", _OnGameOver);

        ScoreText.text = score.ToString();

        coins = GameManager.Instance.Coins;
        CoinsText.text = coins.ToString();

    }

    void _OnUpdateScore()
    {
        score++;
        ScoreText.text = score.ToString();

        coins++;
        CoinsText.text = coins.ToString();
    }

    void _OnPoseBille()
    {
        coins--;
        CoinsText.text = coins.ToString();
    }

    void _OnGameOver()
    {
        gameOverPanel.gameObject.SetActive(true);
    }
}
    
