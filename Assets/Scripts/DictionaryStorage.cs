using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Wrapper pour stocker les données avec leur hash de validation
/// </summary>
[Serializable]
public class SecureData
{
    public string data;
    public string hash;
}

public class DictionaryStorage
{
    // Clé secrète pour le hash (à personnaliser)
    private static readonly string SECRET_KEY = "Marble5_S3cr3t_K3y_2024!";

    /// <summary>
    /// Calcule le hash SHA256 des données + clé secrète
    /// </summary>
    private static string ComputeHash(string data)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            string toHash = data + SECRET_KEY;
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(toHash));

            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }

    /// <summary>
    /// Sauvegarde un dictionnaire avec hash de validation
    /// </summary>
    public static void SaveDictionaryToFile<T, U>(Dictionary<T, U> dictionary, string fileName)
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            // Sérialisation des données
            string json = JsonConvert.SerializeObject(dictionary);

            // Création de l'objet sécurisé avec hash
            SecureData secureData = new SecureData
            {
                data = json,
                hash = ComputeHash(json)
            };

            // Sérialisation finale
            string finalJson = JsonConvert.SerializeObject(secureData, Formatting.Indented);
            File.WriteAllText(filePath, finalJson);

            Debug.Log($"Dictionnaire sauvegardé avec validation dans {filePath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors de la sauvegarde : {ex.Message}");
        }
    }

    /// <summary>
    /// Charge un dictionnaire et vérifie son intégrité
    /// </summary>
    public static Dictionary<T, U> LoadDictionaryFromFile<T, U>(string fileName)
    {
        try
        {
            string filePath = Path.Combine(Application.persistentDataPath, fileName);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning("Fichier inexistant.");
                return null;
            }

            string fileContent = File.ReadAllText(filePath);

            // Tenter de charger comme SecureData
            SecureData secureData = JsonConvert.DeserializeObject<SecureData>(fileContent);

            if (secureData == null || string.IsNullOrEmpty(secureData.data))
            {
                Debug.LogWarning("Format de fichier invalide.");
                return null;
            }

            // Vérifier le hash
            string expectedHash = ComputeHash(secureData.data);
            if (secureData.hash != expectedHash)
            {
                Debug.LogWarning("Intégrité compromise ! Scores réinitialisés.");
                // Supprimer le fichier corrompu
                File.Delete(filePath);
                return null;
            }

            // Hash valide, charger les données
            Dictionary<T, U> dictionary = JsonConvert.DeserializeObject<Dictionary<T, U>>(secureData.data);
            Debug.Log("Dictionnaire chargé avec succès (intégrité vérifiée).");
            return dictionary;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Erreur lors du chargement : {ex.Message}");
            return null;
        }
    }
}