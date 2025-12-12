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
    [SerializeField] private TMP_Text leaderboardRankText;

    // score
    [SerializeField] private Transform scorePanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private ScoreBarController scoreBarController;
    [SerializeField] private GameObject scoreIncrementPrefab;
    [SerializeField] private float scoreToUpdateDuration = 2.0f;
    [SerializeField] private Vector2 scoreToUpdateOffset = new Vector2(10, 10);

    // options sons
    [SerializeField] private Transform slidersPanel;

    // options paramètres 
    [SerializeField] private GameObject restartButton;    
    [SerializeField] private GameObject quitToMenuButton;
    [SerializeField] private GameObject rulesButton;
    [SerializeField] private GameObject contactUsButton;
    [SerializeField] private TMP_Text prototypeExpirationText;

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
    private GameObject developerModeLabel;
    [SerializeField] private TMP_Text reserveBilleCounter;
    [SerializeField] private TMP_Text reservePlombCounter;


    private void Start()
    {
        UpdatePrototypeExpirationText();

        EventManager.AddListener("UpdateBilleCompteur", _OnUpdateBilleCompteur);
    }

    private void UpdatePrototypeExpirationText()
    {
        if (prototypeExpirationText != null)
        {
            prototypeExpirationText.text = "Private beta live";
        }
    }

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
        // Détecter si on est en jeu AVANT de masquer le scorePanel
        bool inGame = scorePanel.gameObject.activeSelf;

        if (!slidersPanel.gameObject.activeSelf)
        {
            slidersPanel.gameObject.SetActive(true);

            // Synchroniser le toggle developerMode avec la valeur actuelle
            UpdateDeveloperModeLabel(GameManager.Instance.DeveloperMode);

            // Masquer le RankPanel (menu principal) ou ScorePanel (en jeu)
            rankPanel.gameObject.SetActive(false);
            if (inGame)
            {
                scorePanel.gameObject.SetActive(false);
            }

            // Afficher/cacher les boutons selon le contexte
            restartButton.SetActive(inGame);
            quitToMenuButton.SetActive(inGame);
            rulesButton.SetActive(!inGame);
            contactUsButton.SetActive(!inGame);

        }
        else
        {
            slidersPanel.gameObject.SetActive(false);

            // Réafficher le panel approprié
            if (mainPanel.gameObject.activeSelf)
            {
                rankPanel.gameObject.SetActive(true);
                GameManager.Instance.PrepareMainMenu();
            }
            else
            {
                // On est en jeu, réafficher le scorePanel
                scorePanel.gameObject.SetActive(true);
            }

            restartButton.SetActive(false);
            quitToMenuButton.SetActive(false);
            rulesButton.SetActive(false);
            contactUsButton.SetActive(false);
        }
    }


    public void SetMainPanel(List<LevelStruct> list, string rank, int globalScore)
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

        rankText.text = totalStars.ToString(); //+ "/" + list.Count * 3;
        // mettre à jour le global score
        rankTitle.text = globalScore.ToString();
    

        UpdateLeaderboardRank();

        UpdateDeveloperModeLabel(GameManager.Instance.DeveloperMode);

    }

    private void UpdateLeaderboardRank()
    {
        if (leaderboardRankText != null)
        {
            // TODO: Remplacer par les vraies données Google Play Games
            leaderboardRankText.text = "Classement mondial (hors ligne)";
        }
    }

    public void SetLeaderboardRank(int rank, int totalPlayers)
    {
        if (leaderboardRankText != null)
        {
            leaderboardRankText.text = $"Classement : {rank} sur {totalPlayers}";
        }
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

    public void UpdateDeveloperModeLabel(bool isEnabled)
    {
        // Chercher le label s'il n'est pas encore assigné
        if (developerModeLabel == null)
        {
            var allObjects = Resources.FindObjectsOfTypeAll<Transform>();
            foreach (var t in allObjects)
            {
                if (t.name == "DeveloperModeLabel" && t.gameObject.scene.isLoaded)
                {
                    developerModeLabel = t.gameObject;
                    break;
                }
            }
        }

        if (developerModeLabel != null)
        {
            developerModeLabel.SetActive(isEnabled);
        }
    }
    public void RestartLevel()
    {
        GameManager.Instance.Replay();
        slidersPanel.gameObject.SetActive(false); // Fermer le panneau
    }

    public void QuitToMenu()
    {
        EventManager.TriggerEvent("ReturnToMainMenu");
        slidersPanel.gameObject.SetActive(false); // Fermer le panneau
    }

    public void GetRules()
    {
        Application.OpenURL("http://marble5.app/#concept");
    }

    public void ContactUs()
    {
        Application.OpenURL("http://marble5.app/#contact");
    }

    private void _OnUpdateBilleCompteur(object data)
    {
        int newCompteur = (int)data;
        reserveBilleCounter.text = newCompteur.ToString();
    }
}