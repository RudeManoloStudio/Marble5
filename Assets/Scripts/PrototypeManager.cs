using System;
using UnityEngine;
using TMPro;

public class PrototypeManager : MonoBehaviour
{
    // Date d'expiration du prototype
    private static readonly DateTime EXPIRATION_DATE = new DateTime(2026, 3, 31);

    [SerializeField] private GameObject expirationPanel;
    [SerializeField] private TMP_Text expirationMessageText;

    public static bool IsExpired => DateTime.Now > EXPIRATION_DATE;
    public static string ExpirationDateString => EXPIRATION_DATE.ToString("dd/MM/yyyy");

    private void Awake()
    {
        if (IsExpired)
        {
            ShowExpirationAndQuit();
        }
    }

    private void ShowExpirationAndQuit()
    {
        if (expirationPanel != null)
        {
            expirationPanel.SetActive(true);
        }

        if (expirationMessageText != null)
        {
            expirationMessageText.text = "Prototype Marble5 expiré";
        }

        Invoke(nameof(QuitGame), 10f);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }
}