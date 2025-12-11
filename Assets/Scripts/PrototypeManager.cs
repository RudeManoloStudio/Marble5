using UnityEngine;

/// <summary>
/// Gestionnaire de prototype - système d'expiration désactivé pour bêta privée ouverte
/// </summary>
public class PrototypeManager : MonoBehaviour
{
    // Système d'expiration désactivé - bêta privée sans restriction
    // [SerializeField] private GameObject expirationPanel;
    // [SerializeField] private TMP_Text expirationMessageText;
    // private static readonly DateTime EXPIRATION_DATE = new DateTime(2026, 3, 31);
    // public static bool IsExpired => DateTime.Now > EXPIRATION_DATE;
    // public static string ExpirationDateString => EXPIRATION_DATE.ToString("dd/MM/yyyy");

    public static bool IsExpired => false;
}