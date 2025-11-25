using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    // rank
    [SerializeField] private Transform rankPanel;
    [SerializeField] private TMP_Text rankText;
    [SerializeField] private TMP_Text rankTitle;

    // score
    [SerializeField] private Transform scorePanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private ScoreBarController scoreBarController;
    [SerializeField] private GameObject scoreIncrementPrefab;
    [SerializeField] private float scoreToUpdateDuration = 2.0f;
    [SerializeField] private Vector2 scoreToUpdateOffset = new Vector2(10, 10);

    // options sons
    [SerializeField] private Transform slidersPanel;

    // main panel
    [SerializeField] private Transform mainPanel;
    [SerializeField] private Transform levelPanel;
    [SerializeField] private GameObject levelPrefab;

    // gameover
    [SerializeField] private Transform gameOverPanel;
    [SerializeField] private Transform gameOverScorePanel;
    [SerializeField] private Transform gameOverNewHighscorePanel;
    [SerializeField] private TMP_Text finalScore;
    [SerializeField] private TMP_Text bestScore;
    [SerializeField] private TMP_Text newBestScore;
    [SerializeField] private StarRecapController starRecapController;

    // reserve
    [SerializeField] private Transform reservePanel;
    [SerializeField] private Toggle developerModeToggle;
    [SerializeField] private TMP_Text reserveBilleCounter;
    [SerializeField] private TMP_Text reservePlombCounter;

    public void UpdateReserveBilleCounter(int count)
    {
        if (count == 1)
        {
            reserveBilleCounter.color = Color.red;
        }
        else
        {
            reserveBilleCounter.color = Color.white;
        }

        reserveBilleCounter.text = count.ToString();
    }

    public void UpdateReservePlombCounter(int count)
    {
        if (count == 1)
        {
            reservePlombCounter.color = Color.red;
        }
        else
        {
            reservePlombCounter.color = Color.white;
        }

        reservePlombCounter.text = count.ToString();
    }


    public void ToggleSliders()
    {
        if (!slidersPanel.gameObject.activeSelf)
        {
            slidersPanel.gameObject.SetActive(true);
        }
        else
        {
            slidersPanel.gameObject.SetActive(false);
            // Rafraîchir le menu après fermeture des paramètres
            GameManager.Instance.PrepareMainMenu();
        }
    }

    public void SetMainPanel(List<LevelStruct> list, string rank)
    {

        mainPanel.gameObject.SetActive(true);
        rankPanel.gameObject.SetActive(true);

        gameOverPanel.gameObject.SetActive(false);
        scorePanel.gameObject.SetActive(false);
        slidersPanel.gameObject.SetActive(false);
        reservePanel.gameObject.SetActive(false);

        ClearMenu();

        int totalStars = 0;

        for (int x = 0; x < list.Count; x++)
        {

            totalStars += list[x].stars;

            GameObject lp = Instantiate(levelPrefab, levelPanel);

            lp.GetComponent<LevelSelector>().SetLevelParameters(list[x]);

            Button button = lp.GetComponent<Button>();
            AddLevelButton(button, x);

        }

        rankText.text = totalStars.ToString() + "/" + list.Count * 3;
        rankTitle.text = rank;

        InitializeDeveloperToggle(GameManager.Instance.DeveloperMode);

    }

    private void AddLevelButton(Button b, int x)
    {
        b.onClick.AddListener(() => EventManager.TriggerEvent("LevelSelected", x));
    }

    private void ClearMenu()
    {
        foreach (Transform child in levelPanel)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetGameMode(Vector3Int starsScore)
    {

        scorePanel.gameObject.SetActive(true);
        reservePanel.gameObject.SetActive(true);

        mainPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        rankPanel.gameObject.SetActive(false);

        scoreText.text = "0";
        scoreBarController.SetStars(starsScore);
    }

    public void HideReservePanel()
    {
        reservePanel.gameObject.SetActive(false);
    }

    public void GameOver(int score, int highscore)
    {

        gameOverPanel.gameObject.SetActive(true);

        scorePanel.gameObject.SetActive(false);

        if (score >= highscore)
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

        int starsToHighlight = scoreBarController.GetStars();
        starRecapController.SetStarsHighlight(starsToHighlight);

    }
        public void UpdateScore(int score, int increment, int quinteCount)
    {
        scoreText.text = score.ToString();
        ShowScoreIncrement(increment, quinteCount);
        scoreBarController.AddScore(increment);
    }

    private void ShowScoreIncrement(int inc, int quinteCount)  // ← Ajouter quinteCount ici
    {
        GameObject go = Instantiate(scoreIncrementPrefab, scorePanel);
        TMP_Text textComponent = go.GetComponent<TMP_Text>();
        textComponent.text = "+" + inc.ToString();

        float fontSize = 36f + (quinteCount - 1) * 20f;
        textComponent.fontSize = fontSize;

        PositionneScoreIncrement(go);

        UpdateScoreIncrement flyScript = go.GetComponent<UpdateScoreIncrement>();
        if (flyScript != null)
        {
            flyScript.FlyToScore(scoreText);
        }
    }   

    private void PositionneScoreIncrement(GameObject go)
    {
        Vector2 mousePosition = Input.mousePosition;
        go.transform.position = mousePosition + scoreToUpdateOffset;
    }

    public void ReturnToMain()
    {
        EventManager.TriggerEvent("ReturnToMainMenu");
    }

    public void OnDeveloperModeToggled(bool isEnabled)
    {
        GameManager.Instance.SetDeveloperMode(isEnabled);
    }

    public void InitializeDeveloperToggle(bool currentValue)
    {
        if (developerModeToggle != null)
        {
            // Désactiver temporairement l'événement pour éviter de déclencher OnValueChanged
            developerModeToggle.onValueChanged.RemoveListener(OnDeveloperModeToggled);

            // Mettre à jour la valeur visuelle
            developerModeToggle.isOn = currentValue;

            // Réactiver l'événement
            developerModeToggle.onValueChanged.AddListener(OnDeveloperModeToggled);
        }
    }

}