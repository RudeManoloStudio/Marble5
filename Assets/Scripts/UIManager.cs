using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    //[SerializeField] private Transform headerPanel;
    [SerializeField] private Transform highScorePanel;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Transform scorePanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private Transform optionsPanel;
    [SerializeField] private Transform quitPanel;
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Transform gameOverPanel;

    private void Start()
    {

        //gameOverPanel.gameObject.SetActive(false);
        //headerPanel.gameObject.SetActive(false);
        //highScorePanel.gameObject.SetActive(false);
        //scorePanel.gameObject.SetActive(false);
        //quitPanel.gameObject.SetActive(false);

    }
    public void SetMainPanel(List<LevelStruct> list)
    {

        mainPanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(true);

        gameOverPanel.gameObject.SetActive(false);
        highScorePanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(false);
        quitPanel.gameObject.SetActive(false);
        //headerPanel.gameObject.SetActive(false);

        ClearMenu();

        for (int x = 0; x < list.Count; x++)
        {

            GameObject lp = Instantiate(levelPrefab, levelPanel);

            lp.GetComponent<LevelSelector>().SetLevelParameters(list[x]);

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
        highScorePanel.gameObject.SetActive(true);
        scorePanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(true);

        mainPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        //headerPanel.gameObject.SetActive(true);
        scoreText.text = "0";
    }

    public void GameOver()
    {

        //headerPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(true);

        mainPanel.gameObject.SetActive(false);
        highScorePanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);

    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetHighScoreText(int highScore)
    {
        highScoreText.text = highScore.ToString();
    }
}