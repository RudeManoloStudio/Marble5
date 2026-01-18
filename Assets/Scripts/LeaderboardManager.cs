using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

/// <summary>
/// Gère l'authentification Google Play Games et la soumission des scores au leaderboard.
/// Singleton accessible via LeaderboardManager.Instance
/// </summary>
public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance { get; private set; }

    private bool isAuthenticated = false;

    public bool IsAuthenticated
    {
        get { return isAuthenticated; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        InitializePlayGames();
    }

    /// <summary>
    /// Initialise Google Play Games et tente l'authentification automatique.
    /// </summary>
    private void InitializePlayGames()
    {
        PlayGamesPlatform.Activate();
        Authenticate();
    }

    /// <summary>
    /// Authentifie le joueur auprès de Google Play Games.
    /// </summary>
    public void Authenticate()
    {
        PlayGamesPlatform.Instance.Authenticate(OnAuthenticateCallback);
    }

    private void OnAuthenticateCallback(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            isAuthenticated = true;
            Debug.Log("[LeaderboardManager] Authentification réussie : " + Social.localUser.userName);
        }
        else
        {
            isAuthenticated = false;
            Debug.LogWarning("[LeaderboardManager] Authentification échouée : " + status);
        }
    }

    /// <summary>
    /// Soumet le score global au leaderboard Google Play Games.
    /// </summary>
    /// <param name="score">Le score à soumettre</param>
    public void SubmitScore(int score)
    {
        if (!isAuthenticated)
        {
            Debug.LogWarning("[LeaderboardManager] Non authentifié, score non soumis.");
            return;
        }

        Social.ReportScore(score, GPGSIds.leaderboard_global_score, (bool success) =>
        {
            if (success)
            {
                Debug.Log("[LeaderboardManager] Score soumis avec succès : " + score);
            }
            else
            {
                Debug.LogWarning("[LeaderboardManager] Échec de la soumission du score.");
            }
        });
    }

    /// <summary>
    /// Affiche l'interface native Google Play Games du leaderboard.
    /// </summary>
    public void ShowLeaderboard()
    {
        if (!isAuthenticated)
        {
            Debug.LogWarning("[LeaderboardManager] Non authentifié, impossible d'afficher le leaderboard.");
            // Optionnel : tenter une nouvelle authentification
            Authenticate();
            return;
        }

        PlayGamesPlatform.Instance.ShowLeaderboardUI(GPGSIds.leaderboard_global_score);
    }

    /// <summary>
    /// Affiche tous les leaderboards disponibles.
    /// </summary>
    public void ShowAllLeaderboards()
    {
        if (!isAuthenticated)
        {
            Debug.LogWarning("[LeaderboardManager] Non authentifié, impossible d'afficher les leaderboards.");
            return;
        }

        Social.ShowLeaderboardUI();
    }
}
