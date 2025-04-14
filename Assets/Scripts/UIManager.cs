using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    // score
    [SerializeField] private Transform scorePanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject scoreIncrementPrefab;
    [SerializeField] private float scoreToUpdateDuration = 2.0f;
    [SerializeField] private Vector2 scoreToUpdateOffset = new Vector2(10, 10);

    // quit ou menu
    [SerializeField] private Transform quitButton;
    [SerializeField] private Transform menuButton;

    //options sons
    [SerializeField] private Transform slidersPanel;

    // quit confirmation
    [SerializeField] private Transform quitPanel;

    // abort confirmation
    [SerializeField] private Transform abortPanel;

    // main panel
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private GameObject levelPrefab;

    //gameover
    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private Transform gameOverScorePanel;
    [SerializeField] private Transform gameOverNewHighscorePanel;
    [SerializeField] private Text finalScore;
    [SerializeField] private Text bestScore;
    [SerializeField] private Text newBestScore;

    public void ToggleSliders()
    {
        if (!slidersPanel.gameObject.activeSelf)
        {
            slidersPanel.gameObject.SetActive(true);
        }
        else
        {
            slidersPanel.gameObject.SetActive(false);
        }
    }

    public void SetMainPanel(List<LevelStruct> list)
    {

        mainPanel.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);

        gameOverPanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(false);
        quitPanel.gameObject.SetActive(false);
        slidersPanel.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);
        abortPanel.gameObject.SetActive(false);

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

        scorePanel.gameObject.SetActive(true);
        menuButton.gameObject.SetActive(true);

        mainPanel.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);

        scoreText.text = "0";
    }

    public void GameOver(int score, int highscore)
    {

        gameOverPanel.gameObject.SetActive(true);

        scorePanel.gameObject.SetActive(false);
        menuButton.gameObject.SetActive(false);

        if (score == highscore)
        {
            gameOverNewHighscorePanel.gameObject.SetActive(true);
            gameOverScorePanel.gameObject.SetActive(false);

            newBestScore.text = score.ToString();
        }
        else
        {
            gameOverScorePanel.gameObject.SetActive(true);
            gameOverNewHighscorePanel.gameObject.SetActive(false);

            finalScore.text = score.ToString();
            bestScore.text = highscore.ToString();
        }

    }

    public void UpdateScore(int score, int increment)
    {

        scoreText.text = score.ToString();
        ShowScoreIncrement(increment);

    }

    private void ShowScoreIncrement(int inc)
    {

        GameObject go = Instantiate(scoreIncrementPrefab, scorePanel);
        go.GetComponent<Text>().text = "+" + inc.ToString();

        PositionneScoreIncrement(go);

        Destroy(go, scoreToUpdateDuration);

    }

    private void PositionneScoreIncrement(GameObject go)
    {
        Vector2 mousePosition = Input.mousePosition;
        go.transform.position = mousePosition + scoreToUpdateOffset;
    }

    public void SetHighScoreText(int highScore)
    {
        //highScoreText.text = highScore.ToString();
    }

    public void ConfirmQuit()
    {
        quitPanel.gameObject.SetActive(true);
    }

    public void CancelQuit()
    {
        quitPanel.gameObject.SetActive(false);
    }

    public void ConfirmAbort()
    {
        abortPanel.gameObject.SetActive(true);
    }

    public void CancelAbort()
    {
        abortPanel.gameObject.SetActive(false);
    }

    public void ReturnToMain()
    {
        GameManager.Instance.PrepareMainMenu();
    }
}