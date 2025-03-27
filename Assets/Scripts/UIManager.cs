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

    private void Start()
    {

        gameOverPanel.gameObject.SetActive(false);
        headerPanel.gameObject.SetActive(false);
        quitPanel.gameObject.SetActive(false);

    }
    public void SetMainPanel(List<LevelStruct> list)
    {

        mainPanel.gameObject.SetActive(true);
        gameOverPanel.gameObject.SetActive(false);
        headerPanel.gameObject.SetActive(false);

        ClearMenu();

        Debug.Log("ui : " + list.Count);

        // préparer le premier level, toujours available
        //GameObject lp = Instantiate(levelPrefab, levelPanel);

        //lp.GetComponent<LevelSelector>().SetLevelParameters(list[0]);

        //Button button = lp.GetComponent<Button>();
        //AddLevelButton(button, 0);

        // puis les levels suivants
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

        mainPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        headerPanel.gameObject.SetActive(true);
        scoreText.text = "0";
    }

    public void GameOver()
    {

        headerPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(true);
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