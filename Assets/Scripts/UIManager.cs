using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    [SerializeField] private Transform highScorePanel;
    [SerializeField] private Text highScoreText;
    [SerializeField] private Transform scorePanel;
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject scoreIncrementPrefab;
    [SerializeField] private float scoreToUpdateDuration = 2.0f;
    [SerializeField] private Vector2 scoreToUpdateOffset = new Vector2(10, 10);
    [SerializeField] private Transform optionsPanel;
    [SerializeField] private Transform quitPanel;
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private GameObject levelPrefab;
    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private Transform reservePanel;

    //private Text scoreToUpdate;

    


    private void Start()
    {

        //scoreToUpdate = updateScore.gameObject.GetComponent<Text>();

    }

    public void SetMainPanel(List<LevelStruct> list)
    {

        mainPanel.gameObject.SetActive(true);
        optionsPanel.gameObject.SetActive(true);

        gameOverPanel.gameObject.SetActive(false);
        highScorePanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(false);
        quitPanel.gameObject.SetActive(false);
        reservePanel.gameObject.SetActive(false);

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
        reservePanel.gameObject.SetActive(true);

        mainPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);

        scoreText.text = "0";
    }

    public void GameOver()
    {

        gameOverPanel.gameObject.SetActive(true);

        mainPanel.gameObject.SetActive(false);
        highScorePanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(false);
        optionsPanel.gameObject.SetActive(false);
        reservePanel.gameObject.SetActive(false);

    }

    public void UpdateScore(int score, int increment)
    {

        scoreText.text = score.ToString();

        //StartCoroutine(ShowScoreIncrement(increment));

        ShowScoreIncrement(increment);


    }

    //private IEnumerator ShowScoreIncrement(int inc)
    private void ShowScoreIncrement(int inc)
    {

        //updateScore.gameObject.SetActive(true);

        GameObject go = Instantiate(scoreIncrementPrefab, scorePanel);
        //go.transform.SetParent(this.GetComponent<CanvasRenderer>().transform);
        go.GetComponent<Text>().text = "+" + inc.ToString();

        PositionneScoreIncrement(go);


        //scoreToUpdate.text = "+" + inc.ToString();
        //yield return new WaitForSeconds(scoreToUpdateDuration);

        Destroy(go, scoreToUpdateDuration);
        //updateScore.gameObject.SetActive(false);
    }

    private void PositionneScoreIncrement(GameObject go)
    {
        Vector2 mousePosition = Input.mousePosition;
        go.transform.position = mousePosition + scoreToUpdateOffset;
    }

    public void SetHighScoreText(int highScore)
    {
        highScoreText.text = highScore.ToString();
    }

    public void ConfirmQuit()
    {
        quitPanel.gameObject.SetActive(true);
    }

    public void CancelQuit()
    {
        quitPanel.gameObject.SetActive(false);
    }
}